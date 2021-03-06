﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Blurhash.ImageSharp.Tests
{
    [TestClass]
    public class EncoderTest
    {
        [TestMethod]
        public async Task EncodeTest()
        {
            var image = await Image.LoadAsync<Rgb24>("1233360707896238080.jpg");
            var encoder = new Encoder();

            var encoded = encoder.Encode(image, 9, 9);
            //Result of float non-vector version
            Assert.AreEqual(@"|cPixSOsi_n%XmkqWVj[bH1kWrW;ayaKaKjZaejZG^rXtQkCiwnioLj[jaQTxti^a|XSXSbHbHbHxDo}X8e:j[jZe.n%fQ%gRjX9f8i{jFf7ayjZt7VtVsaykWbbbbbbbGk=V[j?kVofkCjZoLayR6baozofaejbjZjFj[", encoded);
            Console.WriteLine();
        }
    }
}
