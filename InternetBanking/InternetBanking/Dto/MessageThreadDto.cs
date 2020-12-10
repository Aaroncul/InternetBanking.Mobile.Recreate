using System;

namespace InternetBanking.Dto
{
    public class MessageThreadDto
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }
        public bool? Closed { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int MessageTypeId { get; set; }
        public string Subject { get; set; }
    }
}

