using System;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Blurhash.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Blurhash.ImageSharp
{
    public class Encoder : CoreEncoder
    {
        public Encoder(int width, int height, int maxComponentsX, int maxComponentsY) : base(width, height, maxComponentsX, maxComponentsY, false)
        { }

        /// <summary>
        /// Encodes a picture into a Blurhash string
        /// </summary>
        /// <param name="image">The picture to encode</param>
        /// <param name="componentsX">The number of components used on the X-Axis for the DCT</param>
        /// <param name="componentsY">The number of components used on the Y-Axis for the DCT</param>
        /// <returns>The resulting Blurhash string</returns>
        public string Encode(Image<Rgb24> image, int componentsX, int componentsY)
        {
            return CoreEncode(ConvertBitmap(image), componentsX, componentsY);
        }

        /// <summary>
        /// Converts the given bitmap to the library-independent representation used within the Blurhash-core
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to encode</param>

        public static PixelVector ConvertBitmap(Image<Rgb24> sourceBitmap)
        {
            var width = sourceBitmap.Width;
            var height = sourceBitmap.Height;
            var bytesPerPixel = sourceBitmap.PixelType.BitsPerPixel / 8;
            var stride = width * 3;

            var result = new PixelVector(width, height);

            for (int y = 0; y < height; y++)
            {
                var rgbValues = MemoryMarshal.AsBytes(sourceBitmap.GetPixelRowSpan(y));
                var pixelsY = result.Pixels[y];
                for (var i = 0; i < width * 3; i++)
                {
                    pixelsY[i] = rgbValues[i];
                }
            }
            result.ChangeFromSrgbToLinear();
            return result;
        }
    }
}