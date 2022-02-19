
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Verifier.Domain.Contracts;

namespace Domain.Identity
{ public class VerifierRole : IdentityRole, IAuditableEntity
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public virtual ICollection<VerifierRoleClaim> RoleClaims { get; set; }

        public VerifierRole() : base()
        {
            RoleClaims = new HashSet<VerifierRoleClaim>();
        }

        public VerifierRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<VerifierRoleClaim>();
            Description = roleDescription;
        }
    }
}
