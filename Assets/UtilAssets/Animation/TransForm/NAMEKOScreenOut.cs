//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class NAMEKOScreenOut : BaseAnim2D{

//    //+ Unity場で変更する値---------------------
    
//    //[Range(0.01f, 2.0f)]
//    public Vector3 vVelocity = new Vector3(0, 0, 0);  // 初速

//    //[Range(0.1f, 1.0f)]
//    public float fPower = 1;     // fVelocity(初速)から何割の力で引き戻すか
//    public float fMaxPower = 1;  // fVelocity(初速)から何割の力で引き戻すか

//    private Vector3 vAddVec = new Vector3(0, 0, 0); // 加算されていくやつ

//    private Vector3 vNextPos = new Vector3(0.0f, 0.0f, 0.0f);// 元の値に足した先の場所
//    public  Vector3 vNextVector = new Vector3(0.0f, 0.0f, 0.0f);     // 元の値のベクトル
//    private Vector3 vAwakePos = new Vector3(0.0f, 0.0f, 0.0f);
//    private Vector3 vAwakeNextPos = new Vector3(0.0f, 0.0f, 0.0f);


//    //private Vector3 vNextPos = new Vector3(0.0f, 0.0f, 0.0f);// 元の値に足した先の場所

//    // public Vector3 vNextVector = new Vector3(0.0f, 0.0f, 0.0f);     // 元の値のベクトル
//    // private Vector3 vAwakePos = new Vector3(0.0f, 0.0f, 0.0f);
//    //+-------------------------------

//    //+ メンバ変数---------------------
//    // public float m_fRate = 0.0f;
//    //+-------------------------------
//    protected enum STEP
//    {
//        START, BACK, END
//    }
//    protected STEP eStep;

//    // 初期化
//    protected override void Awake()
//    {
//        base.Awake();

//        eStep = STEP.START;
//        vAddVec = vVelocity;

//        // ベクトルだけ先に保存しておく
//        //vNextVector = vAddNextPos;
//        vAwakePos = gameObject.transform.localPosition;// 初期座標保存
//        vNextPos = gameObject.transform.localPosition + vNextVector;
//        vAwakeNextPos = vNextPos;
//    }

//    // Update is called once per frame
//    public override void SelfUpdate()
//    {
//        // アクションフラグがたっていないと返す
//        if (!ActionCheck()) return;

//        // 毎フレーム加算
//        gameObject.transform.localPosition += vAddVec;

//        switch (eStep)
//        {
//            case STEP.START:
//                {

//                    float fCurrentScale = gameObject.transform.localScale.x;
//                    if (fCurrentScale >= fEndScale)
//                    {
//                        eStep = STEP.BACK;
//                    }
//                }
//                break;
//            case STEP.BACK:
//                {
//                    //  引っ張る力が働く(重力)

//                    fAddScale -= fVelocity * fBackPower;

//                    // TODO 重力の限界値を付ける
//                    float fPow = 0.35f;
//                    if (fAddScale <= -(fVelocity * fPow)) { fAddScale = -(fVelocity * fPow); }

//                    gameObject.transform.localScale += new Vector3(fAddScale, fAddScale);

//                    float fCurrentScale = gameObject.transform.localScale.x;
//                    if (fCurrentScale <= fEndScale)
//                    {
//                        eStep = STEP.BACK2;
//                        // 最後スケール合わす
//                        //m_pImage.transform.localScale = new Vector3(fEndScale, fEndScale, fEndScale);
//                    }
//                }
//                break;
//            case STEP.END:
//                {


//                    m_bActionFlag = false;
//                    m_bEndFlag = true; // 終りフラグON
//                }
//                break;
//            default:
//                Debug.LogWarning("なめこ: そのタイプはない。");
//                break;
//        }

//    }

//    public override void Action()
//    {
//        base.Action();

//        Vector3 vVec = vNextPos - gameObject.transform.localPosition;
//        vVec.Normalize();

//        vAddVec = vVelocity; // vVec *;
        
//    }

//    // 止める
//    public override void Stop()
//    {
//        m_bActionFlag = false;

//        // このクラスは描画も止める
//        gameObject.SetActive(false);
        
//    }

//    // ポジションを変えたのち進むベクトルも再計算
//    public void SetPosReCalcNextPos(Vector3 vPos)
//    {

//        gameObject.transform.localPosition = vPos;
//        vNextPos = gameObject.transform.localPosition + vNextVector;

//    }

//    // ベクトルを変えたのち進むベクトルも再計算
//    public void SetNextVectorReCalcNextPos(Vector3 vVec)
//    {
//        vNextVector = vVec;
//        vNextPos = gameObject.transform.localPosition + vNextVector;

//    }

//    // 行先を渡す
//    public Vector3 GetNextPos()
//    {
//        return vNextPos;
//    }

//    // 行先直接変える
//    public void SetNextPos(Vector3 vPos)
//    {
//        vNextPos = vPos;
//    }



//    // 速度
//    public float GetSpeed()
//    {
//        return fSpeed;
//    }

//    // 初期座標を渡す
//    public Vector3 GetAwakPos()
//    {
//        return vAwakePos;
//    }

//    // 初期行先を渡す
//    public Vector3 GetAwakeNextPos()
//    {
//        return vAwakeNextPos;
//    }
//}
