using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WpfApp1
{
    public class CaptchaGenerator
    {
        private static readonly Random random = new Random();
        private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int Width = 200;
        private const int Height = 80;

        public string CurrentCaptchaText { get; private set; }
        public BitmapSource CaptchaImage { get; private set; }

        public void GenerateNewCaptcha()
        {
            // Генерация случайного текста
            CurrentCaptchaText = GenerateRandomText(6);

            // Создание изображения капчи
            CaptchaImage = CreateCaptchaImage(CurrentCaptchaText);
        }

        private string GenerateRandomText(int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(Chars[random.Next(Chars.Length)]);
            }
            return sb.ToString();
        }

        private BitmapSource CreateCaptchaImage(string text)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Фон
                dc.DrawRectangle(Brushes.White, null, new System.Windows.Rect(0, 0, Width, Height));

                // Текст с искажениями
                for (int i = 0; i < text.Length; i++)
                {
                    var tf = new Typeface("Arial");
                    var fontSize = random.Next(20, 30);
                    var ft = new FormattedText(
                        text[i].ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        tf,
                        fontSize,
                        Brushes.Black,
                        96);

                    // Случайное положение символов
                    double x = 10 + i * 30 + random.Next(-5, 5);
                    double y = random.Next(10, Height - 30);

                    // Поворот символов
                    var rotateTransform = new RotateTransform(random.Next(-15, 15));
                    dc.PushTransform(rotateTransform);
                    dc.DrawText(ft, new System.Windows.Point(x, y));
                    dc.Pop();
                }

                // Добавляем шум (линии)
                for (int i = 0; i < 5; i++)
                {
                    dc.DrawLine(
                        new Pen(Brushes.Gray, 1),
                        new System.Windows.Point(random.Next(Width), random.Next(Height)),
                        new System.Windows.Point(random.Next(Width), random.Next(Height)));
                }

                // Добавляем эффект размытия
                visual.Effect = new BlurEffect { Radius = 1.5 };
            }

            // Конвертируем DrawingVisual в BitmapSource
            var rtb = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            return rtb;
        }
    }
}
