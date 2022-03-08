---
# 記事タイトルはこちら
title: "ARマーカーの値の取得"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理", "推定", "追従対象", "認識"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
# 以下は目次において何番目に表示するか決めるタグです。
weight: 1
---
#### 1. IDの取得

```C#
ids.rows()
```

で検出できたIDの個数を取得。個数が0個より多いとき、各ARマーカーの座標・回転を取得

```C#
// IDの数が検出できたARマーカーの数
idsNum = ids.rows();
if (idsNum > 0)
{
    // 各ARマーカーのIDを格納
    idsArr = new int[idsNum];
    ids.get(0, 0, idsArr);  // ← ここで検出したARマーカーのIDを配列に
    detected = true;
    EstiamtePoseMarkers();
}
```

#### 2. 各ARマーカーの座標・回転を取得
- ##### 検出した各ARマーカーの平行移動・回転ベクトルを計算
```C#
Aruco.estimatePoseSingleMarkers(/*各ARマーカーの角の画像座標*/,
                                /*ARマーカー1辺の長さ(m)*/,
                                /*内部行列*/,
                                /*歪み係数*/,
                                /*各ARマーカーの回転ベクトル(Out)*/,
                                /*各ARマーカーの平行移動ベクトル(Out)*/)
```
で平行移動・回転ベクトルを取得。<br>
その後、rVecs,tVecsを計算しやすい配列にし、回転ベクトルをオイラー角に変換している。<br>
また、追従対象の情報を計算しやすいように構造体を定義している。
```C#
public struct ARMarker
{
    public int ID;
    public double[] tArr;
    public double[] rArr;
    
    public void Set_tArr(Mat tVec)
    {
        tArr = new double[3];
        tVec.get(0, 0, tArr);
    }

    public void Set_rArr(Mat rVec)
    {
        rArr = new double[3];
        rVec.get(0, 0, rArr);
    }
}

// 省略

private void EstiamtePoseMarkers(float markerLength = 0.06F)
{
    if (idsArr.Length < 1) return;

    Mat rVecs = new Mat(), tVecs = new Mat();

    // tVecs は 要素数が1、チャンネル数が3で得られる
    // rVecs も同様に 要素数が1、チャンネル数が3で得られる
    Aruco.estimatePoseSingleMarkers(corners, markerLength, cameraMat, distMat, rVecs, tVecs);

    /*
        * rVecs    : CV_64FC3
        * channels : 3
        * rows     : 検知したARマーカーの数
        * cols     : 1
        */

    /*
        * tVecs    : CV_64FC3
        * channels : 3
        * rows     : 検知したARマーカーの数
        * cols     : 1
        */

    ARMarker[] markers = new ARMarker[idsArr.Length];

    int i;
    for (i = 0; i < idsArr.Length; i++)
    {
        // IDを格納
        markers[i].ID = idsArr[i];

        // i番目のARマーカーの位置ベクトルを格納
        markers[i].Set_tArr(tVecs.row(i));
        // i番目のARマーカーの位置ベクトルを取り出してベクトルの形にする
        Mat _tVec = new Mat(3, 1, CvType.CV_64FC1);
        _tVec.put(0, 0, markers[i].tArr);

        // 一旦i番目のARマーカーの回転ベクトルを格納
        markers[i].Set_rArr(rVecs.row(i));
        // i番目のARマーカーの回転ベクトルを取り出してベクトルの形にする
        Mat _rVec = new Mat(3, 1, CvType.CV_64FC1);
        _rVec.put(0, 0, markers[i].rArr);

        // 回転ベクトルから回転行列への変換を行う
        // rmatrix 3×3の回転行列
        Mat rMat = new Mat();
        Calib3d.Rodrigues(_rVec, rMat);

        /*
         * rMat
         * channels : 1
         * rows     : 3
         * cols     : 3
         */

        // 行列の拡大
        Mat projMat = new Mat();
        List<Mat> src = new List<Mat> { rMat, new Mat(3,1,CvType.CV_64FC1,new Scalar(0)) };
        // rMat(3×3) → projMat(3×4)へ変換
        Core.hconcat(src, projMat);
        
        // オイラー角以外で必要のない値も受け取る必要があるので宣言しておく
        Mat cameraMatrix = new Mat(),  rotMatrixX = new Mat(), rotMatrixY = new Mat(), rotMatrixZ = new Mat(), eulerAngles = new Mat();

        /* オイラー角への変換
         * projMatrix   : input  : 3×4 Mat
         * cameraMatrix : output : 3×3 Mat
         * rotMatrix    : output : 3×3 Mat
         * transVect    : output : 4×1 Mat
         * eulerAngles  : output : 3×1 Mat ← この出力だけ必要
         */
        Calib3d.decomposeProjectionMatrix(projMat, cameraMatrix, rMat, _tVec, rotMatrixX, rotMatrixY, rotMatrixZ, eulerAngles);

        /*
            * eulerAngles
            * channels : 1
            * rows     : 3
            * cols     : 1
            */
            
        // i番目のARマーカーの回転ベクトルを格納
        markers[i].Set_rArr(eulerAngles);
    }
    CalcTargetRotation(markers);
}
```