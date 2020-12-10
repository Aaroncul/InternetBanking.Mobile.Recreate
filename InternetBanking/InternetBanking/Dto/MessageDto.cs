using System;

namespace InternetBanking.Dto
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long MesageThreadId { get; set; }
        public string Content { get; set; }
        public bool? FromCustomer { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
    }
}
