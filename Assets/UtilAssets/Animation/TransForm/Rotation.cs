using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : BaseAnim2D
{

    //+ Unity場で変更する値---------------------
    //[Range(0.0f, 1.0f)]
    //public float fSpeed = 0.3f;

    public Vector3 vNextRotation = new Vector3(0.0f, 0.0f, 0.0f);     // 目標の大きさ

    [Range(1, 120)]
    public int iEndFrame = 18;

    private Vector3 vDifRotation = new Vector3(0.0f, 0.0f, 0.0f);

    [Range(0.0f, 3.0f)]
    public float fPow = 1.0f; // 何乗するか？

    //+-------------------------------

    //+ メンバ変数---------------------
    // public float m_fRate = 0.0f;
    //+-------------------------------

    // 初期化
    protected override void Awake()
    {
        base.Awake();

    }

    // Update is called once per frame
    public override void SelfUpdate()
    {
        // アクションフラグがたっていないと返す
        if (!ActionCheck()) return;

        // 現在の回転
        //Vector3 vCurrentRotation = transform.localEulerAngles;

        //vNextRotation
        //Quaternion vQCurrentRotation = transform.localRotation;
        //Vector3 vCurrentRotation = new Vector3(vQCurrentRotation.x, vQCurrentRotation.y,vQCurrentRotation.z);

        // エンドフレームまで来たら終わる
        m_iCurrentFrame++;
        if (m_iCurrentFrame >= iEndFrame)
        {
            m_bActionFlag = false;
            m_bEndFlag = true; // 終りフラグON
            
        }

        // 現在のフレームと過去のフレームの補間
        float fRate = (float)m_iCurrentFrame / (float)iEndFrame;
        fRate = Mathf.Pow(fRate, fPow);

        // 場所補間の更新
        //if (fSpeed > 1.0f)
        //{
        //    Debug.LogWarning("Rotation: Speedが1より上。");
        //}fRate
        //float fPrevSpeedParam = 1.0f - fSpeed;

        Vector3 vCurrentRotation = new Vector3(0, 0, 0);
        vCurrentRotation = vNextRotation * fRate;// + vDifRotation * (1.0f - fRate);

        // 差分
        Vector3 vAddRotate = vCurrentRotation - vDifRotation;

        // 差分用に保存
        vDifRotation = vCurrentRotation;

        // 回転更新
        transform.Rotate(vAddRotate);
        
    }

    public override void Action()
    {
        base.Action();

        // 初期化
        vDifRotation.Set(0.0f, 0.0f, 0.0f);

    }

    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;
        // このクラスは描画も止める
        gameObject.SetActive(false);
    }

    // 回転を渡す
    public Vector3 GetNextRotation()
    {
        return vNextRotation;
    }

    // 回転を設定
    public void SetNextRotation(Vector3 vRotation)
    {
        vNextRotation = vRotation;
    }

    //// 速度
    //public float GetSpeed()
    //{
    //    return fSpeed;
    //}

    //public void SetSpeed(float fRate)
    //{
    //    fSpeed = fRate;
    //}

}
