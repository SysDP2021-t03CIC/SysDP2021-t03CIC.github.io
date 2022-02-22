---
# 記事タイトルはこちら
title: "OpenCVSharp"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","OpenCV","OpenCVSharp","断念"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
# 以下は目次において何番目に表示するか決めるタグです。
weight: 2
---

#### 2021年度の開発で断念したOpenCVライブラリの.NET用ラッパー
1. TextureをMatに変換する関数がない
2. 変換関数を自作したが動作が遅かった
これらの理由から利用を断念。OpenCV for Unityの方を利用した。

#### OpenCVSharpの導入
##### 1. NuGetの導入
1. [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity/releases)から「NuGEtForUnity.〇.〇.〇.unitypackage」をダウンロード
2. 「NuGEtForUnity.〇.〇.〇.unitypackage」をUnityEditorのウィンドウにドラッグ&ドロップ
3. 全てチェックを付けたままインポート

##### 2. OpenCVSharpの導入
1. UnityEditorのヘッダーメニュー「NuGet」→「Manage Nuget Packages」を選択
2. 「OpenCVSharp」を検索しインストール
    - OpenCVSharp3の場合は、OpenCVSharp3-AnyCPU
    - OpenCVSharp4の場合は、OpenCVSharp4.WindowsやOpenCVSharp4+OpenCVSharp4.runtime.○○
3. [opencvsharp](https://github.com/shimat/opencvsharp/releases)からインストールしたバージョンに近いもののzipファイルをダウンロード
4. ダウンロードしたファイルをにある「OpenCVSharpExtern.dll」を「Assets/Packages/OpenCvSharp○-○○/lib/netstandard2.0」にコピー