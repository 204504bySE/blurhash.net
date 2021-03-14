using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Blurhash.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://github.com/dotnet/BenchmarkDotNet/issues/856
            var x86net5 = Job.ShortRun
                .WithPlatform(BenchmarkDotNet.Environments.Platform.X86)
                .WithToolchain(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp50.WithCustomDotNetCliPath(@"C:\Program Files (x86)\dotnet\dotnet.exe")))
                .WithId("x86 .NET 5"); // displayed in the results table

            var x64net5 = Job.ShortRun
                .WithPlatform(BenchmarkDotNet.Environments.Platform.X64)
                .WithToolchain(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp50.WithCustomDotNetCliPath(@"C:\Program Files\dotnet\dotnet.exe")))
                .WithId("x64 .NET 5");

            var config = DefaultConfig.Instance
                .AddJob(x86net5)
                .AddJob(x64net5);

            BenchmarkRunner.Run<ImageSharpBenchmarks>(config);
        }
    }

    public class ImageSharpBenchmarks
    {
        [Benchmark]
        public void LoadImageBenchmark()
        {
            var image = Image.Load<Rgb24>("1233360707896238080.jpg");
        }

        [Benchmark]
        public void EncodeBenchmark()
        {
            var image = Image.Load<Rgb24>("1233360707896238080.jpg");
            var encoder = new Encoder();
            encoder.Encode(image, 9, 9);
        }
    }
}
