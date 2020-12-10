namespace InternetBanking.Dto
{
    public class SmsAuthenticationValidateRequestDto
    {
        public long CustomerId { get; set; }
        public long PersonId { get; set; }
        public string CustomerNumber { get; set; }
        public string TwoFactorCode { get; set; }
    }
}

