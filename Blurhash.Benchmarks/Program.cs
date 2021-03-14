using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Blurhash.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ImageSharpBenchmarks>();
        }
    }

    public class ImageSharpBenchmarks
    {
        [Benchmark]
        public async Task EncodeBenchmark()
        {
            var image = Image.Load<Rgb24>("1233360707896238080.jpg");
            var encoder = new Encoder();
            encoder.Encode(image, 9, 9);
        }
    }
}
