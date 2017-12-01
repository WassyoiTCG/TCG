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
            transform.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.UP_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 90));

        }

        if (Input.GetKey(KeyCode.B))
        {
            //UVEffect.GetComponent<UVScroll>().Action();
            transform.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.DOWN_STATUS, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }

        if (Input.GetKey(KeyCode.N))
        {
            //UVEffect.GetComponent<UVScroll>().Action();
            transform.GetComponent<UVEffectManager>().
            Action(UV_EFFECT_TYPE.SUMMON, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }

        if (Input.GetKey(KeyCode.P))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            transform.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.ABILITY, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }

        if (Input.GetKey(KeyCode.O))
        {
            //Panel.GetComponent<Animator>().enabled = true;

            //int iNameHashTag = 0;// 決め打ち
            //Panel.GetComponent<Animator>().Play(iNameHashTag/*"anim"*/, 0,0);

            transform.GetComponent<PanelEffectManager>().
            Action(PANEL_EFFECT_TYPE.DAMAGE, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        }


        Card.GetComponent<ScreenOutAppeared>().SelfUpdate();
        Card.GetComponent<Rotation>().SelfUpdate();

        
    }
}
