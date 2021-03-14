using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Blurhash.Core
{
    /// <summary>
    /// Calculate X|Y basis
    /// Original Basis is...
    /// Math.Cos(Math.PI * xComponent * x / width) * Math.Cos(Math.PI * yComponent * y / height)
    /// </summary>
    public interface IBasisProvider
    {
        /// <summary>
        // Basis X array.
        // [x*3+(0~2(rgb))]
        // for RGB packed array.
        // and has some extra elements to fit into Vector\<float\>
        /// </summary>
        Vector<float>[] BasisX(int width, int componentX);
        /// <summary>
        /// Basis Y array.
        /// [y]
        /// </summary>
        float[] BasisY(int height, int componentY);
    }    

    /// <summary>
    /// The naive IBasisProvider implementation
    /// </summary>
    public class BasisProvider : IBasisProvider
    {
        public Vector<float>[] BasisX(int width, int componentX)
        {
            int basisXVectorLength = (width * 3 + Vector<float>.Count - 1) / Vector<float>.Count;
            var basisArray = new float[basisXVectorLength * Vector<float>.Count];
            for (int x = 0; x < width; x++)
            {
                float basis = MathF.Cos(MathF.PI * componentX * x / width);
                basisArray[3 * x] = basis;
                basisArray[3 * x + 1] = basis;
                basisArray[3 * x + 2] = basis;
            }

            var basisVector = new Vector<float>[basisXVectorLength];
            for (int i = 0; i < basisVector.Length; i++)
            {
                basisVector[i] = new Vector<float>(basisArray, i * Vector<float>.Count);
            }
            return basisVector;
        }

        public float[] BasisY(int height, int componentY)
        {
            var basisArray = new float[height];
            for (int y = 0; y < basisArray.Length; y++)
            {
                basisArray[y] = MathF.Cos(MathF.PI * componentY * y / height);
            }
            return basisArray;
        }
    }
}
