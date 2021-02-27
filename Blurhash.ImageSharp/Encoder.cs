﻿using System;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Blurhash.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Blurhash.ImageSharp
{
    public class Encoder : CoreEncoder 
    {
        public Encoder(int width, int height, int maxComponentsX, int maxComponentsY) : base(width, height, maxComponentsX, maxComponentsY)
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
        /// Encodes a picture into a Blurhash string
        /// </summary>
        /// <param name="image">The picture to encode</param>
        /// <param name="componentsX">The number of components used on the X-Axis for the DCT</param>
        /// <param name="componentsY">The number of components used on the Y-Axis for the DCT</param>
        /// <returns>The resulting Blurhash string</returns>
        public string Encode(Image<Rgba32> image, int componentsX, int componentsY)
        {
            return CoreEncode(ConvertBitmap(image), componentsX, componentsY);
        }

        /// <summary>
        /// Converts the given bitmap to the library-independent representation used within the Blurhash-core
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to encode</param>
        internal static PixelVector ConvertBitmap<T>(Image<T> sourceBitmap) where T : struct, IPixel<T>
        {
            if (typeof(T) != typeof(Rgba32) && typeof(T) != typeof(Rgb24))
                throw new ArgumentOutOfRangeException(nameof(sourceBitmap), "Only Rgba32 and Rgb24 are supported");
            
            var width = sourceBitmap.Width;
            var height = sourceBitmap.Height;
            var bytesPerPixel = sourceBitmap.PixelType.BitsPerPixel / 8;
            var stride = width * 3;

            var result = new PixelVector(width, height);
            
            for (int y = 0; y < height; y++)
            {
                var rgbValues = MemoryMarshal.AsBytes(sourceBitmap.GetPixelRowSpan(y));

                var index = stride;
                var pixelsY = result.Pixels[y];
                for (var i = 0; i < width * 3; i++)
                {
                    pixelsY[i] = rgbValues[i];
                }
            }

            return result;
        }
    }
}