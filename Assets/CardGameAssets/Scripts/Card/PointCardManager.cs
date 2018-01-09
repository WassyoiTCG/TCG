using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCardManager
{
    readonly int[] points = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public int[] randomIndexArray;
    public int step { get; private set; }

    // 要素変換
    void ChangeIndex(int i, int index)
    {

        int targetIndex = 0;
        // ↑の要素を持っている配列を探すチェック
        for (int i2 = 0; i2 < randomIndexArray.Length; i2++)
        {
            // インデックスが同じ
            if ( randomIndexArray[i2] == index)
            {
                targetIndex = i2;
                break;
            }

        }
        
        // テンプ処理
        int temp = randomIndexArray[i];
        randomIndexArray[i] = randomIndexArray[targetIndex];
        randomIndexArray[targetIndex] = temp;

    }

    public void Start()
    {
        // ランダム配列作成
        randomIndexArray = oulRandom.GetRandomArray(0, points.Length);


        // [NEW]ランダムを一塩補正
        for (int i = 0; i < points.Length; i++)
        {
            int randomSeed = Random.Range(0, 100);

            switch (i)
            {
                case 0:
                case 1:
                case 2:
                    {
                        // 前半戦

                        // 75%の確率で　40 50 60に
                        if (randomSeed <= 80)
                        {
                            int ram = Random.Range(2, 6);
                            ChangeIndex(i, ram);

                        }

                    }
                    break;
                case 3:
                case 4:
                case 5:
                    // 中盤戦
                    { 
                    
                    
                    
                    }
                    break;
                default:
                    // 終盤
                    {




                    }
                    break;
            }



        }



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
