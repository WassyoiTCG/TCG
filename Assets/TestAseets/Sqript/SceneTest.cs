using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTest : MonoBehaviour
{
    //+------------------------------
    public GameObject Card;
    public GameObject UVEffectManager;
    public GameObject Panel;
    public GameObject TurnEndEffects;

    public GameObject UVMgr;
    public GameObject PanelMgr;

    public GameObject ClickEf;

    //+------------------------------
    // Use this for initialization
    void Start()
    {
        oulSystem.Initialize();
        oulAudio.PlayBGM("CollectionBGM", true);
        oulAudio.PlayBGM("tori", true);

        Card.GetComponent<ScreenOutAppeared>().Action();
        Card.GetComponent<Rotation>().Action();

    }

    // Update is called once per frame
    void Update()
    {   
        // throw処理
        if (Input.GetKey(KeyCode.Z))
        {
            Card.GetComponent<ScreenOutAppeared>().Action();
            Card.GetComponent<ScreenOutAppeared>().SetPos(new Vector3(-10,0,0));
        }
        // throw処理
        if (Input.GetKey(KeyCode.X))
        {
            Card.GetComponent<ScreenOutAppeared>().Stop();
        }
       
         // throw処理
        if (Input.GetKey(KeyCode.C))
        {
            Card.GetComponent<Rotation>().Action();
            Card.GetComponent<Rotation>().SetAngle(new Vector3(0, 0, 0));
        }

        // throw処理
        if (Input.GetKey(KeyCode.V))
        {
            //UVEffect.GetComponent<UVScroll>().Action();
            UVMgr.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.UP_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 90));

        }

        if (Input.GetKey(KeyCode.B))
        {
            //UVEffect.GetComponent<UVScroll>().Action();
            UVMgr.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.DOWN_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }

        if (Input.GetKey(KeyCode.N))
        {
            //UVEffect.GetComponent<UVScroll>().Action();
            UVMgr.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.SUMMON, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }

        if (Input.GetKey(KeyCode.P))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            PanelMgr.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.ABILITY, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }

        if (Input.GetKey(KeyCode.O))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            PanelMgr.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.DAMAGE, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }
        if (Input.GetKey(KeyCode.I))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            PanelMgr.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.STAR, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }
        if (Input.GetKey(KeyCode.U))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            PanelMgr.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.ORANGE_LIGHT, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }


        if (Input.GetKey(KeyCode.Y))
        {
            ClickEf.SetActive(true);
            ClickEf.GetComponent<Animator>().Play(0, 0, 0.0f);
        }

        if (Input.GetKey(KeyCode.F))
        {
            TurnEndEffects.GetComponent<TurnEndEffects>().Action(TURN_END_TYPE.MAIN);
        }

        if (Input.GetKey(KeyCode.G))
        {
            TurnEndEffects.GetComponent<TurnEndEffects>().Action(TURN_END_TYPE.BATTLE);
        }
        if (Input.GetKey(KeyCode.H))
        {
            TurnEndEffects.GetComponent<TurnEndEffects>().Action(TURN_END_TYPE.WINNER);
        }
        if (Input.GetKey(KeyCode.J))
        {
            TurnEndEffects.GetComponent<TurnEndEffects>().Action(TURN_END_TYPE.LOSER);
        }

        Card.GetComponent<ScreenOutAppeared>().SelfUpdate();
        Card.GetComponent<Rotation>().SelfUpdate();

        
    }
}
