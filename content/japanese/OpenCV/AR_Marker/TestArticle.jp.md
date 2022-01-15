---
# 記事タイトルはこちら
title: "ARマーカーの作成"
# 投稿日時をご記入ください
date: 2021-12-25T08:59:39+09:00
# trueにすると下書きとして非表示。falseにすると記事が表示されます。
draft: false
# 以下は検索用のタグです。適宜書き換えてください
keywords: ["画像処理","ARマーカー"]
---
#### ARマーカーの作成方法(C#, Unity)
1. 新しいUnityプロジェクトを作成
2. OpenCVforUnityをインポート
3. ARマーカー作成用のスクリプトを作成
4. 以下のようにコードを変更
    ```C#
     //ここから
    using OpenCVForUnity.CoreModule;
    using OpenCVForUnity.ArucoModule;
    using OpenCVForUnity.ImgcodecsModule;
    //ここまでが追加する名前空間
    using UnityEngine;

    public class CreateARmarker : MonoBehaviour
    {
        Dictionary ARdict;
        Mat ARmat;
        bool isCreated;

        // Start is called before the first frame update
        void Start()
        {
            ARdict = Aruco.getPredefinedDictionary(Aruco.DICT_4X4_50);
            ARmat = new Mat();

            // nの値が作成するARマーカーの枚数
            // iの値が作成するARマーカーのID
            int n = 4, i;
            for (i = 0; i < n; i++)
            {
                // ARマーカーの画像情報をARmatに格納
                Aruco.drawMarker(ARdict, i, 150, ARmat);
                string fileName = "id_" + i.ToString() + ".png";
                // ARマーカーの画像ファイルを作成
                isCreated = Imgcodecs.imwrite("Assets/ARmarkers/" + fileName, ARmat);
                //  確認用ログ
                if (isCreated) Debug.Log(string.Format("create {0}", i));
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
    ```
5. 空のオブジェクトを作成。そのオブジェクトに作成したスクリプトをアタッチ
6. Assetsフォルダに「ARmarkers」という名前のフォルダを作成
7. 実行 → 指定したフォルダに画像ファイルが生成されている
8. 印刷

※OpenCV for Unity試用版ではARマーカーにロゴが入ってしまう。