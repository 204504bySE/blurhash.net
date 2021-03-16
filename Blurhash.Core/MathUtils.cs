using System;
using System.Runtime.CompilerServices;

namespace Blurhash.Core
{
    /// <summary>
    /// Utility methods for mathematical calculations
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Calculates <c>Math.Pow(base, exponent)</c> but retains the sign of <c>base</c> in the result.
        /// </summary>
        /// <param name="base">The base of the power. The sign of this value will be the sign of the result</param>
        /// <param name="exponent">The exponent of the power</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignPow(float @base, float exponent)
        {
            return Math.Sign(@base) * MathF.Pow(Math.Abs(@base), exponent);
        }

        /// <summary>
        /// Converts an sRGB input value (0 to 255) into a linear double value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SRgbToLinear(int value) {
            float v = value;
            if (value <= 10) return v * (float)(1 / (12.92 * 255)); // 0.04045 * 255 = 10.31475
            else return MathF.Pow((v * (1f / 255f) + 0.055f) *  (1f / 1.055f), 2.4f);
        }

        /// <summary>
        /// Converts a linear double value into an sRGB input value (0 to 255)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LinearTosRgb(float value) {
            float v = Math.Max(0f, Math.Min(1f, value));
            if(v <= 0.0031308f) return (int)(v * (12.92f * 255f) + 0.5);
            else return (int)((1.055 * Math.Pow(v, 1f / 2.4f) - 0.055f) * 255f + 0.5f);
        }
    }
}