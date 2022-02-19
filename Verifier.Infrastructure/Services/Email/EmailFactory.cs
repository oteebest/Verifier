using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Application.Interfaces.Services;

namespace Verifier.Infrastructure.Services.Email
{
    public class EmailFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;

        public EmailFactory(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }

        public IMailService GetEmailService()
        {

            if (_config.GetSection("EmailProvider").Value.Equals("SendGrid", StringComparison.OrdinalIgnoreCase))
            {
                return (IMailService)_serviceProvider.GetService(typeof(SendGridMailService));
            }
            else
            {
                return (IMailService)_serviceProvider.GetService(typeof(SendGridMailService));

            }

        }
    }
}
