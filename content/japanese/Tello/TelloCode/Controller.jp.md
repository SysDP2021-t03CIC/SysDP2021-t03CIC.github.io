---
# 記事タイトルはこちら
title: "主となる制御のコード"
# 投稿日時をご記入ください
date: 2022-02-22T06:50:26+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["Tello","tello","ドローン","制御"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "ban"
weight: 1
---

#### 注意事項
スクリプト内にTelloControllerの名前のファイルが二つ存在する場合、使われているファイルは「TelloController1122.cs」であることに気を付ける。

#### ファイル内の大まかな説明
ドローンの移動の制御は「TelloController1122.cs」のUpdate内のswitch内で行われる。ドローンが自動で移動するサイクルは「Stop」,「TakeOff」,「TakingOff」,「Detect」,「Follow」,「Land」,「Landing」に分かれる。
- ##### 「Stop」
Telloが受けているコマンドの状態の初期化。  
Sキーが入力されると、「TakeOff」に移行する。  

- ##### 「TakeOff」
Telloが離陸する段階。  
takeoffのコマンドを送ったのち、「TakingOff」に移行する。

- ##### 「TakingOff」
Telloが離陸して上昇、旋回してARマーカーを探す段階。  
離陸自体は瞬時に終了するが、ARマーカーを探すために高度を合わせる。  
（waitTimerが3以上4以下の時）今回は追従対象が決まっていたため固定値分上昇させた。  
（waitTimerが4以上の時）ARマーカーを認識するまで旋回を続ける。認識に成功したとき、 「Follow」に移行する。 

- ##### 「Detect」
「Follow」に移行する役割。今回は未使用。

- ##### 「Follow」
Telloが追従していることを示す段階。この段階が続いている限りTelloは追従を続ける。  
Lキーが入力されると、「Land」に移行する。

- ##### 「Land」
Telloが着陸する段階。  
landのコマンドを送ったのち、「Landing」に移行する。

- ##### 「Landing」
Telloが着陸している段階。  
実質的にシステム終了の段階である。