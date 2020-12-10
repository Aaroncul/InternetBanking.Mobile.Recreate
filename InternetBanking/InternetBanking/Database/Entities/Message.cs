namespace InternetBanking.Database.Entities
{
    public class Message : BaseEntity
    {
        public long Id { get; set; }
        public long MessageThreadId { get; set; }

        public string Content { get; set; }
        public bool FromCustomer { get; set; }
        public int? UserId { get; set; }
    }
}
