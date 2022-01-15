---
# 記事タイトルはこちら
title: "OpenCV for Unity"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","OpenCV","OpenCV for Unity"]
---

#### 2021年度の開発で最終的に利用したアセット
- ##### ダウンロード
[OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?locale=ja-JP)<br>
概要欄の「Trail & Demo」に試用版もある  
2021年度の開発で利用したバージョンは2.4.5
- ##### 利用

- ##### コードの説明
    正直引き継ぎでアプリ実行するだけなら、見る必要は無いが少しだけメソッドの説明をしておく。  
    プログラムの処理の流れはスクリプトファイル内のコメントや参考にしたサイトを見てほしい。
    - ###### Matクラスのインスタンス生成
        ```
        new Mat(//行数, //列数, //CvType.CV_[成分の型]C[チャンネル数])
        ```
        ※成分の型:(例)double型 → 64F, short型 → 32S等  
        ※チャンネル:行列の成分の表し方(行,列)にもう1つ要素が加わったもの(というイメージ)
    - ###### Matに具体的な数値を格納する
        ```
        Mat.put(//数値を入れ始める行の目, //列の目, //具体的な数値の格納された配列)
        ```
    - ###### Matから具体的な数値を取り出す
        ```
        Mat.get(//取り出す初める行の目,//列の目,//取り出した数値を格納する配列)
        ```
    - ###### Mat同士の結合(横に並べる結合)
        ```
        Core.hconcat(//結合するMatのList, //結合結果を格納するMat)
        ```

その他のメソッドの内容も[リファレンス](https://enoxsoftware.github.io/OpenCVForUnity/3.0.0/doc/html/namespace_open_c_v_for_unity.html)で見ることができる。~~が正直分かりにくい~~

- ##### 参考
    - [OpenCV for Unity 入門 (1) - 事始め](https://note.com/npaka/n/n0148bca896aa)
    - [pythonでArUcoライブラリを使ってARマーカの位置とか角度とか知る](https://sgrsn1711.hatenablog.com/entry/2018/02/15/224615)
    - [pythonでARマーカーの姿勢推定](https://qiita.com/ReoNagai/items/a8fdee89b1686ec31d10)
    - [OpenCV for Unityのリファレンス](https://enoxsoftware.github.io/OpenCVForUnity/3.0.0/doc/html/namespace_open_c_v_for_unity.html)