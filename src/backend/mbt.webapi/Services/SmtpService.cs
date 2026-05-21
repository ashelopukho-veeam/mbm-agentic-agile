using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using mbt.webapi.Configuration;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MimeKit;
using MimeKit.Text;
using NLog;
using MailboxAddress = MimeKit.MailboxAddress;

namespace mbt.webapi.Services;

public class MailSender
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ISmtpSettings _smtpSettings;
    private const int ChunkSize = 10; // max 10 mails per connection


    public MailSender(ISmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }


    public void Send(IEnumerable<string> to, string subject, string body)
    {
        try
        {
            using var smtpClient = new SmtpClient();
            var mailMessage = new MimeMessage();

            var sendFrom = _smtpSettings.From;

            mailMessage.From.Add(new MailboxAddress(sendFrom, sendFrom));
            mailMessage.To.AddRange(to.Select(t => new MailboxAddress(t, t)));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };
            smtpClient.Connect(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.None);
            smtpClient.Send(mailMessage);
            smtpClient.Disconnect(true);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public async Task SendAsync(IEnumerable<MailEntity> mails, IDbBaseRepository<MailEntity> mailRepository)
    {
        try
        {
            var mailList = mails.ToList();

            for (var i = 0; i < mailList.Count; i += ChunkSize)
            {
                var chunk = mailList.Skip(i).Take(ChunkSize);

                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.None);

                foreach (var mail in chunk)
                {
                    try
                    {
                        var mailMessage = new MimeMessage();

                        var sendFrom = _smtpSettings.From;

                        mailMessage.From.Add(new MailboxAddress(sendFrom, sendFrom));
                        mailMessage.To.Add(new MailboxAddress(mail.To, mail.To));
                        mailMessage.Subject = mail.Subject;
                        mailMessage.Body = new TextPart(TextFormat.Html) { Text = mail.Body };

                        await smtpClient.SendAsync(mailMessage);

                        mail.Status = NotificationStatus.Sent;
                        await mailRepository.UpdateAsync(mail);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error sending mail: {0}, {1}, {2}", mail.To, mail.Subject, mail.Body);
                        Logger.Error(ex);

                        mail.Status = NotificationStatus.Failed;
                        await mailRepository.UpdateAsync(mail);
                    }
                }

                await smtpClient.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}
