using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verifier.Shared.Models.ServiceModel.Token
{
    public class TokenOutputServiceModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public int TokenExpiresIn { get; set; }
    }
}
