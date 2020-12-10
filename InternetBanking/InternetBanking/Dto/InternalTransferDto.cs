namespace InternetBanking.Dto
{
    public class InternalTransferDto
    {
        public long CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
