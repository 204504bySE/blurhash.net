using System;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Blurhash.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Blurhash.ImageSharp
{
    public class Encoder : CoreEncoder
    {
        public Encoder() : base(false) { }
        public Encoder(IBasisProviderEncode basisProvider) : base(basisProvider, false) { }

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

            var result = new PixelVector(width, height);

            for (int y = 0; y < height; y++)
            {
                var pixelsY = result.RowSpan(y);
                var rgbSpan = sourceBitmap.GetPixelRowSpan(y);
                var rgbValues = MemoryMarshal.AsBytes(rgbSpan);
                
                for(int i = 0; i < rgbValues.Length; i++)
                {
                    pixelsY[i] = MathUtils.SRgbToLinear(rgbValues[i]);
                }
            }
            return result;
        }
    }
}