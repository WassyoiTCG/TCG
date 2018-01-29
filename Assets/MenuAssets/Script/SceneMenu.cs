using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // 後ろ隠すよう
    public GameObject BlackPanel;

    public GameObject NetBattleGroup;

    // シーンチェンジフラグ
    public bool m_bSceneChange = false;

    // ネットワーク
    public oulNetwork networkManager;
    public InputField ipInput;

    // Tips
    public GameObject TipsGroup;
    public GameObject Tips1;
    public GameObject Tips2;
    public GameObject Tips3;
    public GameObject Tips4;
    //public GameObject Tips5;

    // プロフィール
    public ProfileCanvas profile;


    // Use this for initialization
    void Start () {

        // 追加1126 システムの初期化をする(WinMainのInitApp)
        oulSystem.Initialize();

        //  BGM
        oulAudio.PlayBGM("Select0", true);

        // ネットワークオブジェクト取得
        networkManager = GameObject.Find("NetworkManager").GetComponent<oulNetwork>();

        m_bSceneChange = false;
        //  a.GetComponent<ScreenOutAppeared>().Action();

        BlackPanel = Canvas.transform.Find("BlackPanel").gameObject;
        BlackPanel.SetActive(false);

        NetBattleGroup = Canvas.transform.Find("NetBattleSecondSelect").gameObject;
        NetBattleGroup.SetActive(false);

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

        // IP入力初期化
        if (ipInput)
        {
            ipInput.text = PlayerDataManager.GetPlayerData().ip;
        }

        // ステートマシンの初期化や切り替えは最後に行う
        m_pStateMachine = new BaseEntityStateMachine<SceneMenu>(this);

        m_pStateMachine.globalState = SceneMenuState.Global.GetInstance();
        m_pStateMachine.ChangeState(SceneMenuState.Intro.GetInstance());
            return;


        // Canvas.transform.Find("BG").position= new Vector3(0, 100, 0);


    }

    // 終了時
    void OnDisable()
    {
        //  BGM
        oulAudio.StopBGM();

    }

    // Update is called once per frame
    void Update () 
    {
        m_pStateMachine.globalState.Execute(this);
        m_pStateMachine.currentState.Execute(this);

        //a.GetComponent<ScreenOutAppeared>().SelfUpdate();
    }

    // 説明ボタン押したとき
    public void ClickTipsButton()
    {
        m_pStateMachine.ChangeState(SceneMenuState.Tips.GetInstance());
    }

    // プロフィールボタン押したとき
    public void ClickProfileCanvas()
    {
        // プロフィール表示
        profile.Action();
    }

    // なんかのボタン達に触れた時
    public void ClickAnyButton(int no)
    {
        // シーンチェンジを行っていたら反応させない
        if (m_bSceneChange) return;

        AnyButton tagAny;

        // セレクトナンバー設定
        tagAny.Index = no;

        // メッセージ作成
        var message = new MessageInfo();
        message.messageType = MessageType.ClickAnyButton;

        // エクストラインフォに構造体を詰め込む
        message.SetExtraInfo(tagAny);

        HandleMessge(message);
    }


    // メニューボタン達に触れた時
    public void ClickMenuButton(int no)
    {
        // シーンチェンジを行っていたら反応させない
        if (m_bSceneChange) return;

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
        // シーンチェンジを行っていたら反応させない
        if (m_bSceneChange) return;



        SelectSphereNo tagSelect;

        // セレクトナンバー設定
        tagSelect.selectNo = no;

        // ↑の引数によってデッキ選択画面をだしたり
        // シーンを移動する処理を行う

        switch ((MENU_SPHERE_TYPE)no)
        {
            case MENU_SPHERE_TYPE.TOTORIAL:
                 
                //m_bSceneChange = true;
                break;
            case MENU_SPHERE_TYPE.BATTLE:
                // 追加(SceneMainに飛ぶ)
                m_bSceneChange = true;
                SelectData.isNetworkBattle = false;
                SceneManager.LoadScene("Main");
                break;
            case MENU_SPHERE_TYPE.NET_BATTLE:
                //+-------------------------------------------------------------------
                // ステートを次の選択へ
                SelectData.isNetworkBattle = true;
                m_pStateMachine.ChangeState(SceneMenuState.NetBattleSecondSelect.GetInstance());
                break;
            case MENU_SPHERE_TYPE.DECK_CREATE:
                m_bSceneChange = true;
                SceneManager.LoadScene("Deck");
                break;
            case MENU_SPHERE_TYPE.COLLECTION:

                //m_bSceneChange = true;
                break;
            case MENU_SPHERE_TYPE.SHOP:

                //m_bSceneChange = true;
                break;
            case MENU_SPHERE_TYPE.OPTION:

                //m_bSceneChange = true;
                break;
            case MENU_SPHERE_TYPE.END:
            default:
                Debug.LogWarning("ないです、");
                break;
        }

        //// メッセージ作成
        //var message = new MessageInfo();
        //message.messageType = MessageType.ClickSphereButton;

        //// エクストラインフォに構造体を詰め込む
        //message.SetExtraInfo(tagSelect);

        //HandleMessge(message);


        //+-------------------------------------------------------------------
        // ステートをシーンチェンジへ
        //m_pStateMachine.ChangeState(SceneMenuState.SceneChange.GetInstance());

    }





    public bool HandleMessge(MessageInfo message)
    {
        return m_pStateMachine.HandleMessage(message);
    }

}
