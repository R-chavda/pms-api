namespace Domain.Interfaces
{
    public interface IBaseEntity
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
    }
}
