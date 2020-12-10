using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace InternetBanking.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        public bool IsAuthenticated => _settingsService.IsSignedIn;

        public AuthenticationService(IAbacusApiService abacusApiService, ISettingsService settingsService)
        {
            _abacusApiService = abacusApiService;
            _settingsService = settingsService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginAttemptDto dto)
        {
            using (var response = await _abacusApiService.PostAsync("login", JsonConvert.SerializeObject(dto)))
            {
                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.PreconditionFailed)
                {
                    return new LoginResponseDto
                    {
                        StatusCode = response.StatusCode
                    };
                }
                
                var authorization = JsonConvert.DeserializeObject<LoginResponseDto>(
                    await response.Content.ReadAsStringAsync());

                authorization.StatusCode = response.StatusCode;

                if (authorization == null || authorization.CustomerId == 0)
                {
                    return authorization;
                }

                _settingsService.IsSignedIn = true;
                _settingsService.IsFirstRun = false;
                _settingsService.RememberPin = true;

                _settingsService.CustomerId = (int)authorization.CustomerId;
                _settingsService.PersonId = (int)authorization.PersonId;
                _settingsService.Name = authorization.Name;
                _settingsService.Email = authorization.Email;
                _settingsService.Mobile = authorization.Phone;
                _settingsService.DateOfBirth = authorization.DateOfBirth;

                try
                {
                    _settingsService.MemberNumber = Convert.ToInt32(dto.UserName);
                }
                catch
                {
                    _settingsService.MemberNumber = 0;
                }

                return authorization;
            }
        }

        public Task LogoutAsync()
        {
            _settingsService.IsSignedIn = false;

            _settingsService.MemberNumber = 0;
            _settingsService.CustomerId = 0;
            _settingsService.PersonId = 0;
            _settingsService.Name = string.Empty;
            _settingsService.Email = string.Empty;
            _settingsService.Mobile = string.Empty;
            _settingsService.TwoFactorCode = string.Empty;
            _settingsService.DateOfBirth = null;

            return Task.FromResult(true);
        }

        public async Task<HttpStatusCode> RegisterAsync(RegisterDto dto)
        {
            var response = await _abacusApiService.PostAsync("register/mobile", JsonConvert.SerializeObject(dto));

            return response.StatusCode;
        }

        public async Task<bool> ForgotPinAsync(ForgotPinRequestDto dto)
        {
            var response = await _abacusApiService.PostAsync("forgot", JsonConvert.SerializeObject(dto));

            return response.IsSuccessStatusCode;
        }

        public async Task<HttpStatusCode> ActivateAsync(ActivationDto dto)
        {
            var response = await _abacusApiService.PostAsync("activate", JsonConvert.SerializeObject(dto));

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendAuthenticationSmsAsync(long customerId, string phone)
        {
            var dto = new SmsAuthenticationSendRequestDto()
            {
                CustomerId = customerId,
                MobileNumber = phone
            };

            var response = await _abacusApiService.PostAsync("tfauth/verification", JsonConvert.SerializeObject(dto));

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> ResendAuthenticationSmsAsync(long customerId)
        {
            var dto = new SmsAuthenticationResendRequestDto()
            {
                CustomerId = customerId,
                Identifier = KnownId.TwoFactorCodeTypeAddTFAId
            };
            
            var response = await _abacusApiService.PostAsync("tfauth/resendverification", JsonConvert.SerializeObject(dto));

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> AuthenticateSmsAsync(LoginResponseDto login, string memberNumber, string code)
        {
            if (code == string.Empty)
                return HttpStatusCode.BadRequest;

            var dto = new SmsAuthenticationValidateRequestDto()
            {
                CustomerId = login.CustomerId,
                PersonId = login.PersonId,
                CustomerNumber = memberNumber,
                TwoFactorCode = code
            };
            
            var response = await _abacusApiService.PostAsync("tfauth/check", JsonConvert.SerializeObject(dto));

            return response.StatusCode;
        }
    }
}
