using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifePoint : MonoBehaviour
{

    //+-----------------------------
    //  
    //+-----------------------------

    public int iMaxLP = 250;   // スコア
    private float fLerpLP;                          // 補間用スコア
    public int iLP { get; private set; }    // 自分のスコア
    public Number Number;
    public Number NumberPinch;
    public Image Gauge;
    public Image GaugePinch;
    public Image GaugeDamage;

    private bool bDangerFlag = false;
    public Image GaugeDanger;

    const int iYellowRate = 100;// 100から黄色や
    private bool bYellowFlag = false; // 黄色になる

    const int iDelayFrameMax = 20;
    int iDelayFrame = 0;
    public void Restart()
    {
        iLP = iMaxLP;
        Number.SetNumber(iLP);

        bYellowFlag = false;

        Number.gameObject.SetActive(true);
        Gauge.gameObject.SetActive(true);
        NumberPinch.gameObject.SetActive(false);
        GaugePinch.gameObject.SetActive(false);

        StopDangerFlag();
    }

    // Use this for initialization
    public void Awake()
    {
        Restart();
    }

    // Update is called once per frame
    public void Update()
    {

        // 色変更
        CheakYellowFlag();




        // LP黄色
        //if (bYellowFlag == true)
        //{
        //    GaugePinch.fillAmount = rate;
        //}
        //else // LP緑
        //{
        //    Gauge.fillAmount = rate;
        //}

        var rate2 = (float)iLP / (float)iMaxLP;
        Gauge.fillAmount = rate2;
        GaugePinch.fillAmount = rate2;

        iDelayFrame++;
        if (iDelayFrame >= iDelayFrameMax)
        {
            // 補間処理
            fLerpLP = Mathf.Lerp(fLerpLP, (float)iLP, 0.05f);
            // ゲージ処理
            var rate = fLerpLP / iMaxLP;
            iDelayFrame = iDelayFrameMax;
            GaugeDamage.fillAmount = rate;
        }

        // ピンチのとき
        if (bDangerFlag == true)
        {
            GaugeDanger.fillAmount = rate2;
            GaugeDanger.GetComponent<AlphaWave>().SelfUpdate();
        }

    }

    private void CheakYellowFlag()
    {
        // ある一定の数値になれば
        if (iLP <= iYellowRate)
        {
            if (bYellowFlag == true) return;
            bYellowFlag = true;
            Number.gameObject.SetActive(false);
            Gauge.gameObject.SetActive(false);
            NumberPinch.gameObject.SetActive(true);
            GaugePinch.gameObject.SetActive(true);
        }
        else 
        {
            if (bYellowFlag == false) return;
            bYellowFlag = false;
            Number.gameObject.SetActive(true);
            Gauge.gameObject.SetActive(true);
            NumberPinch.gameObject.SetActive(false);
            GaugePinch.gameObject.SetActive(false);
        }

        
    }

    public bool SetLP(int iScore)
    {
        int iPrevLP = iLP;

        iLP = Mathf.Min(Mathf.Max(iScore, 0), iMaxLP);
        if (iLP > iPrevLP)
        {
            // ゲージ処理
            var rate = (float)iLP / (float)iMaxLP;
            fLerpLP = iLP;
            GaugeDamage.fillAmount = rate;
        }

        iDelayFrame = 0;
        CheakYellowFlag();
        if (bYellowFlag == true)
        {
            NumberPinch.SetNumber(iLP); // 黄色
        }
        else 
        {
            Number.SetNumber(iLP);　// 緑色
        }
            
        if (iLP == 0) return true;

        return false;

    }

    public bool AddLP(int iAddScore)
    {
        iLP = Mathf.Min(Mathf.Max(iLP + iAddScore, 0), iMaxLP);

        iDelayFrame = 0;
        CheakYellowFlag();
        if (bYellowFlag == true)
        {
            NumberPinch.SetNumber(iLP); // 黄色
        }
        else
        {
            Number.SetNumber(iLP);　// 緑色
        }

        if (iLP == 0) return true;

        return false;

    }

    public void ActionDangerFlag() 
    {
        //bDangerFlag = bFlag;
        bDangerFlag = true;
        GaugeDanger.GetComponent<AlphaWave>().Action();
    }
    public void StopDangerFlag()
    {
        bDangerFlag = false;
        GaugeDanger.GetComponent<AlphaWave>().Stop();
    }
}
