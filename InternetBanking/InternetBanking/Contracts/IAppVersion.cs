namespace InternetBanking.Contracts
{
    public interface IAppVersion
    {
        string GetVersion();

        long GetBuild();
    }
}
