using mbt.webapi.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace mbt.webapi.Configuration;

public static class QuartzConfiguration
{
    public static void AddQuartsConfiguration(this WebApplicationBuilder builder)
    {
        // get intervals from configuration or use default
        var mailProcessorInterval = builder.Configuration.GetValue("Quartz:MailProcessorInterval", 120);
        var emailSenderInterval = builder.Configuration.GetValue("Quartz:EmailSenderInterval", 900);

        builder.Services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";
            q.SchedulerName = "Scheduler";

            var mailProcessorJobKey = new JobKey("MailProcessorJob");
            q.AddJob<MailProcessorJob>(j => j
                .StoreDurably()
                .WithIdentity(mailProcessorJobKey)
                .WithDescription("Mail processor job")
            );

            q.AddTrigger(t => t
                .ForJob(mailProcessorJobKey)
                .WithIdentity(mailProcessorJobKey.Name + "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(mailProcessorInterval)
                    .RepeatForever())
            );

            var emailSenderJobKey = new JobKey("EmailSenderJob");
            q.AddJob<EmailSenderJob>(j => j
                .StoreDurably()
                .WithIdentity(emailSenderJobKey)
                .WithDescription("Mail job")
            );

            q.AddTrigger(t => t
                .ForJob(emailSenderJobKey)
                .WithIdentity(emailSenderJobKey.Name + "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(emailSenderInterval)
                    .RepeatForever())
            );
        });

        builder.Services.AddQuartzHostedService(
            q => q.WaitForJobsToComplete = true);
    }
}
