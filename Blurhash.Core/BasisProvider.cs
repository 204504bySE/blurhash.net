using System;
using System.Collections.Generic;
using System.Text;

namespace Blurhash.Core
{
    /// <summary>
    /// Calculate X|Y basis for CoreEncoder
    /// Original Basis is...
    /// Math.Cos(Math.PI * xComponent * x / width) * Math.Cos(Math.PI * yComponent * y / height)
    /// </summary>
    public interface IBasisProviderEncode
    {
        /// <summary>
        // Basis X array. [x]
        ///</summary>
        double[] BasisX(int width, int componentX);
        /// <summary>
        /// Basis Y array. [y]
        /// </summary>
        double[] BasisY(int height, int componentY);
    }

    /// <summary>
    /// The naive IBasisProviderEncode implementation
    /// </summary>
    public class BasisProviderEncode : IBasisProviderEncode
    {
        public double[] BasisX(int width, int componentX)
        {
            var basisArray = new double[width];
            for (int x = 0; x < basisArray.Length; x++)
            {
                basisArray[x] = Math.Cos(Math.PI * componentX * x / width);
            }
            return basisArray;
        }

        public double[] BasisY(int height, int componentY)
        {
            var basisArray = new double[height];
            for (int y = 0; y < basisArray.Length; y++)
            {
                basisArray[y] = Math.Cos(Math.PI * componentY * y / height);
            }
            return basisArray;
        }
    }
}
