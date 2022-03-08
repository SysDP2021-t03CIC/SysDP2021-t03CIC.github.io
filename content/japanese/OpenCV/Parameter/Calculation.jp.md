---
# 記事タイトルはこちら
title: "カメラ行列の計測"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","カメラパラメータ","カメラキャリブレーション","歪み"]
# 以下は著者名タグです。著者名の苗字をローマ字で記入してください。
Author: "Tsuchiya Koken"
---
#### カメラ行列・歪み係数の計測の方法
1. 以下の内容のスクリプトを作成。
```C#
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.UnityUtils;

using UnityEngine;

namespace EstimateTargetTransform_OCVfU
{

    public class CameraParaCal
    {
        private Mat img;
        private Mat gray;
        private Size crossNum;
        private List<Mat> objPoints;
        private List<Mat> imgPoints;
        private Mat pattern_points;
        
        public Mat cameraMatrix;
        public Mat distCoeffs;

        public double[] cameraArray;
        public double[] distArray;

        bool calculating;

        // Start is called before the first frame update
        public CameraParaCal()
        {
            gray = new Mat();

            objPoints = new List<Mat>();
            imgPoints = new List<Mat>();

            pattern_points = new Mat();
            cameraMatrix = new Mat();
            distCoeffs = new Mat();

            calculating = true;

            int w, h;
            float pattern_size = 2.0f;
            crossNum = new Size(7.0, 7.0);
            
            Point3[] a = new Point3[7 * 7];
            for (h = 0; h < 7; h++)
            {
                for (w = 0; w < 7; w++)
                {
                    a[h * 7 + w] = new Point3(w * pattern_size, h * pattern_size, 0);
                }
            }
            MatOfPoint3f b = new MatOfPoint3f(a);
            b.copyTo(pattern_points);
        }

        // Update is called once per frame
        public void DataSampling(Texture2D texture, int calcPoint = 40)
        {
            img = new Mat(texture.height, texture.width, CvType.CV_8UC3);
            Utils.texture2DToMat(texture, img);

            if (img == null) return;
            if (!calculating) return;
            
            CalcPara();
            Debug.Log("Sampling <=== " + objPoints.Count + " / " + calcPoint + " ===> ");

            System.Threading.Thread.Sleep(200);

            if (calculating && objPoints.Count.Equals(calcPoint))
            {
                Debug.Log("Calculating...");
                CaliCamera();
                calculating = false;

                cameraArray = new double[cameraMatrix.rows() * cameraMatrix.cols() * cameraMatrix.channels()];
                distArray = new double[distCoeffs.rows() * distCoeffs.cols() * distCoeffs.channels()];
                cameraMatrix.get(0, 0, cameraArray);
                distCoeffs.get(0, 0, distArray);

                int i, j;
                Debug.Log(" --- cameraMatrix --- ");
                for (i = 0; i < 3; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        // Debug.Log(cameraArray.Length);
                        Debug.Log(String.Format("{0:00000.00000000}, ", cameraArray[i * 3 + j]));
                    }
                }

                Debug.Log(" --- distMatrix --- ");
                for (i = 0; i < distArray.Length; i++)
                {
                    Debug.Log((String.Format("{0:00000.00000000},", distArray[i])));
                }
            }
        }

        private void CalcPara()
        {
            bool patternFound;

            Imgproc.cvtColor(img, gray, Imgproc.COLOR_BGR2GRAY);

            MatOfPoint2f corners = new MatOfPoint2f();

            patternFound = Calib3d.findChessboardCorners(gray, crossNum, corners);

            /*
             * corners
             * Channels : 2
             * Rows     : 49
             * Cols     : 1
             */

            if (patternFound)
            {
                TermCriteria term = new TermCriteria(TermCriteria.EPS | TermCriteria.COUNT, 30, 0.1);

                Imgproc.cornerSubPix(gray, corners, new Size(5, 5), new Size(-1, -1), term);
                imgPoints.Add(corners);
                objPoints.Add(pattern_points);
            }
        }

        private void CaliCamera()
        {
            List<Mat> rvecs = new List<Mat>();
            List<Mat> tvecs = new List<Mat>();
            Calib3d.calibrateCamera(objPoints, imgPoints, gray.size(), cameraMatrix, distCoeffs, rvecs, tvecs);
        }
    }
}
```
2. TextureSenderクラスのUpdateメソッドを以下のように書き換える。
```C#
public class TextureSender : MonoBehaviour
{
    CameraParaCal cameraPara;           //←追加
    // public EstimateTra estimater;    //←コメントアウト
    Texture2D texture;
    Texture2D result;
    RenderTexture rt;
    Rect source;

    // Start is called before the first frame update
    void Start()
    {
        cameraPara = new CameraParaCal();   //←追加
        // estimater = new EstimateTra();   //←コメントアウト
    }

    /*
     * 省略
     */
    
    void Update()
    {
        // 追加
        time += Time.deltaTime;
        if (time > 0.2f)
        {
            texture = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
            cameraPara.DataSampling(texture);
            time = 0.0f;
        }
        // 追加ここまで
        
        /* コメントアウト
        texture = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
        estimater.Estimate(texture);
        */
        Destroy(result);
        Destroy(rt);
    }
}
```
3. プログラムを実行し、カメラに以下の画像を向ける
(画像)
4. 指定した回数(上記プログラムだと40回)サンプリングできたらパラメータの計算が始まる<br>
    (かなり時間がかかる)。
5. 計算が終わるとパラメータの情報を出力するのでメモする。
6. サンプリングで誤差が出るので何度か(サンプリング→計算)を繰り返し、平均を出す。<br>
※1回のサンプリング数を増やすならば何度も繰り返す必要性は減る
