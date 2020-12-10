using System.Text;
using System.Text.RegularExpressions;

namespace InternetBanking.Models
{
    public class Branch
    {
        public string BranchName { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string iOSMapsLink { get; set; }
        public bool IsPrimary { get; set; }

        public string FormattedAddress()
        {
            var stringBuilder = new StringBuilder(Line1);

            if (!string.IsNullOrEmpty(Line2))
            {
                stringBuilder.Append($", {Line2}");
            }

            if (!string.IsNullOrEmpty(City))
            {
                stringBuilder.Append($", {City}");
            }

            if (!string.IsNullOrEmpty(Region))
            {
                stringBuilder.Append($", {City}");
            }

            if (!string.IsNullOrEmpty(PostalCode))
            {
                stringBuilder.Append($", {PostalCode}");
            }

            return stringBuilder.ToString();
        }

        public bool Like(string toFind)
        {
            toFind = $"%{toFind}%";

            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
                .Replace(toFind, ch => @"\" + ch)
                .Replace('_', '.')
                .Replace("%", ".*") + @"\z", RegexOptions.Singleline)
                .IsMatch(FormattedAddress());
        }
    }
}

