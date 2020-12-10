using InternetBanking.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InternetBanking.Services.Skins
{
    public class SkinService : ISkinService
    {
        public Skin GetSkin(int id)
        {
            return GetSkins().FirstOrDefault(x => x.Id == id);
        }

        public Skin GetSkin(string name)
        {
            return GetSkins().FirstOrDefault(x => x.Name == name);
        }

        public ICollection<Skin> GetSkins()
        {
            var assembly = typeof(SkinService).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("InternetBanking.Skins.json");

            string content = "";
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert
                .DeserializeObject<List<Skin>>(content)
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
