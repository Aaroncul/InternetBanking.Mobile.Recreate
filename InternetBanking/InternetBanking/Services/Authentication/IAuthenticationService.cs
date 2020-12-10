using InternetBanking.Dto;
using System.Net;
using System.Threading.Tasks;

namespace InternetBanking.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }

        Task<LoginResponseDto> LoginAsync(LoginAttemptDto dto);

        Task LogoutAsync();

        Task<HttpStatusCode> RegisterAsync(RegisterDto dto);

        Task<bool> ForgotPinAsync(ForgotPinRequestDto forgotPinRequestDto);

        Task<HttpStatusCode> ActivateAsync(ActivationDto activationDto);

        Task<HttpStatusCode> SendAuthenticationSmsAsync(long customerId, string phone);
        Task<HttpStatusCode> ResendAuthenticationSmsAsync(long customerId);
        Task<HttpStatusCode> AuthenticateSmsAsync(LoginResponseDto login, string memberNumber, string code);
    }
}
