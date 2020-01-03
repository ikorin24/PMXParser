# PMX Parser

[日本語](https://github.com/ikorin24/PMXParser/blob/master/README_ja.md)

## What is This ?

PMX file parser library of C#, .NET Standard 2.1. PMX file is MMD (*Miku Miku Dance*) model file.

This parser parses PMX file into structual C# class.

**This is just a parser, so drawing 3D models is NOT SUPPORTED in this library.**

## How to Use

Parsing from file name

```cs
var pmx = MMDTools.PMXParser.Parse("your_file.pmx");
```

Parsing from `Stream`

```cs
using(var stream = System.IO.File.OpenRead(fileName))
{
    var pmx = MMDTools.PMXParser.Parse(stream);
}
```

## Requirements and Dependencies (On Building)

- .NET Standard 2.1
- C# 8.0
- `dotnet` command (.NET Core CLI Tools)

## Building

Windows, Mac, Linux

```sh
$ dotnet build PMXParser/PMXParser.csproj -c Release

# ---> PMXParser/bin/Release/netstandard2.1/PMXParser.dll
```

## You don't Know PMX File Format ?

You can see the format of PMX in a text of PmxEditor, download from the following link. Download zip and extract `'PmxEditor/Lib/PMX仕様書/PMX仕様.txt'`. (It is written in Japanese)

**NOTICE**

PmxEditor is **NOT MY PRODUCTION**.

http://kkhk22.seesaa.net/category/14045227-1.html

## License and Credits

This repository is under [MIT License](https://github.com/ikorin24/PMXParser/blob/master/LICENSE).

This repository contains some licensed products. The list of them and their license are [HERE](https://github.com/ikorin24/PMXParser/blob/master/CREDITS.md).
