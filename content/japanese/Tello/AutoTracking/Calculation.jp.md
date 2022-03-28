---
# 記事タイトルはこちら
title: "移動距離の計算について"
# 投稿日時をご記入ください
date: 2022-03-15T14:34:00+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["Tello","追従","計算式"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "ban"
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

『移動距離に関する制御のコード』では「他のファイルで計算された」と表記したが、その箇所に関しての説明をここで行う。  
移動距離の計算は「TelloRequestCalculation02.cs」で行われており、前後移動、左右移動、旋回の3つについての移動距離の計算を行う。

#### 前提の説明  
「TelloRequestCalculation02.cs」で値を送る前に、「TelloController1122.cs」ではARマーカーとの距離の値を取得する。  
![移動経路計算](/images/Tello/AutoTracking/Calculation/figure.png "移動経路計算")  

円中心の三角形がARマーカーを付けた追従対象で、左下にあるものがドローンとする。この時、下のコードから3つのパラメーターを受け取る。  

```C#
double targetPosX = textureSender.estimater.position[0]; // 左右方向の距離
double targetPosZ = textureSender.estimater.position[2]; // 前後方向の距離
double targetRotY = textureSender.estimater.rotation[1]; // ARマーカーとの角度
```
targetPosX → ドローンとARマーカーまでの横方向の距離  
targetPosZ → ドローンとARマーカーまでの前方向の距離  
targetRotY → ドローンから見たARマーカーの角度(右方向に+、左方向に-)  
この時、取得できる単位は <u>**X,Y は[m]、 Y は[°]**</u>であることに気を付ける。  

### 「TelloRequestCalculation02.cs」内の説明  
まず、TelloとARマーカーがどれだけ離れて追従するかを設定する。  
```C#
// 単位は[cm]
private static double targetDistance = 2;
```
この場合2m離れて追従する設定となり、この値が基準値となって以下の計算を行う。  
**単位は[cm] と書いてあるが本当は 単位は[m]であることに気を付けて下さい！**


<u>なお、これより以下の計算では、角度はラジアンで扱う点に注意する。</u>

- #### x軸方向(左右)の計算  
**式：$MoveX=targetPosX-targetDistance×\sin\(targetRotY)$**  
![移動経路計算](/images/Tello/AutoTracking/Calculation/figure2.png "移動経路計算")  
```C#
//x軸方向(左右)の計算
public static double CalculateRequestMoveX(double targetPosX, double targetRotY)
{
    // 引数は単位が[m]
    return targetPosX - targetDistance * Math.Sin(targetRotY * Mathf.Deg2Rad);
}
```

- #### y軸方向(前後)の計算  
**式：$MoveZ=targetPosZ-targetDistance×\cos\(targetRotY)$**  
![移動経路計算](/images/Tello/AutoTracking/Calculation/figure3.png "移動経路計算")  
```C#
//y軸方向(前後)の計算
public static double CalculateRequestMoveY(double targetPosZ, double targetRotY)
{
    // 引数は単位が[m]
    return targetPosZ - targetDistance * Math.Cos(targetRotY * Mathf.Deg2Rad);
}
```

- #### 旋回の計算  
Yの大きさによって旋回の向きを変える
```C#
public static double CalculateRequestRoteY(double targetRotY)
{
    return (targetRotY <= 180) ? targetRotY * Mathf.Deg2Rad : (targetRotY - 360) * Mathf.Deg2Rad;
}
```
