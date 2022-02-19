using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verifier.Shared.Models.ServiceModel.Account
{
    public class ResetPasswordInputServiceModel
    {
        public string EncryptedEmail { get; set; }

        public string Password { get; set; }
    }
}
