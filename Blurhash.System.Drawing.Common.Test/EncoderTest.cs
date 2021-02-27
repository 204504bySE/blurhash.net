using System;
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
            var encoder = new Encoder();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                encoder.Encode(image, 9, 9);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
