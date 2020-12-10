using System;

namespace InternetBanking.Dto
{
    public class BankAccountDto
    {
        public long Id { get; set; }
        public long BankAccountId
        {
            get => Id;
        }

        public long CustomerId { get; set; }

        public long PersonId { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string SortCode { get; set; }

        public string TwoFactorCode { get; set; }
        public DateTime? DateClosed { get; set; }

        public string Status
        {
            get => !DateClosed.HasValue ? "Active" : "Inactive";
        }

    }

}
