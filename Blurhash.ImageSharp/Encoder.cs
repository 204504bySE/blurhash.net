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
                var pixelsY = result.LineSpan(y);
                var pixelsYVector = result.LineSpanVector(y);
                var rgbSpan = sourceBitmap.GetPixelRowSpan(y);
                var rgbValues = MemoryMarshal.AsBytes(rgbSpan);
                var rgbVector = MemoryMarshal.Cast<Rgb24, Vector<byte>>(rgbSpan);
                for (int i = 0; i < rgbVector.Length; i++)
                {
                    Vector.Widen(rgbVector[i], out var ushort0, out var ushort1);
                    Vector.Widen(ushort0, out var uint0, out var uint1);
                    Vector.Widen(ushort1, out var uint2, out var uint3);
                    int pixelsYBase = i << 2;
                    pixelsYVector[pixelsYBase] = Vector.ConvertToSingle(uint0);
                    pixelsYVector[pixelsYBase + 1] = Vector.ConvertToSingle(uint1);
                    pixelsYVector[pixelsYBase + 2] = Vector.ConvertToSingle(uint2);
                    pixelsYVector[pixelsYBase + 3] = Vector.ConvertToSingle(uint3);
                }

                for (var i = rgbVector.Length * Vector<byte>.Count; i < width * 3; i++)
                {
                    pixelsY[i] = rgbValues[i];
                }
            }
            result.ChangeFromSrgbToLinear();
            return result;
        }
    }
}