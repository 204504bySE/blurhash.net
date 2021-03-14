using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blurhash.Core
{
    /// <summary>
    /// The core encoding algorithm of Blurhash.
    /// To be not specific to any graphics manipulation library this algorithm only operates on <c>double</c> values.
    /// </summary>
    public class CoreEncoder
    {
        readonly IBasisProviderEncode BasisProvider;
        readonly bool IsBgrOrder;

        public CoreEncoder(bool isBgrOrder): this(new BasisProviderEncode(), isBgrOrder) { }
        /// <summary>
        /// </summary>
        /// <param name="basisProvider"></param>
        /// <param name="isBgrByteOrder">BGR byte order e.g. GdiPlus.</param>
        public CoreEncoder(IBasisProviderEncode basisProvider, bool isBgrOrder)
        {
            BasisProvider = basisProvider;
            IsBgrOrder = isBgrOrder;
        }

        /// <summary>
        /// Encodes a PixelVector into a Blurhash string
        /// </summary>
        /// <returns>The resulting Blurhash string</returns>
        protected string CoreEncode(PixelVector pixels, int componentsX, int componentsY)
        {
            if (componentsX < 1) throw new ArgumentException("componentsX needs to be at least 1");
            if (componentsY < 1) throw new ArgumentException("componentsY needs to be at least 1");

            var factors = new Pixel[componentsX, componentsY];

            for(int y = 0; y < componentsY; y++)
            {
                for(int x = 0; x < componentsX; x++)
                {
                    factors[x, y] = MultiplyBasisFunction(x, y, pixels);
                }
            }

            var dc = factors[0, 0];
            var acCount = componentsX * componentsY - 1;
            var resultBuilder = new StringBuilder();

            var sizeFlag = (componentsX - 1) + (componentsY - 1) * 9;
            resultBuilder.Append(sizeFlag.EncodeBase83(1));

            float maximumValue;
            if(acCount > 0)
            {
                // Get maximum absolute value of all AC components
                var actualMaximumValue = 0.0;
                for (var y = 0; y < componentsY; y++)
                {
                    for (var x = 0; x < componentsX; x++)
                    {
                        // Ignore DC component
                        if (x == 0 && y == 0) continue;

                        actualMaximumValue = Math.Max(Math.Abs(factors[x,y].Red), actualMaximumValue);
                        actualMaximumValue = Math.Max(Math.Abs(factors[x,y].Green), actualMaximumValue);
                        actualMaximumValue = Math.Max(Math.Abs(factors[x,y].Blue), actualMaximumValue);
                    }
                }

                var quantizedMaximumValue = (int) Math.Max(0.0, Math.Min(82.0, Math.Floor(actualMaximumValue * 166 - 0.5)));
                maximumValue = ((float)quantizedMaximumValue + 1) / 166;
                resultBuilder.Append(quantizedMaximumValue.EncodeBase83(1));
            } else {
                maximumValue = 1;
                resultBuilder.Append(0.EncodeBase83(1));
            }

            resultBuilder.Append(EncodeDc(dc.Red, dc.Green, dc.Blue).EncodeBase83(4));


            for (var y = 0; y < componentsY; y++)
            {
                for (var x = 0; x < componentsX; x++)
                {
                    // Ignore DC component
                    if (x == 0 && y == 0) continue;
                    resultBuilder.Append(EncodeAc(factors[x, y].Red, factors[x, y].Green, factors[x, y].Blue, maximumValue).EncodeBase83(2));
                }
            }

            return resultBuilder.ToString();
        }

        private Pixel MultiplyBasisFunction(int xComponent, int yComponent, PixelVector pixels)
        {
            var componentBasisX = BasisProvider.BasisX(pixels.Width, xComponent);
            var componentBasisY = BasisProvider.BasisY(pixels.Height, yComponent);

            Span<Vector<float>> sumVec =  stackalloc Vector<float>[pixels.SpanLength];
            sumVec.Fill(Vector<float>.Zero);
            var sumArray = MemoryMarshal.Cast<Vector<float>, float>(sumVec);

            var width = pixels.Width;
            var height = pixels.Height;

            //calc DCT and sum results vertically
            for (var y = 0; y < height; y++)
            {
                var currentBasisY = componentBasisY[y];
                var vec = pixels.VectorSpan(y);
                for(int x = 0; x < vec.Length; x++)
                {
                    sumVec[x] += vec[x] * componentBasisX[x] * currentBasisY;
                }
            }

            float a = 0, b = 0, c = 0;
            float normalization = (xComponent == 0 && yComponent == 0) ? 1 : 2;

            //then sum horizontally
            for (int i = 0; i < width * 3; i += 3)
            {
                a += sumArray[i];
                b += sumArray[i + 1];
                c += sumArray[i + 2];
            }

            var scale = normalization / (width * height);
            if (IsBgrOrder) { return new Pixel(c * scale, b * scale, a * scale); }
            else {  return new Pixel(a * scale, b * scale, c * scale); }
        }

        private static int EncodeAc(float  r, float  g, float  b, float  maximumValue) {
            var quantizedR = (int) Math.Max(0, Math.Min(18, Math.Floor(MathUtils.SignPow(r / maximumValue, 0.5f) * 9 + 9.5f)));
            var quantizedG = (int) Math.Max(0, Math.Min(18, Math.Floor(MathUtils.SignPow(g / maximumValue, 0.5f) * 9 + 9.5f)));
            var quanzizedB = (int) Math.Max(0, Math.Min(18, Math.Floor(MathUtils.SignPow(b / maximumValue, 0.5f) * 9 + 9.5f)));

            return quantizedR * (19 * 19) + quantizedG * 19 + quanzizedB;
        }

        private static int EncodeDc(float r, float  g, float  b) {
            var roundedR = MathUtils.LinearTosRgb(r);
            var roundedG = MathUtils.LinearTosRgb(g);
            var roundedB = MathUtils.LinearTosRgb(b);
            return (roundedR << 16) | (roundedG << 8) | roundedB;
        }
    }
}