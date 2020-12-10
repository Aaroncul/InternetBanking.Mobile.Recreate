using System.Collections.Generic;

namespace InternetBanking.ViewModels.Components
{
    public class ListViewSection : List<ListViewSectionItem>
    {
        public string Heading { get; set; }

        public ListViewSection(List<ListViewSectionItem> list)
        {
            AddRange(list);
        }
    }
}
