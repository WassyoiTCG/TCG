using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaMove : BaseAnim2D
{

    //+ Unity場で変更する値---------------------

    //[Range(1, 120)]
    public int iEndFrame = 180;

    // [Range(0.0f, 10.0f)]
    //public float fAlpha;

    //[Range(-0.1f, 0.1f)]
    public int iArrivalFrame = 60;
    public int iVanishFrame = 120;

    //+ メンバ変数---------------------
    //private Vector3 vShakePower = new Vector3(0.0f, 0.0f, 0.0f);
     private float fOrgAlpha = 0.0f;// 初期保存


    // 初期化
    protected override void Awake()
    {
        base.Awake();

        // 初期保存
        fOrgAlpha = m_pImage.color.a;
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
        {

            gameObject.SetActive(true);
        }



        // エンドフレームまで来たら終わる
        m_iCurrentFrame++;
        if (m_iCurrentFrame >= iEndFrame)
        {
            m_bActionFlag = false;
            m_bEndFlag = true; // 終りフラグON

            //vOrgPos // (TOODO)初期座標にもどす？
        }
        else
        {
            float lAlpha = 0.0f;

            // Alpha調整
            if (m_iCurrentFrame <= iArrivalFrame )
            {
                lAlpha = (float)m_iCurrentFrame / (float)iArrivalFrame;

                var newColor = m_pImage.color;
                newColor.a = lAlpha * fOrgAlpha;
                m_pImage.color = newColor;

                if (m_pImage.color.a >= 1.0f) SetAlpha(1);

            }
            else if (m_iCurrentFrame >= iVanishFrame)
            {
                if (m_bRoop == true)
                {
                    // 
                    m_iCurrentFrame = iArrivalFrame;
                    return;
                }

                float l_fParam = (float)(iEndFrame - m_iCurrentFrame);
                float l_fMax = (float)(iEndFrame - iVanishFrame);
                float l_fAlpha = l_fParam / l_fMax;
                var newColor = m_pImage.color;
                newColor.a = l_fAlpha * fOrgAlpha;
                m_pImage.color = newColor;

                if (m_pImage.color.a <= 0.0f) SetAlpha(0);

            }
            else
            {
                // 真ん中は何もしない
                //pic->SetARGB((int)(255), 255, 255, 255);
            }

      

        }

    }


    public override void Action()
    {
        base.Action();

        // 初期保存
        //fOrgAlpha = m_pImage.color.a;
     
           SetAlpha(0);
    }

    public override void ActionRoop()
    {
        base.ActionRoop();

        // 初期保存
        //fOrgAlpha = m_pImage.color.a;

        SetAlpha(0);
    }

    public override void StopRoop()
    {
        m_iCurrentFrame = iVanishFrame;
    }

    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;

        // このクラスは描画も止める
        gameObject.SetActive(false);

    }

    public float GetOrgAlpha() { return fOrgAlpha; }


}
