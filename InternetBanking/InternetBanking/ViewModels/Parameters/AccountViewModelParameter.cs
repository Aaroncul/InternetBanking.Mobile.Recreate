using InternetBanking.Dto;

namespace InternetBanking.ViewModels.Parameters
{
    public class AccountViewModelParameter
    {
        public AccountViewModelParameter(AccountDto account)
        {
            Account = account;
        }

        public AccountDto Account { get; set; }
    }
}
