namespace Verifier.Domain.Contracts
{
    public interface ICollabEntity : IAuditableEntity<string>
    {
        public string ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
    }
}
