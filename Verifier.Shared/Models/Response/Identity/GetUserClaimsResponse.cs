using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Shared.Models.Response;
using Verifier.Shared.Models.Response.Identity;

namespace Core.Models.Response.Identity
{
    public class GetUserClaimsResponse : ResponseBase
    {
        public IEnumerable<UserClaimResponse> List { get; set; }
    }
}
