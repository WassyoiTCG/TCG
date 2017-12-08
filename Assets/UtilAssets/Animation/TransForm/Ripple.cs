using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ripple : BaseAnim2D
{

    //+ Unity場で変更する値---------------------

    [Range(1, 120)]
    public int iEndFrame = 18;

    [Range(0.0f, 10.0f)]
    public float fStartScale = 0;

    [Range(-0.2f, 0.2f)]
    public float fAddScale = 0.05f;// 広がる力

    //private float fAddScale = 0.0f;
 
       //+-------------------------------

    //+ メンバ変数---------------------
    // public float m_fRate = 0.0f;
    //+-------------------------------

    // 初期化
    protected override void Awake()
    {
        base.Awake();
        
        // 透明に
        SetAlpha(.0f);
    }

    // Update is called once per frame
    public override void SelfUpdate()
    {
        // アクションフラグがたっていないと返す
        if (!ActionCheck())
        {
            gameObject.SetActive(false);
            return; // 更新させない
        }
        else
        {   // (仮)透明度　を触るなら↓はコメントアウトしよう
            //SetAlpha(1.0f);
            gameObject.SetActive(true);
        }

        // エンドフレームまで来たら終わる
        m_iCurrentFrame++;
        if (m_iCurrentFrame >= iEndFrame)
        {
            // ループ中なら初期に戻り∞ループ 
            if (m_bRoop == true)
            {
                m_iCurrentFrame = 0;
            }else 
            {
                m_bActionFlag = false;
                m_bEndFlag = true; // 終りフラグON
            }

        }


        // アルファ処理
        float alpha = (float)m_iCurrentFrame / (float)iEndFrame;//
        alpha = 1.0f - alpha;
        //gameObject.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
        SetAlpha(alpha);

        // 拡大率更新
        gameObject.transform.localScale += new Vector3(fAddScale, fAddScale, fAddScale);
    }

    public override void Action()
    {
        base.Action();

        // ★↑でActive状態時に
        // 情報を変えてから描画させないように消す
        gameObject.SetActive(false);

        // 初期スケール
        m_pImage.transform.localScale =
         new Vector3(fStartScale, fStartScale, fStartScale);
         
        // 透明に
        //SetAlpha(0.0f);
        // 
        //gameObject.SetActive(false);
    }

    public override void ActionRoop()
    {
        base.ActionRoop();

        // ★↑でActive状態時に
        // 情報を変えてから描画させないように消す
        gameObject.SetActive(false);

        // 初期スケール
        m_pImage.transform.localScale =
         new Vector3(fStartScale, fStartScale, fStartScale);
         
    }

    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;

        // このクラスは描画も止める
        gameObject.SetActive(false);
        
    }

    // ループ止める
    public override void StopRoop()
    {
        m_bActionFlag = false;

        // このクラスは描画も止める
        gameObject.SetActive(false);

        m_bRoop = false;
    }


}
