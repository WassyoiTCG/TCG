using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAppeared : BaseAnim2D
{

    //+ Unity場で変更する値---------------------
    [Range(0.0f, 1.0f)]
    public float fSpeed = 0.3f;
    //[Range(0.0f, 10.0f)]
    public Vector3 vNextScale = new Vector3(0.0f, 0.0f, 0.0f);     // 目標の大きさ
  
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

        // 現在のスケール
        Vector3 vCurrentScale = m_pImage.transform.localScale;

        // 場所補間の更新
        if (fSpeed > 1.0f)
        {
            Debug.LogWarning("ScreenOutAppeared: Speedが1より上。");
        }
        float fPrevSpeedParam = 1.0f - fSpeed;
        vCurrentScale = vNextScale * fSpeed + vCurrentScale * fPrevSpeedParam;

        // 0.1以下の誤差は無くす処理
        if (0.9f >= Mathf.Abs(vCurrentScale.x - vNextScale.x) &&
            0.9f >= Mathf.Abs(vCurrentScale.y - vNextScale.y) &&
            0.9f >= Mathf.Abs(vCurrentScale.z - vNextScale.z))
        {
            vCurrentScale = vNextScale;

            m_bActionFlag = false;
            m_bEndFlag = true; // 終りフラグON
        }

        // 画像のスケール更新！
        m_pImage.transform.localScale = vCurrentScale;
        
    }

    public override void Action()
    {
        base.Action();

        // m_fRate = 0.0f;
        // SetAlpha(0);

    }

    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;
        // このクラスは描画も止める
        gameObject.SetActive(false);
    }

    // スケールを渡す
    public Vector3 GetNextScale()
    {
        return vNextScale;
    }

    // スケールを設定
    public void SetNextScale(Vector3 vScale)
    {
        vNextScale = vScale;
    }

    // 速度
    public float GetSpeed()
    {
        return fSpeed;
    }

    public void SetSpeed(float fRate)
    {
        fSpeed = fRate;
    }

}
