using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Shared.Models.Request.User;

namespace Verifier.Shared.Models.ServiceModel.Account
{
    public class ChangePasswordInputServiceModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
