using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Configuration;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class MailService : IMailService
{
    private readonly IDbBaseRepository<MailEntity> _mailRepository;

    public MailService(IDbBaseRepository<MailEntity> mailRepository, ISmtpSettings smtpSettings)
    {
        _mailRepository = mailRepository;
    }

    public async Task QueueAsync(MailEntity mail)
    {
        await _mailRepository.CreateAsync(mail);
    }

    public Task QueueAsync(List<MailEntity> mails)
    {
        return _mailRepository.CreateManyAsync(mails);
    }

    public async Task QueueAsync(string to, string subject, string body)
    {
        var mail = new MailEntity
        {
            To = to,
            Subject = subject,
            Body = body
        };

        await QueueAsync(mail);
    }

    public async Task QueueAsync(IEnumerable<string> to, string subject, string body)
    {
        foreach (var sendTo in to.Distinct())
        {
            await QueueAsync(sendTo, subject, body);
        }
    }
}
