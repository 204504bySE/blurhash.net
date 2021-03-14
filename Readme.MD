# blurhash.net Performance Fork
This is a performance fork of [blurhash.net](https://github.com/MarkusPalcer/blurhash.net).

Only the encoder has been tuned. The decoder is still untouched. (I'll tune it later...)

## Performance

See [Benchmarks](https://github.com/204504bySE/blurhash.net/wiki/Benchmarks).

## Branches

* vector: The fastest version. Not compatible with the original version.
* basisprovider: Just reduced calling Math.Cos().
* master: The original version.


## Motivation
I have a website [TwiGaTen](https://twigaten.204504byse.info/). ([source code](https://github.com/204504bySE/twigaten))
It crawls Twitter to collect tweets with image and image itself. It only collects ":thumb"(150x150px) images but the number is about 3~4 million/day. And I use 9x9 blurhash to make them as clear as possible.

However, original [blurhash.net](https://github.com/MarkusPalcer/blurhash.net) was heavy for my usage. It doubled the CPU load of my server... 

## Projects in this solution
This list does not contain the projects with names ending in `.Test` as they are the test projects for the ones listed here.

- Blurhash.Core
  The core algorithm of blurhash. For maximum compatibility this is a .NET-Standard project.
- Blurhash-System.Drawing.Blurhash
  Bridge-Library to use Blurhash with the `System.Drawing.dll` namespace of the .NET-Framework.
  This is only available in Windows projects as it uses GDI+
- Blurhash-System.Drawing.Common
  Bridge-Library to use Blurhash with the System.Drawing.Common NuGet-Library in DotNetStandard
- Blurhash.ImageSharp
  Bridge-Library to use Blurhash with ImageSharp
