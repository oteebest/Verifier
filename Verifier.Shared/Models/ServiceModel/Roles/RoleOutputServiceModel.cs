using System.ComponentModel.DataAnnotations;

namespace Verifier.Shared.Models.ServiceModel.Roles
{
    public class RoleOutputServiceModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}