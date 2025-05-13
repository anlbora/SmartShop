using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.Drawing;
using SkiaSharp;

namespace SmartShop.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 20, Bottom = 20 }
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return _converter.Convert(doc);
        }
        public string GenerateChartAsBase64(string label)
        {
            using var bitmap = new SKBitmap(600, 300);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.DarkSlateBlue,
                TextSize = 36,
                IsAntialias = true
            };

            canvas.DrawText(label, 50, 150, paint);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return Convert.ToBase64String(data.ToArray());
        }


        public string SaveChartToTempFile(string label)
        {
            var bitmap = new SKBitmap(600, 300);
            var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);
            canvas.DrawText(label, 20, 50, new SKPaint { Color = SKColors.Blue, TextSize = 24 });
            var image = SKImage.FromBitmap(bitmap);
            var data = image.Encode(SKEncodedImageFormat.Png, 100);

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine("wwwroot/temp", fileName);
            using var fs = File.OpenWrite(filePath);
            data.SaveTo(fs);

            return "/temp/" + fileName; // public path
        }

    }
}
