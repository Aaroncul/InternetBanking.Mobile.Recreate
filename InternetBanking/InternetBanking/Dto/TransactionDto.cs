using System;

namespace InternetBanking.Dto
{
    public class TransactionDto
    {
        public long Id { get; set; }
        public DateTime ValueDate { get; set; }
        public long ReceiptNumber { get; set; }
        public string Description { get; set; }
        public bool Credit { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
