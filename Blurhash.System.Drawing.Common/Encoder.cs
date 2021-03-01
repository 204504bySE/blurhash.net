using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Blurhash.Core;

// ReSharper disable once CheckNamespace Justification: Meant to extend the System.Drawing.Common-Namespace
namespace System.Drawing.Common.Blurhash
{
    /// <summary>
    /// The Blurhash encoder for use with the <code>System.Drawing.Common</code> package
    /// </summary>
    public class Encoder : CoreEncoder
    {
        public Encoder(int width, int height, int maxComponentsX, int maxComponentsY) : base(width, height, maxComponentsX, maxComponentsY) { }

        /// <summary>
        /// Encodes a picture into a Blurhash string
        /// </summary>
        /// <param name="image">The picture to encode</param>
        /// <param name="componentsX">The number of components used on the X-Axis for the DCT</param>
        /// <param name="componentsY">The number of components used on the Y-Axis for the DCT</param>
        /// <returns>The resulting Blurhash string</returns>
        public string Encode(Image image, int componentsX, int componentsY)
        {
            return CoreEncode(ConvertBitmap(image as Bitmap ?? new Bitmap(image)), componentsX, componentsY);
        }

        /// <summary>
        /// Converts the given bitmap to the library-independent representation used within the Blurhash-core
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to encode</param>
        public static PixelVector ConvertBitmap(Bitmap sourceBitmap)
        {
            var width = sourceBitmap.Width;
            var height = sourceBitmap.Height;
            PixelVector result;

            int stride;
            byte[] rgbValues;
            using (var temporaryBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            {
                using (var graphics = Graphics.FromImage(temporaryBitmap))
                {
                    graphics.DrawImageUnscaled(sourceBitmap, 0, 0);
                }
                // Lock the bitmap's bits.
                var bmpData = temporaryBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, temporaryBitmap.PixelFormat);

                // Get the address of the first line.
                var ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                var bytes = Math.Abs(bmpData.Stride) * height;
                stride = bmpData.Stride;
                rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
                temporaryBitmap.UnlockBits(bmpData);
            }

            result = new PixelVector(width, height);

                for (int y = 0; y < height; y++)
                {
                var baseIndex = 0 <= stride ? stride * y : -stride * (height - y);
                    var pixelsY = result.Pixels[y];
                    for (var i = 0; i < width * 3; i++)
                    {
                        pixelsY[i] = rgbValues[i + baseIndex];
                    }
                }
            result.ChangeFromSrgbToLinear();
            return result;
        }
    }
}
