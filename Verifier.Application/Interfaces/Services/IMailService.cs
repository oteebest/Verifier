using System.Collections.Generic;
using System.Threading.Tasks;
using Verifier.Shared.Models.Request.Email;

namespace Verifier.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailRequest request);
        Task<bool> SendTemplateAsync(MailRequest request, string templateId, Dictionary<string,string> dynamicData);
    }
}