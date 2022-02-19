using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Verifier.Domain.Contracts;

namespace Verifier.Domain.Entities.User
{
    public class VerifierUser : IdentityUser, IAuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsNamePublic { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpiryTime { get; set; }
        public string LoginScheme { get; set; } = "Default";
        public string LoginSchemeId { get; set; }
    }
}
