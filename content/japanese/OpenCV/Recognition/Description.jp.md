---
# 記事タイトルはこちら
title: "数式的な解説"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","ARマーカー","追従対象"]
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

#### ARマーカーの持つ値
各ARマーカーから得られる値は以下の3つ<br>
- ###### 座標の値:$[tx, ty, tz]$
- ###### 回転の値:$[rx, ry, rz]$
- ###### ID      :$id$<br>

認識できたARマーカーを$AR = [AR_0, \ldots , AR_n]$として、各ARマーカーの持つ値を$AR_i.id$のように表す<br>
##### 追従対象の情報を計算
- ##### 座標
  追従対象の座標$[tx,ty,tz]$は<br>
  $[tx, ty, tz] = [\frac{\sum_{n}^{i=0}AR_i.tx}{n},
                   \frac{\sum_{n}^{i=0}AR_i.ty}{n},
                   \frac{\sum_{n}^{i=0}AR_i.tz}{n}]$<br>
  で求める。
- ##### 回転
  追従対象の回転$[rx,ry,rz]$は<br>  
  $[rx, ry, rz] = [\frac{\sum_{n}^{i=0}AR_i.rx + AR_i.id \times 90}{n},
                   \frac{\sum_{n}^{i=0}AR_i.ry + AR_i.id \times 90}{n},
                   \frac{\sum_{n}^{i=0}AR_i.rz + AR_i.id \times 90}{n}]$<br>
    で求める。<br>
    ただし、$AR_i.id$が$0$で、回転の値が負の場合$+360$する。
- ##### 方向
  追従対象の方向はそれぞれ
  - 左右の方向 : $arctan(\frac{tx}{tz})$
  - 上下の方向 : $arctan(\frac{ty}{tz})$  
  で求める。
- ##### 距離
  カメラから追従対象の距離は<br>
  $d = \sqrt{tx^2 + ty^2 + tz^2}$
  で求める。