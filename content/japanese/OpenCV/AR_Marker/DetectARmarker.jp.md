---
# 記事タイトルはこちら
title: "ARマーカーの認識"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","ARマーカー"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
---
まず、次の内容のスクリプトを作成し、スクリーンのオブジェクトにアタッチ。
このスクリプトでは、オブジェクトのTextureをTexture2Dに変換している。
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

- EstimateTraクラスのメソッド内
    ```
    Aruco.detectMarkers(//撮影された画像, //ARマーカーの辞書, //ARマーカーの角(Out), //ARマーカーのID(Out), //使わないのでいらないかも, //使わないのでいらないかも)
    ```
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
        /* 以下略
         * ...
         */
    }
```