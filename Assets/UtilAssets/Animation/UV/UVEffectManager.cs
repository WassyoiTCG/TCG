using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UV_EFFECT_TYPE
{
    SUMMON, UP_STATUS, DOWN_STATUS,SKILL_WIN, SKILL_LOSE,BIG_DAMAGE,
    END
}

public class UVEffectManager : MonoBehaviour {


    // [メモ]
    // UnassignedReferenceException: The Variable 変数名 of 'コンポーネント名' has not been assigned.
    // マネージャーをゲームオブジェクトとして読み込むときはHierarchyに
    // 置いてからHierarchy場のマネージャーをアタッチする。

    //+------------------------------------------
    //      メンバ変数
    //+------------------------------------------

    //public GameObject UVEffectGroup; //  UVエフェクトのグループ

    // メモ　Findはなるべくつかはないようにするため必要なObj全部作る
    public GameObject RisingImpact;
    public GameObject HitRing;
    public GameObject SummonRing;
    public GameObject DownWave;
    public GameObject UpWave;
    
    public GameObject SkiilWin;
    public GameObject SkiilLose;
    public GameObject BigDamage;

    //+------------------------------------------
    //      
    //+------------------------------------------

    // Use this for initialization
    public void Awake() {

    }

    // 実行
    public void Action(UV_EFFECT_TYPE eType, Vector3 vPos, Vector3 vVec , int iDelay = 0)
    {
        //if (vPos == null)
        //{
        //    vPos = new Vector3(0, 0, 0);
        //}
        //if (vVec == null)
        //{
        //    vVec = new Vector3(0, 0, 0);
        //}


        switch (eType)
        {
            case UV_EFFECT_TYPE.SUMMON:
                RisingImpact.SetActive(true);
                RisingImpact.GetComponent<UVScroll>().Action(vPos, iDelay);
                SummonRing.SetActive(true);
                SummonRing.GetComponent<UVScroll>().Action(vPos, iDelay);

                break;
            case UV_EFFECT_TYPE.UP_STATUS:
                UpWave.SetActive(true);
                UpWave.GetComponent<UVScroll>().Action(vPos, iDelay);
                
                break;
            case UV_EFFECT_TYPE.DOWN_STATUS:
                DownWave.SetActive(true);
                DownWave.GetComponent<UVScroll>().Action(vPos, iDelay);

                break;
            case UV_EFFECT_TYPE.SKILL_WIN:
                SkiilWin.SetActive(true);
                SkiilWin.GetComponent<UVScroll>().Action(vPos, iDelay);

                break;
            case UV_EFFECT_TYPE.SKILL_LOSE:
                SkiilLose.SetActive(true);
                SkiilLose.GetComponent<UVScroll>().Action(vPos, iDelay);

                break;
            case UV_EFFECT_TYPE.BIG_DAMAGE:
                BigDamage.SetActive(true);
                BigDamage.GetComponent<UVScroll>().Action(vPos, vVec, vVec, iDelay);

                break;
            case UV_EFFECT_TYPE.END:
            default:
                Debug.LogWarning(" カミュ: おいへろし、そのEFFECT_TYPEは存在しないぜ。- UVEffectManager");
                break;
        }


    }

    // Update is called once per frame
    public void Update () {
		
	}
}
