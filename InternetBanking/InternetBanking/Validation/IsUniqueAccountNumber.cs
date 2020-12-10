using InternetBanking.Dto;
using System;
using System.Collections.Generic;

namespace InternetBanking.Validation
{
    public sealed class IsUniqueAccountNumber<T> : BaseValidationRule<T>
    {
        private List<BankAccountDto> ListOfLinkedAccounts { get; set; }

        public IsUniqueAccountNumber(List<BankAccountDto> listOfLinkedAccounts)
        {
            ListOfLinkedAccounts = listOfLinkedAccounts;
        }

        public override bool Check(T value)
        {
            try
            {
                return !ListOfLinkedAccounts.Exists(
                    x => x.AccountNumber == value as string);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}