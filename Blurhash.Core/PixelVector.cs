using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Blurhash.Core
{
    public readonly struct PixelVector
    {
        public int Width { get; }
        public int Height { get; }
        /// <summary>
        /// 2nd index: X and RGB (BGR order because of little endian)
        /// B: [(y * XCount + x) * 3 ]
        /// G: [(y * XCount + x) * 3 + 1]
        /// R: [(y * XCount + x) * 3 + 2]
        /// and some extra elements to fit into Vector\<float\> 
        /// </summary>
        readonly float[] Pixels;
        /// <summary>
        /// Get a Vector Span of one line of Pixels
        /// </summary>
        public Span<Vector<float>> RowSpanVector(int y) =>MemoryMarshal.Cast<float, Vector<float>>(Pixels.AsSpan(y * XCount, XCount));
        /// <summary>
        /// Get a Span of one line of Pixels
        /// </summary>
        public Span<float> RowSpan(int y) => Pixels.AsSpan(y * XCount, XCount);
        /// <summary>
        /// Length of RowSpan()
        /// </summary>
        public int XCount { get; }
        /// <summary>
        /// Length of RowSpanVector()
        /// </summary>
        public int SpanLength { get; }

        public PixelVector(int width, int height)
        {
            Width = width;
            Height = height;
            SpanLength = (width * 3 + Vector<float>.Count - 1) / Vector<float>.Count;
            XCount = SpanLength * Vector<float>.Count;

            //With some extra elements
            Pixels = new float[Height * XCount];
        }

        /// <summary>
        /// For compatibility of test code
        /// </summary>
        /// <param name="isBgrByteOrder">BGR byte order e.g. GdiPlus.</param>
        /// <returns></returns>
        public Pixel[,] AsPixels(bool isBgrByteOrder)
        {
            var ret = new Pixel[Width, Height];
            for(int y = 0; y < Height; y++)
            {
                var pixelsY = Pixels.AsSpan(y * XCount, XCount);
                for(int x = 0; x < Width; x++)
                {
                    if (isBgrByteOrder)
                    {
                        ret[x, y].Blue = pixelsY[x * 3];
                        ret[x, y].Green = pixelsY[x * 3 + 1];
                        ret[x, y].Red = pixelsY[x * 3 + 2];
                    }
                    else
                    {
                        ret[x, y].Red = pixelsY[x * 3];
                        ret[x, y].Green = pixelsY[x * 3 + 1];
                        ret[x, y].Blue = pixelsY[x * 3 + 2];
                    }
                }
            }
            return ret;
        }
    }
}
