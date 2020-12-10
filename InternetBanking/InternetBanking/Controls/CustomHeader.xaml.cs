using Xamarin.Forms;

namespace InternetBanking.Controls
{
    public partial class CustomHeader : Label
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
            propertyName: "Title",
            returnType: typeof(string),
            declaringType: typeof(CustomHeader),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty MessageProperty =
          BindableProperty.Create(
          propertyName: "Message",
          returnType: typeof(string),
          declaringType: typeof(CustomHeader),
          defaultValue: "",
          defaultBindingMode: BindingMode.TwoWay,
          propertyChanged: MessagePropertyChanged);

        private static void MessagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomHeader)bindable;
            control.message.Text = newValue.ToString();
        }

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }

            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public CustomHeader()
        {
            InitializeComponent();
        }
    }
}
