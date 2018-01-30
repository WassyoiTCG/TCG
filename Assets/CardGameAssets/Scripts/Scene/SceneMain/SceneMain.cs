using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SceneMain : BaseNetworkScene
{
    //enum State
    //{
    //    NetOrOffline,   // ネット対戦かオフラインのどちらか
    //    Matching,       // マッチング
    //    Start,          // 開始
    //    FirstDraw,      // 最初のドロー、マリガン
    //    PointDraw,      // 点数カードをめくる
    //    Draw,           // 最初のターンと3ターンずつドローフェイズがある
    //    SetStriker,     // ファイターをセットする、イベントを発動などができる、互いがファイターを伏せ、準備完了となるまで難解でもファイターをセットしなおすことができる。
    //    BeforeOpen,     // オープン前
    //    Open,           // オープン
    //    AfterOpen,      // オープン後
    //    Result,         // 結果
    //    TurnEnd,        // ターン終了
    //    End             // 勝ったプレイヤーに点数を与える、相内の場合は次へ持ち越し(もう点数山札ないときは終了)
    //}
    //State state;
    BaseEntityStateMachine<SceneMain> stateMachine;
    
    public GameObject offlinePlayers;                           // オフライン用
    public oulNetwork networkManager;                           // ネット管理さん
    public UIManager uiManager;                                 // UI管理さん
    public PlayerManager playerManager;                         // プレイヤー管理さん
    public CardAbilityManager abilityManager;                   // 効果管理さん
    public TurnEndEffects turnEndEffect;                        // フェーズ演出さん
    public Text pointText;                                      // ポイントのテキスト
    public Text restPointText;                                  // 残りポイントのテキスト
    public PointCardManager pointManager { get; private set; }  // ポイント山札
    public int currentPoint { get; set; }                       // 現在表示されているポイント
    public List<int> aikoPoint { get; private set; }            // あいこポイント
    public int turn;                                            // 現在のターン
    //public PlayerController playerControl;                      // 

    public UVEffectManager UvEffectMgr_My;
    public UVEffectManager UvEffectMgr_Cpu;
    public PanelEffectManager PanelEffectMgr_My;
    public PanelEffectManager PanelEffectMgr_Cpu;

    public int iWaitFrame;                                      //  演出用止めるフレーム

    // ステート同期用
    public string[] currentStateNames = new string[2];
    public Text debugText;

    public Text debugCardText;
    //public bool isOnline = false;

    // Use this for initialization
    void Awake()
    {
        // 待機フレーム
        iWaitFrame = 0;

        // システム初期化
        oulSystem.Initialize();

        //  BGM
        oulAudio.PlayBGM("RisingWinter", true);

        // メッセージ管理初期化
        MessageManager.Start(SelectData.isNetworkBattle);
        MessageManager.SetNetworkScene(this);
        //// カード初期化
        //CardDataBase.Start();
        // ポイントマネージェー初期化
        pointManager = new PointCardManager();
        pointManager.Start();
        aikoPoint = new List<int>();
        // ステート初期化
        stateMachine = new BaseEntityStateMachine<SceneMain>(this);
        stateMachine.globalState = SceneMainState.Global.GetInstance();

        if (SelectData.isNetworkBattle)
        {
            if (!networkManager)
            {
                networkManager = GameObject.Find("NetworkManager").GetComponent<oulNetwork>();
                Debug.Assert(networkManager, "ネットワークマネージャー死んでる");
            }
            // プレイヤー追加
            networkManager.Spawn();
        }

        Restart();
	}

    // 終了時
    //void OnDisable()
    //{
    //    //  BGM
    //    oulAudio.StopBGM();

    //}

    // Update is called once per frame
    void Update ()
    {
        if (MessageManager.isNetwork)
        {
            // ステート同期チェック
            if (currentStateNames[0] != currentStateNames[1])
            {
                if (debugText)
                    debugText.text = "ステート同期中\r\n" + "自分:" + currentStateNames[0] + "\r\n" + "相手:" + currentStateNames[1];
                return;
            }
        }

        // ステートマシン更新
        stateMachine.Update();
    }

    public override void HandleMessage(MessageInfo message)
    {
        if (MessageManager.isNetwork)
        {
            // ステート同期メッセージ
            if (message.messageType == MessageType.SyncState)
            {
                SyncStateInfo info = new SyncStateInfo();
                message.GetExtraInfo<SyncStateInfo>(ref info);

                currentStateNames[message.fromPlayerID] = info.cStateName.ToString();
                return;
            }
        }

        stateMachine.HandleMessage(message);
    }

    public void ChangeState(BaseEntityState<SceneMain> newState)
    {
        // ステートチェンジ
        stateMachine.ChangeState(newState);

        // ステート同期用メッセージ
        char[] newStateName = newState.GetType().FullName.ToCharArray();
        SyncStateInfo info;
        info.cStateName = new char[128];
        for (int i = 0; i < newStateName.Length; i++)
        {
            info.cStateName[i] = newStateName[i];
        }

        MessageManager.Dispatch(playerManager.GetMyPlayerID(), MessageType.SyncState, info);
    }


    public void Finish()
    {
        var myScore = playerManager.uiManager.myLP.iLP;
        var cpuScore = playerManager.uiManager.cpuLP.iLP;
        if (myScore > cpuScore)
        {
            stateMachine.ChangeState(SceneMainState.Winner.GetInstance());
        }
        else if (myScore < cpuScore)
        {
            stateMachine.ChangeState(SceneMainState.Loser.GetInstance());
        }
        else stateMachine.ChangeState(SceneMainState.Winner.GetInstance());
    }

    public void Restart()
    {
        pointText.text = "";
        pointManager.Start();
        aikoPoint.Clear();

        // 残りのポイントUI初期化
        uiManager.remainingPoints.Reset();

        // ターン初期化
        turn = 0;

        // オンラインかそうでないかで分岐
        if (SelectData.isNetworkBattle)
        {
            offlinePlayers.SetActive(false);
            stateMachine.ChangeState(SceneMainState.MatchingWait.GetInstance());
            // ネットワーク初期化
            networkManager.Restart();
        }
        else
        {
            offlinePlayers.SetActive(true);
            // プレイヤー初期化
            playerManager.Restart();
            stateMachine.ChangeState(SceneMainState.BattleStart.GetInstance());
        }


        // UI初期化
        uiManager.Restart();
        MessageManager.Restart();
    }
}
