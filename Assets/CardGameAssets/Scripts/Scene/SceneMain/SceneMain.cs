using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SceneMain : MonoBehaviour
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
    public BaseEntityStateMachine<SceneMain> stateMachine { get; private set; }

    public GameObject offlinePlayers;                           // オフライン用
    public oulNetwork networkManager;                           // ネット管理さん
    public UIManager uiManager;                                 // UI管理さん
    public PlayerManager playerManager;                         // プレイヤー管理さん
    public Text pointText;                                      // ポイントのテキスト
    public Text restPointText;                                  // 残りポイントのテキスト
    public PointCardManager pointManager { get; private set; }  // ポイント山札
    public int currentPoint { get; set; }                       // 現在表示されているポイント
    public List<int> aikoPoint { get; private set; }            // あいこポイント
    public int turn;                                            // 現在のターン

    public bool isOnline = false;

	// Use this for initialization
	void Awake()
    {
        // メッセージ管理初期化
        MessageManager.Start(this, isOnline);
        // カード初期化
        CardDataBase.Start();
        // ポイントマネージェー初期化
        pointManager = new PointCardManager();
        pointManager.Start();
        aikoPoint = new List<int>();
        // ステート初期化
        stateMachine = new BaseEntityStateMachine<SceneMain>(this);
        stateMachine.globalState = SceneMainState.Global.GetInstance();
        // オンラインかそうでないかで分岐
        if (isOnline)
        {
            offlinePlayers.SetActive(false);
            networkManager.gameObject.SetActive(true);
            stateMachine.ChangeState(SceneMainState.MatchingWait.GetInstance());
        }
        else
        {
            offlinePlayers.SetActive(true);
            stateMachine.ChangeState(SceneMainState.BattleStart.GetInstance());
        }

        //state = State.NetOrOffline;
        // UI表示
        //uiManager.AppearNetOrOffline();
        uiManager.DisAppearMainUI();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // ステートマシン更新
        stateMachine.Update();
    }

    public void HandleMessage(MessageInfo message)
    {
        stateMachine.HandleMessage(message);
    }


    public void Finish()
    {
        var myScore = playerManager.uiManager.myScore;
        var cpuScore = playerManager.uiManager.cpuScore;
        if (myScore > cpuScore)
        {
            pointText.text = "あなたの勝ち";
        }
        else if (myScore < cpuScore)
        {
            pointText.text = "あなたの負け";
        }
        else pointText.text = "引き分け";

        stateMachine.ChangeState(SceneMainState.Finish.GetInstance());
    }

    public void Restart()
    {
        pointText.text = "";
        pointManager.Start();
        aikoPoint.Clear();

        // ターン初期化
        turn = 0;

        // オンラインかそうでないかで分岐
        if (isOnline)
        {
            stateMachine.ChangeState(SceneMainState.MatchingWait.GetInstance());
        }
        else
        {
            stateMachine.ChangeState(SceneMainState.BattleStart.GetInstance());
        }
        // プレイヤー初期化
        playerManager.Restart();
        // UI初期化
        uiManager.Restart();
        // ネットワーク初期化
        networkManager.Restart();
        MessageManager.Restart();
    }
}
