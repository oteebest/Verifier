using Hangfire;
using System.Collections.Generic;
using Verifier.Application.Interfaces.Services;
using Verifier.Shared.Models.Request.Email;

namespace Verifier.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailFactory _emailFactory;

        public EmailService(EmailFactory emailFactory)
        {
            _emailFactory = emailFactory;
        }

        public void SendEmail(MailRequest request, string template, Dictionary<string, string> dynamicData)
        {
            var _mailService = _emailFactory.GetEmailService();

            BackgroundJob.Enqueue(() => _mailService.SendTemplateAsync(request, template, dynamicData));
        }
    }
}
