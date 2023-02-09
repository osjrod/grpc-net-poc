using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections;

namespace ImageResizer
{
    public static class Resizer
    {
        public static byte[] ResizeImage(byte[] originalImage, int width, int height)
        {
            IImageFormat format;
            using var image = Image.Load<Rgba32>(originalImage, out format);
            image.Mutate(x => x.Resize(width, height));
            using (var ms = new MemoryStream())
            {
                image.Save(ms,format);
                return  ms.ToArray();
            }
        }
    }
}