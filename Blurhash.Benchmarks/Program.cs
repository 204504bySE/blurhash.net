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
            var image = await Image.LoadAsync<Rgb24>("1233360707896238080.jpg");
            var encoder = new Encoder(150, 150, 9, 9);
            encoder.Encode(image, 9, 9);
        }
    }
}
