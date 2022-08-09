using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using TelloRequestCalculation02;

public class TelloController1122 : SingletonMonoBehaviour<TelloController1122>
{
    private static bool isLoaded = false;

    private TelloVideoTexture telloVideoTexture;

    public enum TelloCommandMode
    {
        Stop,

        TakeOff,

        TakingOff,

        Detect,

        Follow,

        Land,

        Landing
    };
    
    // 実験用
    private float moveX_perFrame;
    private float moveY_perFrame;
    private float rotaX_perFrame;
    // Telloが受けているコマンドの状態を表す
    private TelloCommandMode doingCommand;
    // 待ち時間が必要な時のタイマー
    private float waitTimer;
    private bool detected;

    private TextureSender textureSender;

    // FlipType is used for the various flips supported by the Tello.
    public enum FlipType
    {

        // FlipFront flips forward.
        FlipFront = 0,

        // FlipLeft flips left.
        FlipLeft = 1,

        // FlipBack flips backwards.
        FlipBack = 2,

        // FlipRight flips to the right.
        FlipRight = 3,

        // FlipForwardLeft flips forwards and to the left.
        FlipForwardLeft = 4,

        // FlipBackLeft flips backwards and to the left.
        FlipBackLeft = 5,

        // FlipBackRight flips backwards and to the right.
        FlipBackRight = 6,

        // FlipForwardRight flips forewards and to the right.
        FlipForwardRight = 7,
    };

    // VideoBitRate is used to set the bit rate for the streaming video returned by the Tello.
    public enum VideoBitRate
    {
        // VideoBitRateAuto sets the bitrate for streaming video to auto-adjust.
        VideoBitRateAuto = 0,

        // VideoBitRate1M sets the bitrate for streaming video to 1 Mb/s.
        VideoBitRate1M = 1,

        // VideoBitRate15M sets the bitrate for streaming video to 1.5 Mb/s
        VideoBitRate15M = 2,

        // VideoBitRate2M sets the bitrate for streaming video to 2 Mb/s.
        VideoBitRate2M = 3,

        // VideoBitRate3M sets the bitrate for streaming video to 3 Mb/s.
        VideoBitRate3M = 4,

        // VideoBitRate4M sets the bitrate for streaming video to 4 Mb/s.
        VideoBitRate4M = 5,

    };

    override protected void Awake()
    {
        //if (!isLoaded)
        //{
        //    DontDestroyOnLoad(this.gameObject);
        //    isLoaded = true;
        //}
        //base.Awake();

        //Tello.onConnection += Tello_onConnection;
        //Tello.onUpdate += Tello_onUpdate;
        //Tello.onVideoData += Tello_onVideoData;

        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();

    }

    private void OnEnable()
    {
        if (!isLoaded)
        {
            DontDestroyOnLoad(this.gameObject);
            isLoaded = true;
        }
        base.Awake();

        Tello.onConnection += Tello_onConnection;
        Tello.onUpdate += Tello_onUpdate;
        Tello.onVideoData += Tello_onVideoData;

        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
    }

    private void Start()
    {

        if (textureSender == null)
            textureSender = FindObjectOfType<TextureSender>();

        Tello.startConnecting();

        // Updateごとの移動量の設定
        moveX_perFrame = 1.0f;
        moveY_perFrame = 1.0f;
        rotaX_perFrame = 1.0f;

        // Telloが受けているコマンドの状態の初期化
        doingCommand = TelloCommandMode.Stop;

        detected = false;

        // 待機タイマーの初期化
        waitTimer = 0.0f;
    }

    void OnApplicationQuit()
    {
        Tello.stopConnecting();
    }

    private void SetCommand_DetectMode(ref float rx, ref float ry, ref float lx)
    {
        // 移動しない
        rx = 0.0f;
        ry = 0.0f;
        // 回転して探す
        if (textureSender.estimater.detected)
        {
            lx = 0.0f;
        }
        else
        {
            if (rotaX_perFrame <= 0)
            {
                lx = -0.5f;
            }
            else
            {
                lx = 0.5f;
            }
        }
    }

    private float MyarcsinFuncX(double x)
    {
        if (x < -1) x = -0.9f;
        if (x > 1) x = 0.9f;
        return (float)(4 * 2 * Math.Asin(x) / (9 * Math.PI) );
    }

    private float MyarcsinFuncY(double x)
    {
        if (x < -1) x = -0.9f;
        if (x > 1) x = 0.9f;
        return (float)(2 * Math.Asin(x) / (3 * Math.PI));
    }

    private float MysigmoidFunc(double x)
    {
        return (float)(2 / (1 + Math.Exp(-7.3f * x)) - 1);
    }
    

    private void AxisResize(double moveX, double moveY, double rotaX)
    {
        double moveAxisVecNorm = Math.Sqrt(moveX * moveX + moveY * moveY);
        moveX_perFrame = (moveAxisVecNorm.Equals(0.0)) ? 0.0f : (float)(moveX / moveAxisVecNorm);
        moveY_perFrame = (moveAxisVecNorm.Equals(0.0)) ? 0.0f : (float)(moveY / moveAxisVecNorm);

        moveX_perFrame = MyarcsinFuncX(moveX_perFrame);
        moveY_perFrame = MyarcsinFuncY(moveY_perFrame);

        rotaX_perFrame = MysigmoidFunc(rotaX / Math.PI);
    }

    private void SetCommand_FollowMode(ref float rx, ref float ry, ref float lx)
    {
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
        else
        {
            // 追従中に一定時間見失ったら検知に入る
            // 一定時間経ったか測る間は見失う直前の数値で動く
            waitTimer += Time.deltaTime;
            // 一定時間:3[s]
            if(waitTimer >= 3)
            {
                doingCommand = TelloCommandMode.Detect;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Tello.takeOff();
        }

        // Telloのコマンドの引数(1フレーム当たりの移動量[スティックの傾き] -1.0f ～ 1.0f )
        float lx = 0.0f;
        float ly = 0.0f;
        float rx = 0.0f;
        float ry = 0.0f;

        switch (doingCommand)
        {
            case TelloCommandMode.Stop:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    doingCommand = TelloCommandMode.TakeOff;
                }
                break;
            case TelloCommandMode.TakeOff:
                Tello.takeOff();
                doingCommand = TelloCommandMode.TakingOff;
                waitTimer = 0.0f;
                break;
            case TelloCommandMode.TakingOff:
                // 離陸コマンドから一定時間待機
                waitTimer += Time.deltaTime;
                // 一定時間:4[s]
                if (waitTimer > 4)
                {
                    rotaX_perFrame = 0.47f;
                    doingCommand = TelloCommandMode.Follow;
                }
                else if (waitTimer > 3)
                {
                    ly = 0.51f;
                }

                break;
            case TelloCommandMode.Detect:
                if (textureSender.estimater.detected)
                {   
                    doingCommand = TelloCommandMode.Follow;
                }
                SetCommand_DetectMode(ref rx, ref ry, ref lx);
                break;
            case TelloCommandMode.Follow:
                if (Input.GetKeyDown(KeyCode.L))
                {
                    doingCommand = TelloCommandMode.Land;
                }
                SetCommand_FollowMode(ref rx, ref ry, ref lx);
                break;
            case TelloCommandMode.Land:
                Tello.land();
                doingCommand = TelloCommandMode.Landing;
                break;
            case TelloCommandMode.Landing:
            default:
                break;
        }

        Debug.Log(doingCommand);
        
        /*
        if (Input.GetKey(KeyCode.UpArrow)) {
        	ry = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
        	ry = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
        	rx = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
        	rx = -1;
        }
        if (Input.GetKey(KeyCode.W)) {
        	ly = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
        	ly = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
        	lx = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
        	lx = -1;
        }
        */

        Tello.controllerState.setAxis(lx, ly, rx, ry);
        if (Input.GetKey(KeyCode.L))
        {
            Tello.land();
        }
    }

    private void Tello_onUpdate(int cmdId)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Tello_onUpdate : " + Tello.state);
    }

    private void Tello_onConnection(Tello.ConnectionState newState)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Tello_onConnection : " + newState);
        if (newState == Tello.ConnectionState.Connected)
        {
            Tello.queryAttAngle();
            Tello.setMaxHeight(50);

            Tello.setPicVidMode(1); // 0: picture, 1: video
            Tello.setVideoBitRate((int)VideoBitRate.VideoBitRateAuto);
            //Tello.setEV(0);
            Tello.requestIframe();
        }
    }

    private void Tello_onVideoData(byte[] data)
    {
        //Debug.Log("Tello_onVideoData: " + data.Length);
        if (telloVideoTexture != null)
            telloVideoTexture.PutVideoData(data);
    }

    public void Tello_TakeOff()
    {
        Debug.Log("takeoff");
        doingCommand = TelloCommandMode.TakeOff;
    }
}
