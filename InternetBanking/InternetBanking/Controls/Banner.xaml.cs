using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace InternetBanking.Controls
{
    public partial class Banner : RelativeLayout
    {
        public Banner()
        {
            InitializeComponent();
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs eventArgs)
        {
            var primaryColor = Color.FromHex(App.Skin.PrimaryColor).ToSKColor();

            var imageInfo = eventArgs.Info;
            var canvas = eventArgs.Surface.Canvas;

            var rectangle = new SKRect
            {
                Size = new SKSize(imageInfo.Width * 2, (float)(imageInfo.Height * 3.8))
            };

            rectangle.Location = new SKPoint((rectangle.Size.Width / 2 * -1) + (imageInfo.Width / 2),
                rectangle.Size.Height * -1 + imageInfo.Height);


            canvas.Clear();

            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = primaryColor,
                IsAntialias = true
            })
            {
                canvas.DrawOval(rectangle, paint);
            }
        }
    }
}
