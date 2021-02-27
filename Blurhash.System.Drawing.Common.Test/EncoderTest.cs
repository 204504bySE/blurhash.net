﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Drawing.Common.Blurhash
{
    [TestClass]
    public class EncoderTest
    {
        [TestMethod]
        public void EncodeTest()
        {
            var image = Image.FromFile("1233360707896238080.jpg");
            var encoder = new Encoder(150,150, 9,9);

            var encoded = encoder.Encode(image, 9, 9);
            //Result of float non-vector version
            Assert.AreEqual(@"|cPixSOsi_n%XmgNWVj[bH1kWrW;ayaKaKjZaejZG^rXtQkCiwnij[j[jtQTxti^a|XSXSbHbHbHw^o}X8e:j[jZe.n%fQyDRjX9f8i{jFf7ayjZt7VtVsaykWbbbabbbGk=V[j?kVofkCjZoLayR6baozofaejbjZjFj[", encoded);
            Console.WriteLine();
        }

        public void EncodeBenchmark()
        {
            var image = Image.FromFile("1233360707896238080.jpg");
            var encoder = new Encoder(150, 150, 9, 9);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                encoder.Encode(image, 9, 9);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
