using System;
using System.Net;

namespace InternetBanking.Dto
{
    public class LoginResponseDto
    {
        public HttpStatusCode StatusCode { get; set; }
        public long CustomerId { get; set; }
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string CorrespondenceName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
