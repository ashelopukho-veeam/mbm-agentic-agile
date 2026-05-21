using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Quartz;

namespace mbt.webapi.Jobs;

public class MailProcessorJob : IJob
{
    private readonly IMailRepository _mailRepository;
    private readonly IApiService _apiService;

    private record MailGroup(string Group, string To);

    public MailProcessorJob(
        IMailRepository mailRepository, IApiService apiService)
    {
        _mailRepository = mailRepository;
        _apiService = apiService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var hostUrl = (await _apiService.GetAppConfigAsync()).ClientHostUrl;
        var tokenProcessor = new MailTokenProcessor(hostUrl);

        var unprocessedMails = await _mailRepository.GetUnprocessed();

        var groupedMails = unprocessedMails.GroupBy(m => new MailGroup(m.Group, m.To));

        foreach (var group in groupedMails)
        {
            var processedMailIds = await ProcessGroup(group, tokenProcessor);
            await MarkAsProcessed(processedMailIds);
        }
    }

    private async Task<List<string>> ProcessGroup(IGrouping<MailGroup, MailEntity> group,
        MailTokenProcessor tokenProcessor)
    {
        var mailBody = ProcessBody(group, tokenProcessor);

        var mergedMail = new MailEntity
        {
            To = group.Key.To,
            Subject = group.Key.Group,
            Body = mailBody,
            Status = NotificationStatus.ReadyToSend
        };

        await _mailRepository.CreateAsync(mergedMail);

        var mailIds = group.Select(m => m.Id).ToList();
        return mailIds;
    }

    private async Task MarkAsProcessed(List<string> mailIds)
    {
        foreach (var mailId in mailIds)
        {
            var mail = await _mailRepository.GetAsync(mailId);
            mail.Status = NotificationStatus.Processed;
            await _mailRepository.UpdateAsync(mail);
        }
    }

    private string ProcessBody(IGrouping<MailGroup, MailEntity> group, MailTokenProcessor tokenProcessor)
    {
        var mergedBody = string.Join(GroupedMailTemplates.BodySeparator, group.Select(m => m.Body));
        var mailTemplate = GroupedMailTemplates.GetMailTemplate(group.Key.Group);

        var mailBody = mailTemplate != null ? mailTemplate.Body : GroupedMailTemplates.BodyTag;
        var tokens = new List<MailToken>
        {
            new(GroupedMailTemplates.BodyTag, mergedBody)
        };
        mailBody = tokenProcessor.Process(mailBody, tokens);

        return mailBody;
    }
}
