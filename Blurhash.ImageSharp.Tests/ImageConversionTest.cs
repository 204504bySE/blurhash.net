using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FluentAssertions;

namespace Blurhash.ImageSharp.Tests
{
    [TestClass]
    public class ImageConversionTest
    {
        [TestMethod]
        public void TestConversion24BppRgb()
        {
            var rnd = new Random();


            var sourceImage = new Image<Rgb24>(20, 20);

            for (var x = 0; x < 20; x++)
                for (var y = 0; y < 20; y++)
                {
                    sourceImage[x,y] = new Rgb24((byte)(rnd.Next(0, 2) * 255), (byte)(rnd.Next(0, 2) * 255), (byte)(rnd.Next(0, 2) * 255));
                }

            var sourceData = Encoder.ConvertBitmap(sourceImage).AsPixels(false);

            for (var x = 0; x < 20; x++)
                for (var y = 0; y < 20; y++)
                {
                    var pixel = sourceImage[x, y];

                    sourceData[x, y].Red.Should().BeApproximately(pixel.R == 0 ? 0f : 1f, float.Epsilon);
                    sourceData[x, y].Green.Should().BeApproximately(pixel.G == 0 ? 0f : 1f, float.Epsilon);
                    sourceData[x, y].Blue.Should().BeApproximately(pixel.B == 0 ? 0f : 1f, float.Epsilon);
                }

            var targetImage = Decoder.ConvertToBitmap(sourceData);

            for (var x = 0; x < 20; x++)
                for (var y = 0; y < 20; y++)
                {
                    targetImage[x, y].Should().Be(sourceImage[x, y]);
                }
        }
    }
}
