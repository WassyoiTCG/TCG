using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PANEL_EFFECT_TYPE
{
    DAMAGE, ABILITY,STAR,ORANGE_LIGHT,
    END
}

public class PanelEffectManager : MonoBehaviour
{

    // [メモ]
    // UnassignedReferenceException: The Variable 変数名 of 'コンポーネント名' has not been assigned.
    // マネージャーをゲームオブジェクトとして読み込むときはHierarchyに
    // 置いてからHierarchy場のマネージャーをアタッチする。

    //+------------------------------------------
    //      メンバ変数
    //+------------------------------------------

    //public GameObject UVEffectGroup; //  UVエフェクトのグループ

    // メモ　Findはなるべくつかはないようにするため必要なObj全部作る
    public GameObject Damage;
    public GameObject Ability;
    public GameObject Star;
    public GameObject OrangeLight;

    // private static readonly int iNameHashTag = Animator.StringToHash("Positive");

    //+------------------------------------------
    //      
    //+------------------------------------------

    // Use this for initialization
    public void Awake()
    {

    }

    // 実行
    public void Action(PANEL_EFFECT_TYPE eType, Vector3 vPos, Vector3 vAngle/*, int iDelay = 0*/)
    {

        // 
        //Transform t; 

        switch (eType)
        {
            case PANEL_EFFECT_TYPE.DAMAGE:
                Damage.GetComponent<PanelAnim>().Action(vPos, vAngle);
                break;
            case PANEL_EFFECT_TYPE.ABILITY:
                Ability.GetComponent<PanelAnim>().Action(vPos, vAngle);
                break;
            case PANEL_EFFECT_TYPE.STAR:
                Star.GetComponent<PanelAnim>().Action(vPos, vAngle);
                break;
            case PANEL_EFFECT_TYPE.ORANGE_LIGHT:
                OrangeLight.GetComponent<PanelAnim>().Action(vPos, vAngle);
                break;
            case PANEL_EFFECT_TYPE.END:
            default:
                Debug.LogWarning(" セーニャ: 勇者様、そのEFFECT_TYPEは存在しません。- PanelEffectManager");
                break;
        }


    }

    // Update is called once per frame
    public void Update()
    {

    }
}
