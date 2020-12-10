namespace InternetBanking.Dto
{
    public class RegisterDto
    {
        public string Forename = "default";
        public string Surname = "default";
        public string Email = "default";

        public string CustomerNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
    }
}
