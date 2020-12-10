using System;

namespace InternetBanking.Dto
{
    public class AccountDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public long ProductId { get; set; }
        public long ProductTypeId { get; set; }
        public long CustomerId { get; set; }
        public DateTime BalanceDate { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public decimal UnclearedAmount { get; set; }
        public decimal MinimumShareBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BalanceLocked { get; set; }
        public decimal? LockedPercentage { get; set; }
        public bool? LockedAgainstNetShares { get; set; }
        public long LockedLoanProductId { get; set; }
        public decimal OverdraftLimit { get; set; }
    }
}
