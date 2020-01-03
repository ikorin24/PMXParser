# PMX Parser

[English](https://github.com/ikorin24/PMXParser/blob/master/README.md)

## これは何？

PMX ファイルの C# (.NET Starndard 2.1) パーサーライブラリです。

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

- .NET Standard 2.1
- C# 8.0
- `dotnet` コマンド (.NET Core CLI ツール)

## ビルド方法

Windows, Mac, Linux

```sh
$ dotnet build PMXParser/PMXParser.csproj -c Release

# ---> PMXParser/bin/Release/netstandard2.1/PMXParser.dll
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
