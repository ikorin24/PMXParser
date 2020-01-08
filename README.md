# PMX Parser

[日本語](https://github.com/ikorin24/PMXParser/blob/master/README_ja.md)

## What is This ?

PMX file parser library of C# (.NET Standard 2.1 / .NET Framework 4.8). PMX file is MMD (*Miku Miku Dance*) model file.

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

- .NET Standard 2.1 / .NET Framework 4.8
- C# 8.0
- `dotnet` command (.NET Core CLI Tools)

## Building

### .NET Standard 2.1

Windows, Mac, Linux

```sh
$ dotnet build PMXParser/PMXParser.csproj -c Release

# ---> PMXParser/bin/Release/netstandard2.1/PMXParser.dll
```

### .NET Framework 4.8

Windows

```sh
$ dotnet build PMXParser/PMXParser.NetFramework.csproj -c Release

# ---> PMXParser/bin/Release/net48/PMXParser.NetFramework.dll
```

## You don't Know PMX File Format ?

You can see the format of PMX in a text of PmxEditor, download from the following link. Download zip and extract `'PmxEditor/Lib/PMX仕様書/PMX仕様.txt'`. (It is written in Japanese)

**NOTICE**

PmxEditor is **NOT MY PRODUCTION**.

http://kkhk22.seesaa.net/category/14045227-1.html

## License and Credits

Author : [ikorin24](https://github.com/ikorin24)

This repository is under [MIT License](https://github.com/ikorin24/PMXParser/blob/master/LICENSE).

This repository contains some licensed products. The list of them and their license are [HERE](https://github.com/ikorin24/PMXParser/blob/master/CREDITS.md).

## Release Note

### 2020/01/04 ver 0.9.0

First Release

### 2020/01/06 ver 0.9.1

Bug fix and performance improvement a little

### 2020/01/09 ver 0.9.2

- Add .NET Framework version
- Add `DebuggerDisplayAttribute` to some types
