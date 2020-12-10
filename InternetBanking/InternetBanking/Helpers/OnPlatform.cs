namespace InternetBanking.Helpers
{
    public sealed class OnPlatform<T>
    {
        public T Android { get; set; }
        public T iOS { get; set; }
        public T Other { get; set; }

        public OnPlatform()
        {
            Android = default;
            iOS = default;
        }

        public static implicit operator T(OnPlatform<T> platform)
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.Android:
                    return platform.Android;

                case Xamarin.Forms.Device.iOS:
                    return platform.iOS;

                default:
                    return platform.Other;
            }
        }
    }
}