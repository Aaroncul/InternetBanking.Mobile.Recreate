namespace InternetBanking.ViewModels.Components
{
    public class AwesomeIcon
    {
        public string FontPackage { get; set; }
        public string Code { get; set; }

        public AwesomeIcon(string code, string fontPackage)
        {
            Code = code;
            FontPackage = fontPackage;
        }

        public static class Packages
        {
            public static string Solid = "FontAwesomeSolid";
            public static string Regular = "FontAwesomeRegular";
            public static string Brands = "FontAwesomeBrands";
        }
    }
}
