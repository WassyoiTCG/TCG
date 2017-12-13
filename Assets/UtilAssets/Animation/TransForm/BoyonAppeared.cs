using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyonAppeared : BaseAnim2D
{

    //+ Unity場で変更する値---------------------

    [Range(0.0f, 10.0f)]
    public float fStartScale = 0.25f;

    [Range(0.1f, 10.0f)]
    public float fEndScale = 1.0f;

    [Range(0.01f, 2.0f)]
    public float fVelocity = 0.1f;

    [Range(0.1f, 1.0f)]
    public float fBackPower = 0.2f;// fVelocity(初速)から何割の力で引き戻すか

    private float fAddScale = 0.0f; 
    //private Vector3 vNextPos = new Vector3(0.0f, 0.0f, 0.0f);// 元の値に足した先の場所

    // public Vector3 vNextVector = new Vector3(0.0f, 0.0f, 0.0f);     // 元の値のベクトル
    // private Vector3 vAwakePos = new Vector3(0.0f, 0.0f, 0.0f);
    //+-------------------------------

    //+ メンバ変数---------------------
    // public float m_fRate = 0.0f;
    //+-------------------------------
    protected enum STEP
    {
      START, BACK, BACK2, BACK3, END
    }
    protected STEP eStep;

    // 初期化
    protected override void Awake()
    {
        base.Awake();

        eStep = STEP.START;
        fAddScale = fVelocity;

        // 透明に
        //SetAlpha(0.0f);
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
        {   // (仮)透明度　を触るなら↓はコメントアウトしよう
            //SetAlpha(1.0f);
            gameObject.SetActive(true);
        } 

      

        switch (eStep)
        {
            case STEP.START:
                {
                    gameObject.transform.localScale += new Vector3(fAddScale, fAddScale);

                    float fCurrentScale = gameObject.transform.localScale.x;
                    if (fCurrentScale >= fEndScale)
                    {
                        eStep = STEP.BACK;
                    }
                }
                break;
            case STEP.BACK:
                {
                    //  引っ張る力が働く(重力)

                    fAddScale -= fVelocity * fBackPower;

                    // TODO 重力の限界値を付ける
                    float fPow = 0.35f;
                    if (fAddScale <= -(fVelocity * fPow)) { fAddScale = -(fVelocity * fPow); }

                    gameObject.transform.localScale += new Vector3(fAddScale, fAddScale);

                    float fCurrentScale = gameObject.transform.localScale.x;
                    if (fCurrentScale <= fEndScale)
                    {
                        eStep = STEP.BACK2;
                        // 最後スケール合わす
                        //m_pImage.transform.localScale = new Vector3(fEndScale, fEndScale, fEndScale);
                    }
                }
                break;
            case STEP.BACK2:
                {
                    //  引っ張る力が働く(重力)

                    fAddScale += fVelocity * fBackPower;

                    // TODO 重力の限界値を付ける
                    float fPow = 0.2f;
                    if (fAddScale >= (fVelocity * fPow)) { fAddScale = (fVelocity * fPow); }

                    gameObject.transform.localScale += new Vector3(fAddScale, fAddScale);

                    float fCurrentScale = gameObject.transform.localScale.x;
                    if (fCurrentScale >= fEndScale)
                    {
                        eStep = STEP.BACK3;
                    }
                }
                break;
            case STEP.BACK3:
                {
                    //  引っ張る力が働く(重力)

                    fAddScale -= fVelocity * fBackPower;

                    // TODO 重力の限界値を付ける
                    float fPow = 0.1f;
                    if (fAddScale <= -(fVelocity * fPow)) { fAddScale = -(fVelocity * fPow); }

                    gameObject.transform.localScale += new Vector3(fAddScale, fAddScale);

                    float fCurrentScale = gameObject.transform.localScale.x;
                    if (fCurrentScale <= fEndScale)
                    {
                         eStep = STEP.END;
                        // 最後スケール合わす
                        gameObject.transform.localScale = new Vector3(fEndScale, fEndScale, fEndScale);

                    }
                }
                break;
            case STEP.END:
                {


                    m_bActionFlag = false;
                    m_bEndFlag = true; // 終りフラグON
                }
                  break;
            default:
                Debug.LogWarning("ぼよよん: そのタイプはない。");
                break;
        }
        
    }

    public override void Action()
    {
        base.Action();

        // 初期スケール
        gameObject.transform.localScale = new Vector3(fStartScale, fStartScale, fStartScale);

        eStep = STEP.START;
        fAddScale = fVelocity;

        // 透明に
        //SetAlpha(0.0f);
        // 
       // gameObject.SetActive(false);
    }

    // 止める
    public override void Stop()
    {
        m_bActionFlag = false;

        // このクラスは描画も止める
        gameObject.SetActive(false);

       // eStep = STEP.END;
    }


    // 演出を続ける
    //public override void KeepUp()
    //{
    //    gameObject.SetActive(true);
    //    m_bActionFlag = true;

    //    //eStep = STEP.END;

    //}

}
