using InternetBanking.Dto;
using System.Collections.Generic;

namespace InternetBanking.ViewModels.Components
{
    public class TransactionSection : List<TransactionDto>
    {
        public string Heading { get; set; }

        public TransactionSection(List<TransactionDto> list)
        {
            AddRange(list);
        }
    }
}
