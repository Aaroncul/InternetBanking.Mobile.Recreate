namespace InternetBanking.Dto
{
    public class ActivationDto
    {
        public string MembershipNumber { get; set; }
        public string SmsActivationCode { get; set; }
        public string EmailActivationCode { get; set; }
        public string Pin1 { get; set; }
        public string Pin2 { get; set; }

        public ActivationDto()
        {
            EmailActivationCode = "";
        }
    }
}