using System.Collections.Generic;
using System.Linq;

namespace mbt.webapi.Jobs;

public record MailToken(string Key, string Value);

public class MailTokenProcessor
{
    private readonly string _hostUrl;

    public MailTokenProcessor(string hostUrl)
    {
        _hostUrl = hostUrl;
    }

    private List<MailToken> Tokens => new()
    {
        new MailToken(GroupedMailTemplates.BudgetPlanListPageUrlTag, _hostUrl.TrimEnd('/') + "/budgetplans"),
        new MailToken(GroupedMailTemplates.TasksListPageUrlTag, _hostUrl.TrimEnd('/') + "/tasks")
    };


    public string Process(string body, List<MailToken> tokens)
    {
        var newBody = tokens.Aggregate(body,
            (current, token) => current.Replace(token.Key, token.Value));

        return Tokens.Aggregate(newBody,
            (current, token) => current.Replace(token.Key, token.Value));
    }

    public string Process(string body)
    {
        return Process(body, new List<MailToken>());
    }
}
