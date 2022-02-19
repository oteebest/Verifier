namespace Verifier.Application.Configurations
{
    public class IdentityConfiguration
    {
        public string Secret { get; set; }
        public int TokenLifeInMins { get; set; }

    }
}