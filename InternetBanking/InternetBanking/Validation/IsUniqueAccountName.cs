using InternetBanking.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBanking.Validation
{
    public sealed class IsUniqueAccountName<T> : BaseValidationRule<T>
    {
        private List<BankAccountDto> ListOfLinkedAccounts { get; set; }

        public IsUniqueAccountName(List<BankAccountDto> listOfLinkedAccounts)
        {
            ListOfLinkedAccounts = listOfLinkedAccounts;
        }

        public override bool Check(T value)
        {
            try
            {
                return !ListOfLinkedAccounts.Any(x => x.Name == value.ToString());
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}