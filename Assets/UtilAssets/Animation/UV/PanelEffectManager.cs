using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PANEL_EFFECT_TYPE
{
    DAMAGE, ABILITY,
    END
}

public class PanelEffectManager : MonoBehaviour
{

    //+------------------------------------------
    //      メンバ変数
    //+------------------------------------------

    //public GameObject UVEffectGroup; //  UVエフェクトのグループ

    // メモ　Findはなるべくつかはないようにするため必要なObj全部作る
    public GameObject Damage;
    public GameObject Ability;

    // private static readonly int iNameHashTag = Animator.StringToHash("Positive");
    private const int iNameHashTag = 0;// 決め打ちで0ハッシュタグ0番目のアニメーションを発動

    //+------------------------------------------
    //      
    //+------------------------------------------

    // Use this for initialization
    public void Awake()
    {


    }

    // 実行
    public void Action(PANEL_EFFECT_TYPE eType, Vector3 vPos, Vector3 vVec, int iDelay = 0)
    {

        // 
        //Transform t; 

        switch (eType)
        {
            case PANEL_EFFECT_TYPE.DAMAGE:
                oulMath.Billboard(Damage.transform);
                Damage.GetComponent<Animator>().Play(iNameHashTag, 0, 0.0f);
                break;
            case PANEL_EFFECT_TYPE.ABILITY:
                oulMath.Billboard(Ability.transform);
                Ability.GetComponent<Animator>().Play(iNameHashTag, 0, 0.0f);
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
