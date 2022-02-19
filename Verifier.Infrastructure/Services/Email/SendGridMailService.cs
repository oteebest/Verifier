using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verifier.Application.Configurations;
using Verifier.Application.Interfaces.Services;
using Verifier.Shared.Models.Request.Email;

namespace Verifier.Infrastructure.Services.Email
{
    public class SendGridMailService : IMailService
    {
        private readonly SendGridConfiguration _config;

        public SendGridMailService(IOptions<SendGridConfiguration> config)
        {
            _config = config.Value;
        }

        public async Task<bool> SendAsync(MailRequest request)
        {
            var apiKey = _config.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config.From, _config.FromName);
            var subject = request.Subject;
            var to = new EmailAddress(request.To);
            var plainTextContent = "";
            var htmlContent = request.Body;
            var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(message);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendTemplateAsync(MailRequest request, string templateId, Dictionary<string, string> dynamicData)
        {
            var apiKey = _config.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config.From, _config.FromName);
            var to = new EmailAddress(request.To);

            var sendGridMessage = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicData);

            var response = await client.SendEmailAsync(sendGridMessage);

            return response.IsSuccessStatusCode;
        }
    }
}