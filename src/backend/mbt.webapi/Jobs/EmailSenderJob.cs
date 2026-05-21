using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Configuration;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services;
using Quartz;

namespace mbt.webapi.Jobs;

public class EmailSenderJob : IJob
{
    private readonly IDbBaseRepository<MailEntity> _mailRepository;
    private readonly MailSender _mailSender;

    public EmailSenderJob(IDbBaseRepository<MailEntity> mailRepository, ISmtpSettings smtpSettings)
    {
        _mailRepository = mailRepository;
        _mailSender = new MailSender(smtpSettings);
    }


    public async Task Execute(IJobExecutionContext context)
    {
        var mails = await _mailRepository.FindAsync(m => m.Status == NotificationStatus.ReadyToSend);

        if (mails.Count != 0)
        {
            await _mailSender.SendAsync(mails, _mailRepository);
        }
    }
}
