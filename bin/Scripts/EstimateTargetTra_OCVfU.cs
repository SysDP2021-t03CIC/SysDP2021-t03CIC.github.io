using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine.UI;

namespace EstimateTargetTransform_OCVfU
{
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

        public void SetrArr(Mat rVec)
        {
            rArr = new double[3];
            rVec.get(0, 0, rArr);
        }
    }

    public class EstimateTra
    {
        private Mat ids, cameraMat, distMat;

        private Dictionary dictionary;
        private List<Mat> corners, rejectedImgPoints;
        private DetectorParameters parameters;

        private int[] idsArr;
        
        // ドローン班に渡す値
        public double[] position;
        public double[] rotation;
        public double[] angle;
        public double distance;
        public bool detected;

        public EstimateTra()
        {
            ids = new Mat();
            corners = new List<Mat>();
            rejectedImgPoints = new List<Mat>();

            
            double[] cameraArray = {1441.40498452F,              0, 990.85546633F,
                                                0F, 1437.85028357F, 561.56048518F,
                                                0F,             0F,            1F};


            double[] distArray = { 0.12832571F, 0.58193728F, 0.00148021F, 0.01196757F, 2.06316664F };


            cameraMat = new Mat(3, 3, CvType.CV_64FC1);
            cameraMat.put(0, 0, cameraArray);

            distMat = new Mat(1, 5, CvType.CV_64FC1);
            distMat.put(0, 0, distArray);

            // 辞書の名前 4×4_50
            dictionary = Aruco.getPredefinedDictionary(Aruco.DICT_4X4_100);

            //
            parameters = DetectorParameters.create();

            // ドローン班に渡す値の初期化
            position = new double[3];
            rotation = new double[3];
            angle = new double[2];
            distance = 0;
        }

        // Update is called once per frame
        public void Estimate(Texture2D texture)
        {
            int idsNum;
            Mat img = new Mat(texture.height, texture.width, CvType.CV_8UC3);
            Utils.texture2DToMat(texture, img);
            /*
             * ARマーカーの検知
             * 6cm四方だと2mぐらいまではっきり検知可能
             */

            Aruco.detectMarkers(img, dictionary, corners, ids, parameters, rejectedImgPoints);

            /*
             * ids
             * channels : 1
             * rows     : 検知したARマーカーの数
             * cols     : 1
             */

            idsNum = ids.rows();
            if (idsNum > 0)
            {
                idsArr = new int[idsNum];
                ids.get(0, 0, idsArr);
                detected = true;

                EstiamtePoseMarkers();
                // Debug.Log(String.Format("rotation : {0}, {1}, {2}", rotation[0], rotation[1], rotation[2]));
                // Debug.Log(String.Format("position : {0}, {1}, {2}", position[0], position[1], position[2]));
                Debug.Log(detected);
            }
            else
            {
                detected = false;
            }
        }

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
                markers[i].SetrArr(rVecs.row(i));
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
                Core.hconcat(src, projMat);
                
                // オイラー角以外で必要のない値も受け取る必要があるので宣言しておく
                Mat cameraMatrix = new Mat(),  rotMatrixX = new Mat(), rotMatrixY = new Mat(), rotMatrixZ = new Mat(), eulerAngles = new Mat();

                /* オイラー角への変換
                 * projMatrix   : input  : 3×4 Mat
                 * cameraMatrix : output : 3×3 Mat
                 * rotMatrix    : output : 3×3 Mat
                 * transVect    : output : 4×1 Mat
                 * eulerAngles  : output : 3×1 Mat
                 */
                Calib3d.decomposeProjectionMatrix(projMat, cameraMatrix, rMat, _tVec, rotMatrixX, rotMatrixY, rotMatrixZ, eulerAngles);

                /*
                 * eulerAngles
                 * channels : 1
                 * rows     : 3
                 * cols     : 1
                 */
                 
                // i番目のARマーカーの回転ベクトルを格納
                markers[i].SetrArr(eulerAngles);
            }
            CalcTargetRotation(markers);
        }

        private void CalcTargetRotation(ARMarker[] markers)
        {
            if (markers.Length < 1) return;

            Array.Clear(position, 0, position.Length);
            Array.Clear(rotation, 0, rotation.Length);

            int i, j;
            for (i = 0; i < markers.Length; i++)
            {
                for (j = 0; j < markers[i].rArr.Length; j++)
                {
                    if (markers[i].ID == 0 && markers[i].rArr[j] < 0) markers[i].rArr[j] += 360;
                    rotation[j] += markers[i].ID * 90 + markers[i].rArr[j];
                }
                for (j = 0; j < markers[i].tArr.Length; j++)
                {
                    position[j] += markers[i].tArr[j];
                }
            }

            for (j = 0; j < position.Length; j++) position[j] /= markers.Length;
            for (j = 0; j < rotation.Length; j++) rotation[j] /= markers.Length;

            if (position[2] == 0) return;
            angle[0] = Math.Atan2(position[0], position[2]) * (180 / Math.PI);
            angle[1] = Math.Atan2(position[1], position[2]) * (180 / Math.PI);

            distance = Math.Sqrt(Math.Pow(position[0], 2) + Math.Pow(position[1], 2) + Math.Pow(position[2], 2));
        }
        
        private void ShowMatPara(Mat m)
        {
            Debug.Log(String.Format("this mat : ch {0}, rows {1}, cols {2}, type {3}", m.channels(), m.rows(), m.cols(), CvType.typeToString(m.type())));
        }
    }
}
