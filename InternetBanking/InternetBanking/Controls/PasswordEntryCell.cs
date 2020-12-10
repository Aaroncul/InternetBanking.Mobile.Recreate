using Xamarin.Forms;

namespace InternetBanking.Controls
{
    public class PasswordEntryCell : EntryCell
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                MaskStars();
            }
        }

        private void MaskStars()
        {
            Text = Mask(Value.Length);
        }

        public PasswordEntryCell()
        {
            Value = string.Empty;

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Text")
                {
                    if (Text.Length > Value.Length)
                    {
                        Value += Text.Substring(Text.Length - 1);
                    }
                    else
                    {
                        Value = Value.Substring(0, Text.Length);
                    }

                    MaskStars();
                }
            };
        }

        private string Mask(int count)
        {
            var result = "";

            for (var i = 0; i < count; i++)
            {
                result += "●";
            }

            return result;
        }
    }
}

