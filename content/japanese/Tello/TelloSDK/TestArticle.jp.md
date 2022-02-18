---
# 記事タイトルはこちら
title: "TelloSDKについて"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["tello","SDK"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "ban"
---

#### TelloSDK
基本的に以下のサイトを見ればわかる。
> [ニート翔ぶ～C#でドローンを飛ばす～](https://qiita.com/mima_ita/items/f241a519baec6df505d2)  
  
##### 主なTelloコマンド
- command  
    SDKモードに入る（1番最初に命令するコマンド）
- takeoff  
    Telloが離陸する
- land  
    Telloが着陸する
- emergency  
    全てのモーターが止まる（緊急着陸）
- streamon  
    ビデオストリームをON
- streamoff  
    ビデオストリームをOFF
- up x  
    x cm上昇
- down x  
    x cm下降
- left x  
    x cm左へ
- right x  
    x cm右へ
- forward x  
    x cm前進
- back x  
    x cm後退
- cw x  
    x 度時計回りに旋回
- ccw x  
    x 度反時計回りに旋回

##### 注意事項
- 15秒間Telloになにも命令が送られないと、自動で着陸する。
- 途中で命令が聞かなくなった場合はバッテリー切れを待つか、他のPCからWi-fiをつなぎ直して命令を送ることで命令が送れる時がある。