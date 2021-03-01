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
        readonly int Width, Height;
        readonly int MaxComponentsX, MaxComponentsY;

        /// <summary>
        // Basis X array.
        // [ComponentX][x*3+(r:0 g:1 b:2)]
        // for RGB packed array.
        // and has some extra elements to fit into Vector\<float\>
        /// </summary>
        readonly Vector<float>[][] BasisX;
        /// <summary>
        /// Basis Y array.
        /// [ComponentY][y]
        /// </summary>
        readonly float[][] BasisY;
        public CoreEncoder(int width, int height, int maxComponentsX, int maxComponentsY)
        {
            if (maxComponentsX < 1) throw new ArgumentException("maxComponentsX needs to be at least 1");
            if (maxComponentsX > 9) throw new ArgumentException("maxComponentsX needs to be at most 9");
            if (maxComponentsY < 1) throw new ArgumentException("maxComponentsY needs to be at least 1");
            if (maxComponentsY > 9) throw new ArgumentException("maxComponentsY needs to be at most 9");

            MaxComponentsX = maxComponentsX;
            MaxComponentsY = maxComponentsY;
            Width = width;
            Height = height;

            //Calculate X|Y basis
            //Original Basis is...
            //MathF.Cos(MathF.PI * xComponent * x / width) * MathF.Cos(MathF.PI * yComponent * y / height)

            BasisX = new Vector<float>[maxComponentsX][];
            int basisXVectorLength = (width * 3 + Vector<float>.Count - 1) / Vector<float>.Count;
            for (int c = 0; c < maxComponentsX; c++)
            {   var basisArray = new float[basisXVectorLength * Vector<float>.Count];
                for (int x = 0; x < width; x++)
                {
                    float basis = MathF.Cos(MathF.PI * c * x / Width);
                    basisArray[3 * x] = basis;
                    basisArray[3 * x + 1] = basis;
                    basisArray[3 * x + 2] = basis;
                }

                var basisVector = new Vector<float>[basisXVectorLength];
                BasisX[c] = basisVector;
                for(int i = 0; i < basisVector.Length; i++)
                {
                    basisVector[i] = new Vector<float>(basisArray, i * Vector<float>.Count);
                }
            }

            BasisY = new float[maxComponentsY][];
            for (int c = 0; c < maxComponentsY; c++)
            {
                var basisArray = new float[height];
                BasisY[c] = basisArray;
                for (int y = 0; y < basisArray.Length; y++)
                {
                    basisArray[y] = MathF.Cos(MathF.PI * c * y / height);
                }
            }
        }

        /// <summary>
        /// Encodes a PixelVector into a Blurhash string
        /// </summary>
        /// <returns>The resulting Blurhash string</returns>
        protected string CoreEncode(PixelVector pixels, int componentsX, int componentsY)
        {
            if (componentsX < 1) throw new ArgumentException("componentsX needs to be at least 1");
            if (componentsY < 1) throw new ArgumentException("componentsY needs to be at least 1");
            if (componentsX > MaxComponentsX) throw new ArgumentException("componentsX needs to be not more than MaxComponentsX");
            if (componentsY > MaxComponentsY) throw new ArgumentException("componentsY needs to be not more than MaxComponentsY");
            if (Width != pixels.Width || Height != pixels.Height) { throw new ArgumentException("Width/Height mismatch"); }

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
            var componentBasisX = BasisX[xComponent];
            var componentBasisY = BasisY[yComponent];

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

            float b = 0, g = 0, r = 0;
            float normalization = (xComponent == 0 && yComponent == 0) ? 1 : 2;

            //then sum horizontally
            for (int i = 0; i < width * 3; i += 3)
            {
                b += sumArray[i];
                g += sumArray[i + 1];
                r += sumArray[i + 2];
            }

            var scale = normalization / (width * height);
            return new Pixel(r * scale, g * scale, b * scale);
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