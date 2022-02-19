using System.Collections.Generic;
using Verifier.Shared.Models.Response.Identity;

namespace Verifier.Shared.Models.ServiceModel.Roles
{
    public class PermissionOutputServiceModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimInputServiceModel> RoleClaims { get; set; }
    }
}