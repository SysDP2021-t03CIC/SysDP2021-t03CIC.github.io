using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TelloRequestCalculation02
{
    public static class RequestCalculation
    {
        // 単位は[cm]
        private static double targetDistance = 2;

        public static double Get_targetDistance()
        {
            return targetDistance;
        }

        public static void Set_targetDistance(double distance)
        {
            targetDistance = distance;
        }

        //x軸方向(左右)の計算
        public static double CalculateRequestMoveX(double targetPosX, double targetRotY)
        {
            // 引数は単位が[m]
            return targetPosX - targetDistance * Math.Sin(targetRotY * Mathf.Deg2Rad);
        }

        //y軸方向(前後)の計算
        public static double CalculateRequestMoveY(double targetPosZ, double targetRotY)
        {
            // 引数は単位が[m]
            return targetPosZ - targetDistance * Math.Cos(targetRotY * Mathf.Deg2Rad);
        }

        public static double CalculateRequestRoteY(double targetRotY)
        {
            return (targetRotY <= 180) ? targetRotY * Mathf.Deg2Rad : (targetRotY - 360) * Mathf.Deg2Rad;
        }
    }
}
