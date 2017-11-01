using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMenu : MonoBehaviour {


    // メンバ変数
    public BaseEntityStateMachine<SceneMenu> m_pStateMachine;

   // enum MENU_TYPE { TOTORIAL, BATTLE, DECK, COLLECTION, OPTION, END };

    // 座標
   // private Vector3[] m_vPos = new Vector3[(int)(MENU_TYPE.END)];

    public GameObject Canvas;
    public GameObject MenuPlate;

    public GameObject[] MenuButton = new GameObject[(int)MENU_TYPE.END];


    public GameObject MenuSelectCursor; // カーソル

    //public GameObject TutorialButton;
    //public GameObject BattleButton;
    //public GameObject DeckButton;
    //public GameObject CollectButton;
    //public GameObject OptionButton;

    public GameObject InfoPlate;
    public GameObject[] Info = new GameObject[(int)MENU_TYPE.END];

    // コイン
    public GameObject CoinPlate;


    public GameObject[] MenuSphere = new GameObject[(int)MENU_SPHERE_TYPE.END];

    //public GameObject Canvas;
    //public GameObject Canvas;
    //public GameObject Canvas;

    // Use this for initialization
    void Start () {

        MenuPlate = Canvas.transform.Find("MenuButton/MenuPlate").gameObject;

        InfoPlate = Canvas.transform.Find("Info/InfoPlate").gameObject;

        // メニュー選択カーソル
        MenuSelectCursor = Canvas.transform.Find("MenuButton/MenuSelectCursor").gameObject;

        // 割り当て
        for (int i = 0; i < (int)MENU_TYPE.END; i++)
        {
            switch ((MENU_TYPE)i)
            {
                case MENU_TYPE.TOTORIAL:
                    MenuButton[i] = Canvas.transform.Find("MenuButton/TutorialButton").gameObject;
                    Info[i] = Canvas.transform.Find("Info/TutorialInfo").gameObject;
                    break;
                case MENU_TYPE.BATTLE:
                    MenuButton[i] = Canvas.transform.Find("MenuButton/BattleButton").gameObject;
                    Info[i] = Canvas.transform.Find("Info/BattleInfo").gameObject;
                    break;
                case MENU_TYPE.DECK:
                    MenuButton[i] = Canvas.transform.Find("MenuButton/DeckButton").gameObject;
                    Info[i] = Canvas.transform.Find("Info/DeckInfo").gameObject;
                    break;
                case MENU_TYPE.COLLECTION:
                    MenuButton[i] = Canvas.transform.Find("MenuButton/CollectButton").gameObject;
                    Info[i] = Canvas.transform.Find("Info/CollectInfo").gameObject;
                    break;
                case MENU_TYPE.OPTION:
                    MenuButton[i] = Canvas.transform.Find("MenuButton/OptionButton").gameObject;
                    Info[i] = Canvas.transform.Find("Info/OptionInfo").gameObject;
                    break;
                case MENU_TYPE.END:
                    Debug.LogWarning("SceneMenu: それ以上のタイプはない。");
                   
                     break;
                default:
                    break;
            }
        }


        // 割り当て
        for (int j = 0; j < (int)MENU_SPHERE_TYPE.END; j++)
        {
            switch ((MENU_SPHERE_TYPE)j)
            {
                case MENU_SPHERE_TYPE.TOTORIAL:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/Tutorial").gameObject;
                    break;
                case MENU_SPHERE_TYPE.BATTLE:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/CpuBattle").gameObject;
                    break;
                case MENU_SPHERE_TYPE.NET_BATTLE:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/NetBattle").gameObject;
                    break;
                case MENU_SPHERE_TYPE.DECK_CREATE:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/Deck").gameObject;
                    break;
                case MENU_SPHERE_TYPE.COLLECTION:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/Collection").gameObject;
                    break;
                case MENU_SPHERE_TYPE.SHOP:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/Shop").gameObject;
                    break;
                case MENU_SPHERE_TYPE.OPTION:

                    MenuSphere[j] = Canvas.transform.Find("MenuSphere/Option").gameObject;
                    break;
                case MENU_SPHERE_TYPE.END:

                    break;
                default:
                    Debug.LogWarning("SceneMenu: そのタイプはない。");
                    break;
            }
        } //  for 

        //  コイン
        CoinPlate = Canvas.transform.Find("Info/CoinPlate").gameObject;
        

        // ステートマシンの初期化や切り替えは最後に行う
        m_pStateMachine = new BaseEntityStateMachine<SceneMenu>(this);

        m_pStateMachine.globalState = SceneMenuState.Global.GetInstance();
        m_pStateMachine.ChangeState(SceneMenuState.Intro.GetInstance());
            return;


        // Canvas.transform.Find("BG").position= new Vector3(0, 100, 0);


    }

    // Update is called once per frame
    void Update () 
    {
        m_pStateMachine.globalState.Execute(this);
        

    }


    // メニューボタン達に触れた時
    public void ClickMenuButton(int no)
    {
        SelectMenuNo tagSelect;

        // セレクトナンバー設定
        tagSelect.selectNo = no;

        //ClickMenuButton;
        switch ((MENU_TYPE)no)
        {
            case MENU_TYPE.TOTORIAL:
                break;
            case MENU_TYPE.BATTLE:
                break;
            case MENU_TYPE.DECK:
                break;
            case MENU_TYPE.COLLECTION:
                break;
            case MENU_TYPE.OPTION:
                break;
            case MENU_TYPE.END:
                Debug.LogWarning("SceneMenu: それ以上のタイプはない。");
                break;
            default:
                break;
        }

        

          // メッセージ作成
          var message = new MessageInfo();
        message.messageType = MessageType.ClickMenuButton;

        // エクストラインフォに構造体を詰め込む
        message.SetExtraInfo(tagSelect);

        HandleMessge(message);
    }


    // スフィアボタンに触れた時
    public void ClickSphereButton(int no)
    {

        // ↑の引数によってデッキ選択画面をだしたり
        // シーンを移動する処理を行う

        switch ((MENU_SPHERE_TYPE)no)
        {
            case MENU_SPHERE_TYPE.TOTORIAL:
                break;
            case MENU_SPHERE_TYPE.BATTLE:
                break;
            case MENU_SPHERE_TYPE.NET_BATTLE:
                break;
            case MENU_SPHERE_TYPE.DECK_CREATE:
                break;
            case MENU_SPHERE_TYPE.COLLECTION:
                break;
            case MENU_SPHERE_TYPE.SHOP:
                break;
            case MENU_SPHERE_TYPE.OPTION:
                break;
            case MENU_SPHERE_TYPE.END:
                break;
            default:
                break;
        }



    }


    public bool HandleMessge(MessageInfo message)
    {
        return m_pStateMachine.HandleMessage(message);
    }

}
