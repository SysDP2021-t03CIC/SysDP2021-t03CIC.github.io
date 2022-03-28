---
# 記事タイトルはこちら
title: "重みづけをした実際の移動距離の計算"
# 投稿日時をご記入ください
date: 2022-03-22T15:02:35+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["Tello","追従","計算式"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "ban"
weight: 2
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

『移動距離の計算について』で算出した値を用いて実際に移動する距離を求める。  
重みづけを行うことによって**過度な移動を防いだり、ドローン視点の映像を安定させること**を目的とした。実際、**安定した映像の取得は実現できたが、それと両立したドローンの安定した追従がまだ完璧に出来ていない**ため、改善の余地があることに留意する。  

### 「TelloRequestCalculation02.cs」内の説明  

```C#
if (textureSender.estimater.detected)
{
  waitTimer = 0.0f;
  double targetPosX = textureSender.estimater.position[0];
  double targetPosZ = textureSender.estimater.position[2];
  double targetRotY = textureSender.estimater.rotation[1];
  Debug.Log(String.Format(" TX:{0} [m], TY:{1} [m], RY{2} [°]", targetPosX, targetPosZ, targetRotY));

  double moveX = RequestCalculation.CalculateRequestMoveX(targetPosX, targetRotY);
  double moveY = RequestCalculation.CalculateRequestMoveY(targetPosZ, targetRotY);
  double rotaX = RequestCalculation.CalculateRequestRoteY(targetRotY);

  AxisResize(moveX, moveY, rotaX);

  rx = moveX_perFrame;
  ry = moveY_perFrame;
  lx = rotaX_perFrame;
  Debug.Log(String.Format(" rx:{0} [m], ry:{1} [m], lx:{2} [°]", rx, ry, lx));
}
```
targetPosX,targetPosZ,targetRotYは1で説明したようにカメラで取得したARマーカーとのそれぞれの距離である。それらを理想の移動距離に直したものがその下のmoveX,moveY,rotaXである。  

今回注目する箇所はAxisResize(moveX, moveY, rotaX);である。この箇所で重みづけの作業を行っている。  

####  AxisResize内について  
```C#
private void AxisResize(double moveX, double moveY, double rotaX)
{
  double moveAxisVecNorm = Math.Sqrt(moveX * moveX + moveY * moveY);
  moveX_perFrame = (moveAxisVecNorm.Equals(0.0)) ? 0.0f : (float)(moveX / moveAxisVecNorm);
  moveY_perFrame = (moveAxisVecNorm.Equals(0.0)) ? 0.0f : (float)(moveY / moveAxisVecNorm);

  moveX_perFrame = MyarcsinFuncX(moveX_perFrame);
  moveY_perFrame = MyarcsinFuncY(moveY_perFrame);

  rotaX_perFrame = MysigmoidFunc(rotaX / Math.PI);
}
```

ここでは重み付けの作業の準備を行う。  
「moveAxisVecNorm」はmoveXとmoveYの移動を合体させた斜めの移動について考える。もし「moveAxisVecNorm」が0.0であれば「moveX_perFrame」、「moveY_perFrame」において0.0fと設定し、そうでなければsin,cosをそれぞれ代入する。そして、代入された「moveX_perFrame」,「moveY_perFrame」を「MyarcsinFuncX」,「MyarcsinFuncY」で重みづけをした値にして実際の移動距離を求める。

- #### x軸方向(左右)の計算（MyarcsinFuncX）  
**式：moveX_perFrame $=\frac{2}{9}×\frac{4\arcsin\(x)}{\pi}\$**  
![MyarcsinFuncX](/images/Tello/AutoTracking/Weighting/MyarcsinFuncX.png "MyarcsinFuncX")  
```C#
private float MyarcsinFuncX(double x)
{
  if (x < -1) x = -0.9f;
  if (x > 1) x = 0.9f;
  return (float)(4 * 2 * Math.Asin(x) / (9 * Math.PI) );
}
```

- #### y軸方向(前後)の計算（MyarcsinFuncY）  
**式：moveY_perFrame $=\frac{2}{3}×\frac{\arcsin\(x)}{\pi}\$**  
![MyarcsinFuncY](/images/Tello/AutoTracking/Weighting/MyarcsinFuncY.png "MyarcsinFuncY")   
```C#
private float MyarcsinFuncY(double x)
{
  if (x < -1) x = -0.9f;
  if (x > 1) x = 0.9f;
  return (float)(2 * Math.Asin(x) / (3 * Math.PI));
}
```

- #### 旋回の計算（MysigmoidFunc）  
**式：rotaX_perFrame $=\frac{2}{1+e^{-7.3x}}-1$**  
![MysigmoidFunc](/images/Tello/AutoTracking/Weighting/MysigmoidFunc.png "MysigmoidFunc")  
```C#
private float MysigmoidFunc(double x)
{
  return (float)(2 / (1 + Math.Exp(-7.3f * x)) - 1);
}
```

以上の3つの動きに関して、小さい動きに対してはできるだけ反応しないようにして、大きく動くときに対象についていけるように相応に大きく動くように調整した。それにより、小さな動きによるブレや小移動が少なくなりドローンの映像が安定したものになったが、ドローンの安定した挙動の実現であったり対象を見失ったりなどといった課題も残っている。