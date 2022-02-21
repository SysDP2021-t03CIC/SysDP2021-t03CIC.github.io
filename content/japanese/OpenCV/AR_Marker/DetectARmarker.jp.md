---
# 記事タイトルはこちら
title: "ARマーカーの検出"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","ARマーカー"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
---
#### カメラ映像を取り込む
OpenCV for UnityでARマーカーの検出を行う際、画像をMatクラスにする必要がある。<br>
基のシステムでTelloのカメラ映像をオブジェクトのTextureに変換して表示していたため、<br>
##### Texture →(TextureSender)→ Texture2D →(OpenCV for Unity)→ Mat<br>
という手順で変換している。
1. 画像をARマーカーの検出に送る用スクリプトを作成
2. 以下のようにコードを変更

このスクリプトでは、オブジェクトのTextureをTexture2Dに変換し、さらにARマーカー検出するEstimateメソッドを呼び出している。
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//独自の名前空間
using EstimateTargetTransform_OCVfU;

public class TextureSender : MonoBehaviour
{
    // CameraParaCal cameraPara;
    public EstimateTra estimater;
    Texture2D texture;
    Texture2D result;
    RenderTexture rt;
    Rect source;

    // Start is called before the first frame update
    void Start()
    {
        // cameraPara = new CameraParaCal();
        estimater = new EstimateTra();
    }
    public Texture2D ToTexture2D(Texture self)
    {
        var sw = self.width;
        var sh = self.height;
        var format = TextureFormat.RGBA32;
        result = new Texture2D(sw, sh, format, false);
        var currentRT = RenderTexture.active;
        rt = new RenderTexture(sw, sh, 32);
        Graphics.Blit(self, rt);
        RenderTexture.active = rt;
        source = new Rect(0, 0, rt.width, rt.height);
        result.ReadPixels(source, 0, 0);
        result.Apply();
        RenderTexture.active = currentRT;
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        texture = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
        estimater.Estimate(texture);
        Destroy(result);
        Destroy(rt);
    }
}
```
3. スクリーンとなるオブジェクトにアタッチ

#### 画像からARマーカーの検出
呼び出したEstimateメソッドでは、画像をTexture2DからMatへ変換しARマーカーの検出を行っている

```C#
public void Estimate(Texture2D texture)
    {
        Mat img = new Mat(texture.height, texture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(texture, img);
        /*
         * ARマーカーの検知
         * 6cm四方だと2mぐらいまではっきり検知可能
         */
        Aruco.detectMarkers(img, dictionary, corners, ids, parameters, rejectedImgPoints);
        /* 以下略 */
    }
```

- ###### ARマーカーを検出する
    ```C#
    Aruco.detectMarkers(/*画像*/, /*ARマーカーの辞書*/,
                        /*ARマーカーの角(Out)*/,
                        /*ARマーカーのID(Out)*/,
                        /*検出のパラメータ*/,
                        /*異常検出用(Out)*/)
    ```
    ※第5,6引数は使っていないため与えなくても問題なく動作するとは思う(試していない)。createメソッドで検出のパラメータを設定しているがリファレンスに何も書いていないため、おそらくデフォルトのまま