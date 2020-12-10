using System;

namespace InternetBanking.Dto
{
    public class BankAccountAddDto
    {
        public long CustomerId { get; set; }

        public long PersonId { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string SortCode { get; set; }

        public DateTime? DateClosed { get; set; }

        public string TwoFactorCode { get; set; }
        public long Id { get; set; }
    }
}
