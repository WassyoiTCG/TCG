using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// メモ　
// localは親子関係考慮の数値

public abstract class BaseAnim2D : MonoBehaviour 
{
    //  Unity場で変更する値
    [Range(0, 360)]
    public int m_iDelayFrameMax = 0;    // 設定したディレイ
    private int m_iDelayFrame = 0;    // ディレイ
    //[Range(0, 360)]
    //public int m_iEndFrame = 120;    // 終りのフレーム

    // メンバ変数
    protected bool  m_bActionFlag;        // アクションフラグ	 	 
    protected bool  m_bEndFlag;           // 終了フラグ 
    protected bool  m_bFirstUpdateCheak;  // 最初更新したあとかチェック
    protected Image m_pImage;
    protected int   m_iCurrentFrame;      // 現在のフレーム
    protected bool  m_bRoop;              // ループ用

    //   // Use this for initialization
    //   void Start () {		
    //}
    //   // Update is called once per frame
    //void Update () {		
    //}

    // Use this for initialization
    protected virtual void Awake()
    {
        m_bActionFlag = false;
        m_bEndFlag = false;
        
        m_bFirstUpdateCheak = false;
        m_iCurrentFrame = 0;

        m_bRoop = false;

        m_pImage = GetComponent<Image>();
        if (!m_pImage) Debug.LogWarning("2DAnim: Imageがない");

        if (m_iDelayFrame < 0) Debug.LogWarning("2DAnim: DelayFrameが-の値に");
        //if (m_iEndFrame 　< 0) Debug.LogWarning("2DAnim: EndFrame-の値に");

        //cashTransform = transform;
    }


    // 継承更新
    public abstract void SelfUpdate();

    // 実行
    public virtual void Action(/*int delay*/)// ディレイはUnity場で設定
    {
        //(10/28) ディレイ考慮のためActionCheakに

        //　★★　ActiveがTrueの時始めてawekが呼ばれる
        //        必ずゲームオブジェクトをアクティブにしてから始める
        gameObject.SetActive(true);

        //m_iDelayFrame = delay;
        m_iDelayFrame = m_iDelayFrameMax;
        m_bActionFlag = true; /* 実行フラグOn */
        m_bEndFlag = false; // エンドフラグ
        m_bFirstUpdateCheak = false;

        m_bRoop = false;

        m_iCurrentFrame = 0; // 現在のフレーム初期化
    }

    // [12/04] ループ用のフラグ立てるだけ。
    public virtual void ActionRoop(/*int delay*/)// ディレイはUnity場で設定
    {
        //(10/28) ディレイ考慮のためActionCheakに

        //　★★　ActiveがTrueの時始めてawekが呼ばれる
        //        必ずゲームオブジェクトをアクティブにしてから始める
        gameObject.SetActive(true);

        //m_iDelayFrame = delay;
        m_iDelayFrame = m_iDelayFrameMax;
        m_bActionFlag = true; /* 実行フラグOn */
        m_bEndFlag = false; // エンドフラグ
        m_bFirstUpdateCheak = false;

        m_bRoop = true;

        m_iCurrentFrame = 0; // 現在のフレーム初期化
    }

    public virtual void StopRoop()
    {
        //
        Debug.LogWarning("オーバーライドしてくれめんす");
     
    }

    // エンドフラグ
    public virtual bool IsEndFlag() { return m_bEndFlag; }

    // ディレイをプログラムでも設定できるように
    public virtual void SetDelayFrame(int iDelay)
    {
        m_iDelayFrameMax = iDelay;
    }
    
    // 止める
    public virtual void Stop() { m_bActionFlag = false; }

    // 演出を続ける
    public virtual void KeepUp() {
        gameObject.SetActive(true);
        m_bActionFlag = true;
        
    }

    // アクションフラグが立っているかかつ遅延が切れてるかチェック
    protected bool ActionCheck()
    {
        // アクションフラグがたっていないと返す
        if (!m_bActionFlag) return false;

        m_bFirstUpdateCheak = true; // 更新が一回以上入った

        //Debug.Log("キテルグマ1");

        // ディレイタイマーが0になるまで通さない
        m_iDelayFrame--;
        if (m_iDelayFrame > 0)
        {
            
            return false;
        }

        m_iDelayFrame = 0;

        //　★★　ActiveがTrueの時始めてawekが呼ばれる
        //        必ずゲームオブジェクトをアクティブにしてから始める
        //gameObject.SetActive(true);

        return true; //成功
    }
    // 
    public void SetPos(Vector3 pos)
    {
        transform.localPosition =  pos;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetScale(Vector3 vScale)
    {
        transform.localScale = vScale;
    }

    // 
    public void SetAngle(float angle)
    {
        // 3.14×　360◎
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
    
    public void SetAngle(Vector3 vAngle)
    {
        // 3.14×　360◎
        transform.eulerAngles = vAngle;
    }

    public void SetAlpha(float alpha)
    {
        var newColor = m_pImage.color;
        newColor.a = alpha;
        m_pImage.color = newColor;
    }

    
}
