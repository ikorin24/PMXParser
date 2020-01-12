# PMX Parser

[![GitHub license](https://img.shields.io/github/license/ikorin24/PMXParser?color=967CFF)](https://github.com/ikorin24/PMXParser/blob/master/LICENSE)
[![nuget](https://img.shields.io/badge/nuget-v1.0.0-967CFF)](https://www.nuget.org/packages/PMXParser)

[English](https://github.com/ikorin24/PMXParser/blob/master/README.md)

## これは何？

PMX ファイルの C# (.NET Standard 2.1 / 2.0) パーサーライブラリです。

PMX ファイルは MMD (*Miku Miku Dance*) のモデルデータです。

このパーサーは PMX ファイルを C# のクラス構造に読み込みます。

これはただのパーサーなので、3D モデルの描画はこのライブラリでは出来ません。

## 使い方

ファイル名を指定してパース

```cs
var pmx = MMDTools.PMXParser.Parse("your_file.pmx");
```

`Stream`からのパース

```cs
using(var stream = System.IO.File.OpenRead(fileName))
{
    var pmx = MMDTools.PMXParser.Parse(stream);
}
```

## 必要環境と依存関係 (ビルド時)

- .NET Standard 2.1 / 2.0
- C# 8.0
- `dotnet` コマンド (.NET Core CLI ツール)

## 導入方法

パッケージを Nuget から取得できます。

https://www.nuget.org/packages/PMXParser

```sh
# nuget パッケージマネージャー
PM> Install-Package PMXParser
```

## ビルド方法

Windows, Mac, Linux

```sh
$ git clone https://github.com/ikorin24/PMXParser.git
$ cd PMXParser
$ dotnet build PMXParser/PMXParser.csproj -c Release

# ---> PMXParser/bin/Release/netstandard2.1/PMXParser.dll
# ---> PMXParser/bin/Release/netstandard2.0/PMXParser.dll
```

## PMX ファイルフォーマットについて

PMX ファイルフォーマットは下記のリンクからダウンロードできる PmxEditor 付属のテキストに書かれています。ZIP ファイルをダウンロードして`'PmxEditor/Lib/PMX仕様書/PMX仕様.txt'`を解凍してください。

PmxEditor は私が製作したものではありません、ご注意ください。

http://kkhk22.seesaa.net/category/14045227-1.html

## ライセンスについて

作成者 : [ikorin24](https://github.com/ikorin24)

このリポジトリは [MIT ライセンス](https://github.com/ikorin24/PMXParser/blob/master/LICENSE)です。

また、このリポジトリはいくつかのライセンス製品を含んでいます。使用製品とそのライセンス一覧については[こちら](https://github.com/ikorin24/PMXParser/blob/master/CREDITS.md)をご覧ください。

## リリースノート

### 2020/01/04 ver 0.9.0

初版リリース

### 2020/01/06 ver 0.9.1

バグ修正と多少のパフォーマンス改善

### 2020/01/09 ver 0.9.2

- .NET Framework バージョンを追加
- いくつかの型に`DebuggerDisplayAttribute`を追加

### 2020/01/12 ver 1.0.0

[![nuget](https://img.shields.io/badge/nuget-v1.0.0-967CFF)](https://www.nuget.org/packages/PMXParser/1.0.0)

- ターゲットの .NET バージョンを .NET Standard 2.1 と 2.0に変更。
- 微修正

### 2020/01/12 ver 1.0.1

[![nuget](https://img.shields.io/badge/nuget-v1.0.1-967CFF)](https://www.nuget.org/packages/PMXParser/1.0.1)

- 大きなバグ修正
    - `NextDataOfSize`メソッドの`byteSize`が4でない場合、正しくない値を読み取っていた
