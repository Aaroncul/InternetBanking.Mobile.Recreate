using System;

namespace InternetBanking.Database.Entities
{
    public class MessageThread : BaseEntity
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public int MessageTypeId { get; set; }
        public bool Closed { get; set; }
        public DateTime? ClosedDate { get; set; }
    }
}
