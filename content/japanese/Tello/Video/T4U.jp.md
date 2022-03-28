---
# 記事タイトルはこちら
title: "Tello for Unity"
# 投稿日時をご記入ください
date: 2022-03-27T19:59:10+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["Unity","ドローン","Tello","カメラ映像"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tanaka"
# 以下は目次において何番目に表示するか決めるタグです。
weight: 1
---

### Tello Libとは
その名の通り、Telloを制御するためのライブラリ。公式SDKよりも細かな制御を行う事ができる。

[Tello Lib](https://github.com/comoc/TelloLib/tree/c056d1deb3c337370b477d739261662e3be3a5ac)

### Tello for Unity とは
Unity専用のTello開発キット。公式ではなく有志が開発。

[Tello for Unity](https://github.com/comoc/TelloForUnity)

#### Tello Video Decoder
Tello for Unity独自の映像変換用ライブラリ。

#### インストール
適当なディレクトリで以下のコマンドを使用
```
$ git clone --recursive https://github.com/comoc/TelloForUnity.git
```
--recursiveを付ける理由は、Tello for Unityの使用に別途Tello Libが必須であるため。付ける事でsubmoduleも併せてcloneできる。

#### 使い方
1. cloneしたリポジトリをUnityプロジェクトとしてUnity Hubで選択する。
1. Unityプロジェクトの設定で、Runtime Versionを.NET 4.xにする。
1. Assets/Scripts内にある「TelloController.cs」を適当なオブジェクトに付ける。
1. 映像を表示したいオブジェクトに「TelloVideoTexture.cs」を付ける