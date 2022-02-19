using System.ComponentModel.DataAnnotations.Schema;

namespace Verifier.Domain.Contracts
{
    public interface IChatEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
    }
}
