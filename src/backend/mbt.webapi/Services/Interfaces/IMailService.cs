using System.Collections.Generic;
using System.Threading.Tasks;

namespace mbt.webapi.Services.Interfaces;

public interface IMailService : IBaseService
{
    Task QueueAsync(string to, string subject, string body);

    Task QueueAsync(IEnumerable<string> to, string subject, string body);
}
