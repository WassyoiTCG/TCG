using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UV_EFFECT_TYPE
{
    SUMMON, UP_STATUS, DOWN_STATUS,
    END
}

public class UVEffectManager : MonoBehaviour {

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
                RisingImpact.GetComponent<UVScroll>().Action(vPos, iDelay);
                SummonRing.GetComponent<UVScroll>().Action(vPos, iDelay);

                break;
            case UV_EFFECT_TYPE.UP_STATUS:
                UpWave.GetComponent<UVScroll>().Action(vPos, iDelay);
                
                break;
            case UV_EFFECT_TYPE.DOWN_STATUS:
                DownWave.GetComponent<UVScroll>().Action(vPos, iDelay);

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
