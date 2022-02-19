using System.Collections.Generic;
using Verifier.Shared.Models.Request.Email;

namespace Verifier.Application.Interfaces.Services
{
    public interface IEmailService
    {
        void SendEmail(MailRequest request, string template, Dictionary<string, string> dynamicData);
    }
}
