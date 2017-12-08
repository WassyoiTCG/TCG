using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOutAppeared : BaseAnim2D
{

    //+ Unity場で変更する値---------------------
    [Range(0.0f, 1.0f)]
    public float fSpeed = 0.3f;
    private Vector3 vNextPos = new Vector3(0.0f, 0.0f, 0.0f);// 元の値に足した先の場所
    public Vector3 vNextVector = new Vector3(0.0f, 0.0f, 0.0f);     // 元の値のベクトル
    private Vector3 vAwakePos = new Vector3(0.0f, 0.0f, 0.0f);
    //+-------------------------------

    //+ メンバ変数---------------------
    // public float m_fRate = 0.0f;
    //+-------------------------------

    // 初期化
    protected override void Awake()
    {
        base.Awake();

        // ベクトルだけ先に保存しておく
        //vNextVector = vAddNextPos;
        vAwakePos = m_pImage.transform.localPosition;// 初期座標保存
        vNextPos = m_pImage.transform.localPosition + vNextVector;
    }

    // Update is called once per frame
    public override void SelfUpdate()
    {
        // アクションフラグがたっていないと返す
        if (!ActionCheck()) return;


        Vector3 vCurrentPos = m_pImage.transform.localPosition;

        // 場所補間の更新
        if (fSpeed > 1.0f)
        {
            Debug.LogWarning("ScreenOutAppeared: Speedが1より上。");
        }
        float fPrevSpeedParam = 1.0f - fSpeed;
        vCurrentPos = vNextPos * fSpeed + vCurrentPos * fPrevSpeedParam;

        // 0.1以下の誤差は無くす処理
        if (0.1f >= Mathf.Abs(vCurrentPos.x - vNextPos.x) &&
            0.1f >= Mathf.Abs(vCurrentPos.y - vNextPos.y) &&
            0.1f >= Mathf.Abs(vCurrentPos.z - vNextPos.z))
        {
            vCurrentPos = vNextPos;

            m_bActionFlag = false;
            m_bEndFlag = true; // 終りフラグON
        }

        // 画像の移動地更新！
        m_pImage.transform.localPosition = vCurrentPos;

        // フレーム更新
        // エンドフレームまで来たら終わる
        //m_iCurrentFrame++;
        //if (m_iCurrentFrame >= m_iEndFrame)
        //{
        //    m_bActionFlag = false;
        //    m_iCurrentFrame = m_iEndFrame;
        //    m_bEndFlag = true; // 終りフラグON

        //    // Unityでのイラスト消す
        //    //gameObject.SetActive(false);
        //}

        // // アルファ処理
        // float rate;
        // float alpha;

        //// rate = (float)m_iCurrentFrame / (float)m_iEndFrame;

        // float startAlpha = 0.0f;
        // float endAlpha = 1.0f;
        //// alpha = Mathf.Lerp(startAlpha, endAlpha, rate);

        // SetAlpha(alpha);
    }

    public override void Action()
    {
        base.Action();

        // Appeared系は絶対Actionしても座標初期化するな。
        // やるならAction前にSetで書き換える 
        
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

    // ポジションを変えたのち進むベクトルも再計算
    public void SetPosReCalcNextPos(Vector3 vPos)
    {

        m_pImage.transform.localPosition = vPos;
        vNextPos = m_pImage.transform.localPosition + vNextVector;

    }

    // ベクトルを変えたのち進むベクトルも再計算
    public void SetNextVectorReCalcNextPos(Vector3 vVec)
    {
        vNextVector = vVec;
        vNextPos = m_pImage.transform.localPosition + vNextVector;

    }

    // 行先を渡す
    public Vector3 GetNextPos()
    {
        return vNextPos;
    }

    // 行先直接変える
    public void SetNextPos(Vector3 vPos)
    {
        vNextPos = vPos;
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

    // 初期座標を渡す
    public Vector3 GetAwakPos()
    {
        return vAwakePos;
    }
}
