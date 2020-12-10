using InternetBanking.Dto;
using System.Collections.Generic;

namespace InternetBanking.ViewModels.Components
{
    public class MessageThreadSection : List<MessageThreadDto>
    {
        public string Heading { get; set; }

        public List<MessageThreadDto> MessageThreads => this;
    }
}
