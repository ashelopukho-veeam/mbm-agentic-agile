using System;
using System.Collections.Generic;
using System.Linq;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

public class SendEmailStep : StepBody
{
    private readonly IMailService _mailService;
    private readonly IUserService _userService;
    private readonly IApiService _apiService;

    public SendEmailStep(IMailService mailService, IUserService userService, IApiService apiService)
    {
        _mailService = mailService;
        _userService = userService;
        _apiService = apiService;
    }

    public List<string> To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var toEmails = new List<string>();

        foreach (var t in To)
            if (Guid.TryParse(t, out var id))
            {
                var user = _userService.Get(id.ToString());
                if (!string.IsNullOrWhiteSpace(user.Email)) toEmails.Add(user.Email);
            }
            else
            {
                toEmails.Add(t);
            }

        var bodyWithReplacedTokens = ReplaceTokens(Body);

        _mailService.QueueAsync(toEmails, Subject, bodyWithReplacedTokens);

        return ExecutionResult.Next();
    }

    private string ReplaceTokens(string body)
    {
        var config = _apiService.GetAppConfig();

        return body.Replace(StringTokens.Host, config.ClientHostUrl.Trim('/'));
    }
}
