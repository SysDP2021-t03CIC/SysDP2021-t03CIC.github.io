---
# 記事タイトルはこちら
title: "カメラの特徴に関する前置き"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","カメラパラメータ","カメラキャリブレーション","内部行列","歪み"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
# 以下は目次において何番目に表示するか決めるタグです。
weight: 1
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

カメラで撮影される映像(画像)は、カメラのレンズなどの種類によって写し方、歪みが様々である。
ARマーカーの正しい距離や回転を求めるために、カメラの内部行列と歪み係数を計算しておく必要がある。

- ###### 内部行列<br>
  $ 3 \times 3 $の行列<br>
  <div>
  $ \begin{bmatrix}
    f_x & 0 & c_x \\
    0 & f_y & c_y \\
    0 & 0 & 1
    \end{bmatrix}
  $
  </div>
  カメラを原点とする3次元の座標から画像の左上を原点とする2次元の座標に変換する役割がある。<br>
  逆に、2次元の画像座標から3次元の画像座標を計算するのにも使える。<br>
  $f_x, f_y$はスケーリングとしての役割。<br>
![カメラから画像へ01](/images/OpenCV/Parameter/Description/ParaDesc01.jpg "カメラから画像へ01")
  $c_x, c_y$は画像座標の原点を左上にする役割がある。
![カメラから画像へ02](/images/OpenCV/Parameter/Description/ParaDesc02.jpg "カメラから画像へ02")
- ###### 歪み係数<br>
  5つの係数を持つ配列$(k_1, k_2, p_1, p_2[, k_3])$。<br>
  上記の内部行列だけではカメラのレンズによる歪みを考慮できていない。<br>
  歪み係数はその歪みを考慮するための係数。<br>
  $k_3$は指定しなければ0になる。

- 参考
  - [OpenCVでのキャリブレーション](http://opencv.jp/opencv-2.1/cpp/camera_calibration_and_3d_reconstruction.html)
  - [内部行列の解説1](https://mem-archive.com/2018/02/21/post-157/)
  - [内部行列の解説2](https://qiita.com/S-Kaito/items/ace10e742227fd63bd4c)
  - [pythonのコード](https://qiita.com/ReoNagai/items/5da95dea149c66ddbbdd)