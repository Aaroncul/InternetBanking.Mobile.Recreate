using InternetBanking.Models;
using System.Collections.Generic;

namespace InternetBanking.Services.Skins
{
    public interface ISkinService
    {
        Skin GetSkin(int id);

        Skin GetSkin(string name);

        ICollection<Skin> GetSkins();
    }
}
