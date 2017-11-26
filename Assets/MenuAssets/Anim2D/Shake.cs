using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shake : BaseAnim2D
{

    //+ Unity場で変更する値---------------------

    //[Range(1, 120)]
    public int iEndFrame = 18;

    // [Range(0.0f, 10.0f)]
    public Vector3 vShakeRange = new Vector3(0.0f, 0.0f, 0.0f);
    
    //[Range(-0.1f, 0.1f)]
    public int iCycle = 12;// サイクル

    //+ メンバ変数---------------------
    //private Vector3 vShakePower = new Vector3(0.0f, 0.0f, 0.0f);
    private bool bTrunOver = false;
    private Vector3 vOrgPos = new Vector3(0.0f, 0.0f, 0.0f);// 初期座標保存

    // 初期化
    protected override void Awake()
    {
        base.Awake();

        // 初期座標保存
        vOrgPos = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    public override void SelfUpdate()
    {
        // アクションフラグがたっていないと返す
        if (!ActionCheck())
        {
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
            // 0になったらフラグチェンジ！
            if (m_iCurrentFrame % iCycle == 0)
            {
                //TrunOver == true) ? bTrunOver = false: bTrunOver = true; 
                if (bTrunOver)
                {
                    bTrunOver = false;
                }
                else
                {
                    bTrunOver = true;
                }

            }

            // サイクルで割った数
            Vector3 vShakePower = new Vector3(vShakeRange.x / (float)iCycle, vShakeRange.y / (float)iCycle, vShakeRange.z / (float)iCycle);


            //+-------------------------------------------
            // 更新
            if (bTrunOver)
            {
                // ポジション更新
                gameObject.transform.localPosition -= vShakePower;

                //if (vShakeRange.x <= 0) vShakeRange.x = 0;
                //if (vShakeRange.y <= 0) vShakeRange.y = 0;
                //if (vShakeRange.z <= 0) vShakeRange.z = 0;
            }
            else
            {
                // ポジション更新
                gameObject.transform.localPosition += vShakePower;

               // if (vShakeRange.x <= 0) vShakeRange.x = 0;
               // if (vShakeRange.y <= 0) vShakeRange.y = 0;
               // if (vShakeRange.z <= 0) vShakeRange.z = 0;

            }



        }
        

       

    }

    public override void Action()
    {
        base.Action();

        // 初期座標保存
        vOrgPos = gameObject.transform.localPosition;

        bTrunOver = false;
}


    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;

        // このクラスは描画も止める
        gameObject.SetActive(false);

    }

    public Vector3 GetOrgPos() { return vOrgPos; }


}
