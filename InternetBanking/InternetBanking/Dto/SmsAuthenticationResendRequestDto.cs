namespace InternetBanking.Dto
{
    class SmsAuthenticationResendRequestDto
    {
        public long CustomerId { get; set; }
        public string Identifier { get; set; }
    }
}
