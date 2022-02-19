---
# 記事タイトルはこちら
title: "ARマーカーから追従対象の認識"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","ARマーカー","追従対象"]
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

#### ARマーカーの認識から追従対象の距離・角度の推定についての解説
認識できたARマーカーを$AR = [ AR_0, AR_1, \ldots ]$,<br>
各ARマーカーから得られる情報を<br>
- ARマーカーの座標 : $AR_i.TX,AR_i.TY,AR_i.TZ$<br>
- ARマーカーの回転 : $AR_i.RX,AR_i.RY,AR_i.RZ$<br>
- ARマーカーのID : $AR_i.ID$<br>
としておく。
- ##### 座標
    追従対象の座標$[tx,ty,tz]$は<br>
    - $tx = 1/n \sum_{n}^{i=0}AR_i.X$<br>
    - $ty = 1/n \sum_{n}^{i=0}AR_i.Y$<br>
    - $tz = 1/n \sum_{n}^{i=0}AR_i.Z$<br>
    で求める。
- ##### 回転
    追従対象の回転$[rx,ry,rz]$はおおよそ上記の座標と同じ。<br>  
    - $rx = 1/n \sum_{n}^{i=0}AR_i.X + AR_i.ID \times 90$<br>
    - $ry = 1/n \sum_{n}^{i=0}AR_i.Y + AR_i.ID \times 90$<br>
    - $rz = 1/n \sum_{n}^{i=0}AR_i.Z + AR_i.ID \times 90$<br>
    で求める。
- ##### 方向
    得られた座標の値から  
    - 左右の方向 : $arctan(tx / tz)$ <br>
    - 上下の方向 : $arctan(ty / tz)$  
    で求められる。
- ##### 距離
    座標の値から$d = \sqrt{tx^2 + ty^2 + tz^2}$ で求められる。