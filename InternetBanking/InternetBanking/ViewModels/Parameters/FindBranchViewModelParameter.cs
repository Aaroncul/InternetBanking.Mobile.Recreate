using InternetBanking.Models;

namespace InternetBanking.ViewModels.Parameters
{
    public class FindBranchViewModelParameter
    {
        public FindBranchViewModelParameter(Branch selectedBranch)
        {
            SelectedBranch = selectedBranch;
        }

        public Branch SelectedBranch { get; set; }
    }
}
