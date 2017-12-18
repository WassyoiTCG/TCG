
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//+--------------------------------
// ターンエンドに必要な奴つくる
//+--------------------------------
public enum PHASE_TYPE
{
    MAIN,
    BATTLE,
    WINNER,
    LOSER,
    BATTLE_START,
    LEST,   // 休憩
    END
}


// ターンエンドの演出やるやで
public class TurnEndEffects : MonoBehaviour
{

    // [メモ]
    // UnassignedReferenceException: The Variable 変数名 of 'コンポーネント名' has not been assigned.
    // マネージャーをゲームオブジェクトとして読み込むときはHierarchyに
    // 置いてからHierarchy場のマネージャーをアタッチする。

    //+--------------------------------
    // ターンエンドに必要な奴つくるやで
    //+--------------------------------

    //public GameObject PearentCanvasGroup; //　親のグループ  ( 生成した時の親 )

    public GameObject BlackPanel;
    public GameObject BlueRing;
    public GameObject OrangeRing;
    public GameObject Font_BattlePhase;
    public GameObject Font_BattlePhaseRip;
    public GameObject Font_MainPhase;
    public GameObject Font_MainPhaseRip;
    public GameObject StarDust;

    public GameObject Font_Winner;
    public GameObject Font_Loser;
    public GameObject OrangeFrash;
    public GameObject BlueFrash;

    public GameObject Font_WinnerRip;

    public GameObject VS;
    public GameObject BlackPanel_VS;
    public GameObject NAME_1;
    public GameObject NAME_2;

    public Text nameText1;
    public Text nameText2;

    private PHASE_TYPE m_eType = PHASE_TYPE.MAIN;

    private int m_iSelfFrame = 0;

    private bool m_bFinishFlag = false;

    const int iAnimationEndFrame = 90;// 90フレームで演出強制終了

    // Use this for initialization
    void Awake()
    {
        nameText1.text = "たなかのなかたたいすけん";
        nameText2.text = "まっちょりぬす";

        m_iSelfFrame = 0;

        m_eType = PHASE_TYPE.LEST;

        BlackPanel.SetActive(false);
        Font_MainPhase.SetActive(false);
        Font_BattlePhase.SetActive(false);

        BlackPanel_VS.SetActive(false);

        m_bFinishFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 休憩中よ
        if (m_eType == PHASE_TYPE.LEST) { m_iSelfFrame = 0; m_bFinishFlag = false; return; }

        // フレーム更新
        m_iSelfFrame++;

        BlackPanel.GetComponent<AlphaMove>().SelfUpdate();

        BlueRing.GetComponent<Ripple>().SelfUpdate();
        OrangeRing.GetComponent<Ripple>().SelfUpdate();
        StarDust.GetComponent<Ripple>().SelfUpdate();

        Font_MainPhase.GetComponent<ScaleAppeared>().SelfUpdate();
        Font_MainPhase.GetComponent<AlphaMove>().SelfUpdate();
        Font_MainPhaseRip.GetComponent<Ripple>().SelfUpdate();

        Font_BattlePhase.GetComponent<ScaleAppeared>().SelfUpdate();
        Font_BattlePhase.GetComponent<AlphaMove>().SelfUpdate();
        Font_BattlePhaseRip.GetComponent<Ripple>().SelfUpdate();

        OrangeFrash.GetComponent<Ripple>().SelfUpdate();
        BlueFrash.GetComponent<Ripple>().SelfUpdate();

        // 勝者
        Font_Winner.GetComponent<ScaleAppeared>().SelfUpdate();
        Font_Winner.GetComponent<AlphaMove>().SelfUpdate();
        Font_WinnerRip.GetComponent<Ripple>().SelfUpdate();

        // 敗者
        Font_Loser.GetComponent<ScreenOutAppeared>().SelfUpdate();
        Font_Loser.GetComponent<AlphaMove>().SelfUpdate();

        // VS
        VS.GetComponent<ScaleAppeared>().SelfUpdate();
        VS.GetComponent<AlphaMove>().SelfUpdate();
        NAME_1.GetComponent<ScreenOutAppeared>().SelfUpdate();
        NAME_2.GetComponent<ScreenOutAppeared>().SelfUpdate();

        BlackPanel_VS.GetComponent<AlphaMove>().SelfUpdate();

        const int iScaleAnim = 52;
        switch (m_eType)
        {
            case PHASE_TYPE.MAIN:

                if (m_iSelfFrame >= iScaleAnim)
                {
                    Font_MainPhase.transform.localScale -= new Vector3(0.075f, 0.075f, 0.075f);
                }

                if (m_iSelfFrame >= iAnimationEndFrame)
                {
                    m_eType = PHASE_TYPE.LEST;
                    Debug.Log("演出終了- TurnEndEffects");
                }
                break;
            case PHASE_TYPE.BATTLE:

                if (m_iSelfFrame >= iScaleAnim)
                {
                    Font_BattlePhase.transform.localScale -= new Vector3(0.075f, 0.075f, 0.075f);
                }

                if (m_iSelfFrame >= iAnimationEndFrame)
                {
                    m_eType = PHASE_TYPE.LEST;
                    Debug.Log("演出終了- TurnEndEffects");
                }
                break;
            case PHASE_TYPE.WINNER:
                const int iBackEffectAnim = 16;
                if (m_iSelfFrame == iBackEffectAnim)
                {
                    OrangeFrash.GetComponent<Ripple>().Action();
                    OrangeRing.GetComponent<Ripple>().Action();
                    StarDust.GetComponent<Ripple>().Action();
                }
                // (TODO)仮で直打ち
                if (m_iSelfFrame >= 180)
                {
                    //m_eType = PHASE_TYPE.LEST;
                    m_bFinishFlag = true;
                    Debug.Log("演出終了- TurnEndEffects");
                }
                
                break;
            case PHASE_TYPE.LOSER:
                // (TODO)仮で直打ち
                if (m_iSelfFrame >= 180)
                {
                    //m_eType = PHASE_TYPE.LEST;
                    m_bFinishFlag = true;
                    Debug.Log("演出終了- TurnEndEffects");
                }
                break;
            case PHASE_TYPE.BATTLE_START:

                if (m_iSelfFrame == 110)
                {

                    NAME_1.GetComponent<ScreenOutAppeared>().Action();
                    NAME_2.GetComponent<ScreenOutAppeared>().Action();
                    NAME_1.GetComponent<ScreenOutAppeared>().SetPosReCalcNextPos(NAME_1.transform.localPosition);
                    NAME_2.GetComponent<ScreenOutAppeared>().SetPosReCalcNextPos(NAME_2.transform.localPosition);
                }

                if (m_iSelfFrame >= 115)
                {
                    VS.transform.localScale -= new Vector3(0.075f, 0.075f, 0.075f);
                }

                if (m_iSelfFrame >= 135)
                {

                    NAME_1.GetComponent<ScreenOutAppeared>().Stop();
                    NAME_2.GetComponent<ScreenOutAppeared>().Stop();
                    m_eType = PHASE_TYPE.LEST;
                    Debug.Log("演出終了- TurnEndEffects");
                }
                break;
            case PHASE_TYPE.LEST:

                break;
            case PHASE_TYPE.END:

                break;
            default:
                break;
        }



    }


    void SelfUpdate()
    {

    }

    public void Action(PHASE_TYPE eType)
    {

        // まず残ってる演出止める
        // Stop();
        m_bFinishFlag = false;

        // フレーム初期化
        m_iSelfFrame = 0;

        // タイプによりエフェクト発動

        m_eType = eType;
        switch (m_eType)
        {

            case PHASE_TYPE.MAIN:
                // SE
                oulAudio.PlaySE("phase");

                BlackPanel.GetComponent<AlphaMove>().Action();

                BlueFrash.GetComponent<Ripple>().Action();
                BlueRing.GetComponent<Ripple>().Action();
                StarDust.GetComponent<Ripple>().Action();


                Font_MainPhase.GetComponent<ScaleAppeared>().Action();
                Font_MainPhase.GetComponent<AlphaMove>().Action();
                Font_MainPhaseRip.GetComponent<Ripple>().Action();
                Vector3 AwakeScale = Font_MainPhase.GetComponent<ScaleAppeared>().GetAwakScale();
                Font_MainPhase.GetComponent<ScaleAppeared>().SetScale(AwakeScale);
                break;
            case PHASE_TYPE.BATTLE:
                // SE
                oulAudio.PlaySE("phase");

                BlackPanel.GetComponent<AlphaMove>().Action();

                OrangeFrash.GetComponent<Ripple>().Action();
                OrangeRing.GetComponent<Ripple>().Action();
                StarDust.GetComponent<Ripple>().Action();


                Font_BattlePhase.GetComponent<ScaleAppeared>().Action();
                Font_BattlePhase.GetComponent<AlphaMove>().Action();
                Font_BattlePhaseRip.GetComponent<Ripple>().Action();
                AwakeScale = Font_BattlePhase.GetComponent<ScaleAppeared>().GetAwakScale();
                Font_BattlePhase.GetComponent<ScaleAppeared>().SetScale(AwakeScale);

                break;
            case PHASE_TYPE.WINNER:
                //m_pHeaveho->OrderShrink(8, 1, 6);
                //m_pHeaveho->Action();
                //
                //m_pHeavehoShink->OrderShrink(8, 1, 6);
                //m_pHeavehoShink->Action(3);
                //
                //m_pBlueRing->Action(8);
                //
                //
                //m_pLight->Action(12);

                BlackPanel.GetComponent<AlphaMove>().ActionRoop();

                Font_Winner.GetComponent<ScaleAppeared>().Action();
                Font_Winner.GetComponent<AlphaMove>().Action();
                AwakeScale = Font_Winner.GetComponent<ScaleAppeared>().GetAwakScale();
                Font_Winner.GetComponent<ScaleAppeared>().SetScale(AwakeScale);

                Font_WinnerRip.GetComponent<Ripple>().Action();

                break;

            case PHASE_TYPE.LOSER:

                BlackPanel.GetComponent<AlphaMove>().ActionRoop();

                Font_Loser.GetComponent<ScreenOutAppeared>().Action();
                Font_Loser.GetComponent<AlphaMove>().Action();

                Vector3 vAwakePos = Font_Loser.GetComponent<ScreenOutAppeared>().GetAwakPos();
                Font_Loser.GetComponent<ScreenOutAppeared>().SetPos(vAwakePos);

                break;
            case PHASE_TYPE.BATTLE_START:
                // SE
                oulAudio.PlaySE("phase");

                BlackPanel_VS.GetComponent<AlphaMove>().Action();
                
                //StarDust.GetComponent<Ripple>().Action();

                VS.GetComponent<ScaleAppeared>().Action();
                VS.GetComponent<AlphaMove>().Action();
                AwakeScale = VS.GetComponent<ScaleAppeared>().GetAwakScale();
                VS.GetComponent<ScaleAppeared>().SetScale(AwakeScale);

                //NAME_1.GetComponent<ScreenOutAppeared>().SetNextVectorReCalcNextPos(NAME_1.GetComponent<ScreenOutAppeared>().GetAwakeNextPos());
                //NAME_2.GetComponent<ScreenOutAppeared>().SetNextVectorReCalcNextPos(NAME_2.GetComponent<ScreenOutAppeared>().GetAwakPos());

                NAME_1.GetComponent<ScreenOutAppeared>().Action();
                NAME_2.GetComponent<ScreenOutAppeared>().Action();
                NAME_1.GetComponent<ScreenOutAppeared>().SetPosReCalcNextPos(NAME_1.GetComponent<ScreenOutAppeared>().GetAwakPos());
                NAME_2.GetComponent<ScreenOutAppeared>().SetPosReCalcNextPos(NAME_2.GetComponent<ScreenOutAppeared>().GetAwakPos());

                break;
            case PHASE_TYPE.END:
            default:
                Debug.LogWarning("シルビア： あらへろしちゃん、そのTYPEは存在しないわよ。- TurnEnd ");
                break;
        }

    }


    public　void Stop()
    {
        BlackPanel.GetComponent<AlphaMove>().StopRoop();


    }

    public bool GetFinishFlag() {return  m_bFinishFlag; }
    public int GetSelfFrame() { return m_iSelfFrame;  } 
    public PHASE_TYPE GetTrunEndType() { return m_eType; }// Lestだったら○○とか
}
