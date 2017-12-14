using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardManager
{
    readonly int[] points = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public int[] randomIndexArray;
    public int step { get; private set; }

    public void Start()
    {
        // ランダム配列作成
        randomIndexArray = oulRandom.GetRandomArray(0, points.Length);
        // ステップ初期化
        step = -1;
    }
    public int GetCurrentPoint(){ return (step < points.Length) ? points[randomIndexArray[step]] : 0; }
    // 次のポイント(山札が0だったらtrueを返す)
    public bool Next()
    {
        return (++step >= points.Length);
    }
}
