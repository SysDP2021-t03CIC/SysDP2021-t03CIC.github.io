---
# 記事タイトルはこちら
title: "カメラの特徴に関する前置き"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","カメラパラメータ","カメラキャリブレーション","歪み"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
---
<script type="text/javascript" async
  src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.1/MathJax.js?config=TeX-AMS-MML_HTMLorMML">
  MathJax.Hub.Config({
  tex2jax: {
    inlineMath: [['$','$'], ['\\(','\\)']],
    displayMath: [['$$','$$'], ['\\[','\\]']],
    processEscapes: true,
    processEnvironments: true,
    skipTags: ['script', 'noscript', 'style', 'textarea', 'pre'],
    TeX: { equationNumbers: { autoNumber: "AMS" },
         extensions: ["AMSmath.js", "AMSsymbols.js"] }
  }
  });
  MathJax.Hub.Queue(function() {
    // Fix <code> tags after MathJax finishes running. This is a
    // hack to overcome a shortcoming of Markdown. Discussion at
    // https://github.com/mojombo/jekyll/issues/199
    var all = MathJax.Hub.getAllJax(), i;
    for(i = 0; i < all.length; i += 1) {
        all[i].SourceElement().parentNode.className += ' has-jax';
    }
  });

  MathJax.Hub.Config({
  // Autonumbering by mathjax
  TeX: { equationNumbers: { autoNumber: "AMS" } }
  });
</script> 

カメラで撮影される映像(画像)は、カメラのレンズなどの種類によって様々な歪みが生じる。
この歪みを修正して、ARマーカーの正しい距離や角度を求めるために、カメラキャリブレーション(カメラ行列、歪み)を知っておく必要がある。

- カメラ行列<br>
    $ 3 \times 3 $の行列<br>
- 歪み係数<br>
    5つの係数を持つ配列
- 参考
    - [OpenCVでのキャリブレーション](http://opencv.jp/opencv-2.1/cpp/camera_calibration_and_3d_reconstruction.html)
    - [カメラパラメータの解説](https://qiita.com/S-Kaito/items/ace10e742227fd63bd4c)
    - [pythonのコード](https://qiita.com/ReoNagai/items/5da95dea149c66ddbbdd)