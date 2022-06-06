using Client.Models;
using Client.ViewModels;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client.Services
{
    internal static class ImageConverter 
    {
        public static string ConvertTo64Base(BitmapImage img)
        {
            using (var stream = img.StreamSource)
            {
                byte[] buffer;
                using (var binReader = new BinaryReader(stream))
                {
                    buffer = binReader.ReadBytes((int)stream.Length);
                }
                return System.Convert.ToBase64String(buffer);
            }
        }

        public static BitmapImage ConvertFrom64Base(string str)
        {
            byte[] buffer = System.Convert.FromBase64String(str);
            if (buffer == null || buffer.Length == 0) return null;

            var bmp = new BitmapImage();
            using (var ms = new MemoryStream(buffer))
            {
                bmp.BeginInit();
                bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            //bmp.Freeze();
            return bmp;
        }

        public static CardPresentation ToCardPresentation(CardDto card)=>
            new CardPresentation { Id = card.Id, Name = card.Name, Image = ConvertFrom64Base(card.Base64Image) };

        public static CardDto ToCardDto(CardPresentation card) =>
             new CardDto{ Id = card.Id, Name = card.Name, Base64Image = ConvertTo64Base(card.Image)};
    }
}
