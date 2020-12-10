namespace InternetBanking.Dto
{
    public class WithdrawalDto
    {
        public long CustomerId { get; set; }
        public long SavingsAccountId { get; set; }
        public long BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
