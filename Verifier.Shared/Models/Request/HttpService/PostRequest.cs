using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Shared.Enums;

namespace Verifier.Shared.Models.Request.HttpService
{
    public class PostRequest
    {
        public List<KeyValuePair<string, string>> ContentParmeters { get; set; } = new List<KeyValuePair<string, string>>();
        public HttpContentType HttpContentType { get; set; }
        public HttpServiceAuthenticationType HttpServiceAuthenticationType { get; set; }
        public string BasicAuthenticationUserName{get; set;}
        public string BasicAuthenticationPassword { get; set; }
        public string Token { get; set; }
    }
}
