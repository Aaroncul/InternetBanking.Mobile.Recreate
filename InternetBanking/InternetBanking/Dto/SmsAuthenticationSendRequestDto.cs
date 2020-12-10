namespace InternetBanking.Dto
{
    public class SmsAuthenticationSendRequestDto
    {
        public long CustomerId { get; set; }
        public string MobileNumber { get; set; }
    }
}

