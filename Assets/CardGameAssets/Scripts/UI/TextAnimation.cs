using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : MonoBehaviour {

    public enum NUM_COLOR
    {
        NORMAL, HEAL
    }

    public Text text;
    public int iEndFrame = 180;
    public int iArrivalFrame = 60;
    public int iVanishFrame = 120;

    bool m_bActionFlag = false;

    int m_iCurrentFrame = 0;

    int m_iDelayFrame = 0;

    float m_fScale = 0;

    // Use this for initialization
    void Awake () {
        m_bActionFlag = false;

        m_fScale = 1;
        //text = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {

        if (m_bActionFlag == false)
        {
            return;
        }

        m_iDelayFrame--;
        if (m_iDelayFrame > 0)
        {
            return;
        }

        // エンドフレームまで来たら終わる
        m_iCurrentFrame++;
        if (m_iCurrentFrame >= iEndFrame)
        {
            m_bActionFlag = false;
            //gameObject.SetActive(false);
        }

        //+------------------------------------------------------
        //
        //+------------------------------------------------------
        float lAlpha = 0.0f;

        // Alpha調整
        if (m_iCurrentFrame <= iArrivalFrame)
        {
            lAlpha = (float)m_iCurrentFrame / (float)iArrivalFrame;

            var newColor = text.color;
            newColor.a = lAlpha;
            text.color = newColor;

            if (text.color.a >= 1.0f)
            {
                newColor.a = 1.0f;
                text.color = newColor;
            }

        }
        else if (m_iCurrentFrame >= iVanishFrame)
        {

            float l_fParam = (float)(iEndFrame - m_iCurrentFrame);
            float l_fMax = (float)(iEndFrame - iVanishFrame);
            float l_fAlpha = l_fParam / l_fMax;
            var newColor = text.color;
            newColor.a = l_fAlpha;
            text.color = newColor;

            if (text.color.a <= 0.0f)
            {
                newColor.a = 0.0f;
                text.color = newColor;
            }

        }

    }

    public void Action(int num, NUM_COLOR eColType, int DelayFrame = 0)
    {

        gameObject.SetActive(true);

        m_iDelayFrame = DelayFrame;

        // 表示する数値
        text.text = num.ToString();

        var newColor = text.color;
        switch (eColType)
        {
            case NUM_COLOR.NORMAL:
                newColor.r = 255;
                newColor.g = 255;
                newColor.b = 150;
                newColor.a = 0;

                break;
            case NUM_COLOR.HEAL:
                newColor.r = 0;
                newColor.g = 255;
                newColor.b = 128;
                newColor.a = 0;

                break;
            default:
                int a = 0; a++;
                break;
        }
        text.color = newColor;


        m_bActionFlag = true;
        m_iCurrentFrame = 0;

    }

    public void Action()
    {

        gameObject.SetActive(true);

        var newColor = text.color;
        newColor.a = 0.0f;
        text.color = newColor;

        m_bActionFlag = true;
        m_iCurrentFrame = 0;

    }
}
