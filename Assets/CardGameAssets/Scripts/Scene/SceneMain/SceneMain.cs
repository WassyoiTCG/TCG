using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SceneMain : MonoBehaviour
{
    enum State
    {
        NetOrOffline,   // ネット対戦かオフラインのどちらか
        Matching,       // マッチング
        Start,          // 開始
        FirstDraw,      // 最初のドロー、マリガン
        PointDraw,      // 点数カードをめくる
        Draw,           // 最初のターンと3ターンずつドローフェイズがある
        SetStriker,     // ファイターをセットする、イベントを発動などができる、互いがファイターを伏せ、準備完了となるまで難解でもファイターをセットしなおすことができる。
        BeforeOpen,     // オープン前
        Open,           // オープン
        AfterOpen,      // オープン後
        Result,         // 結果
        TurnEnd,        // ターン終了
        End             // 勝ったプレイヤーに点数を与える、相内の場合は次へ持ち越し(もう点数山札ないときは終了)
    }
    State state;

    public GameObject offlinePlayers;                       // オフライン用
    public oulNetwork networkManager;                       // ネット管理さん
    public UIManager uiManager;                             // UI管理さん
    public PlayerManager playerManager;                     // プレイヤー管理さん
    public Text pointText;                                  // ポイントのテキスト
    public Text restPointText;                              // 残りポイントのテキスト
    PointCardManager pointManager = new PointCardManager(); // ポイント山札
    int currentPoint;                                       // 現在表示されているポイント
    List<int> aikoPoint = new List<int>();                  // あいこポイント
    int turn;                                               // 現在のターン

	// Use this for initialization
	void Awake()
    {
        // メッセージ管理初期化
        MessageManager.Start(this);
        // カード初期化
        CardDataBase.Start();
        // ポイントマネージェー初期化
        pointManager.Start();
        // ステート初期化
        state = State.NetOrOffline;
        uiManager.AppearNetOrOffline();
        // ターン初期化
        turn = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (state)
        {
            case State.NetOrOffline:
                break;
            case State.Matching:
                if(playerManager.isPlayesStandbyOK())
                {
                    uiManager.DisAppearMatchingWait();
                    state = State.Start;
                    Debug.Log("マッチング完了");

                    // (TODO)自分がホストならポイントデータを相手に送信
                    PointInfo exInfo = new PointInfo();
                    exInfo.points = new int[10];
                    exInfo.points = pointManager.randomIndexArray;
                    MessageManager.Dispatch(playerManager.GetMyPlayerID(), MessageType.SyncPoint, exInfo);
                }
                break;
            case State.Start:
                if (playerManager.isPlayesStandbyOK())
                {
                    state = State.FirstDraw;
                    // 最初ドローステートをアクティブに
                    //firstDrawState.gameObject.SetActive(true);
                    uiManager.AppearFirstDraw();
                    // プレイヤーにドローさせる
                    playerManager.SetState(PlayerState.FirstDraw);
                }
                break;
            case State.FirstDraw:
                if(playerManager.isFirstDrawEnd())
                {
                    // 最初ドローステートを非アクティブに
                    //firstDrawState.gameObject.SetActive(false);
                    state = State.PointDraw;
                }
                break;
            case State.PointDraw:
                // ポイント
                if (pointManager.Next())
                {
                    state = State.End;
                    pointText.text = "終了";
                }
                else
                {
                    currentPoint = pointManager.GetCurrentPoint();
                    pointText.text = currentPoint.ToString();
                    restPointText.text = (pointManager.step + 1).ToString();
                    // あいこ分を足す
                    foreach (int add in aikoPoint)
                    {
                        currentPoint += add;
                        pointText.text += " + " + add;
                    }
                    state = State.Draw;
                }
                break;
            case State.Draw:
                // 1ターン目もしくは3ターンごとに1枚引く
                if(turn++ % 3 == 0)
                {
                    playerManager.Draw();
                }
                state = State.SetStriker;
                playerManager.SetState(PlayerState.SetStriker);
                break;
            case State.SetStriker:
                if(playerManager.isSetStrikerEnd())
                {
                    state = State.BeforeOpen;
                }
                break;
            case State.BeforeOpen:
                state = State.Open;
                break;
            case State.Open:
                state = State.AfterOpen;
                break;
            case State.AfterOpen:
                state = State.Result;
                break;
            case State.Result:
                if (playerManager.StrikerBattle(currentPoint))
                    aikoPoint.Clear();
                else
                    aikoPoint.Add(pointManager.GetCurrentPoint());
                state = State.TurnEnd;
                break;
            case State.TurnEnd:
                playerManager.SetState(PlayerState.TurnEnd);
                state = State.PointDraw;
                break;
            case State.End:

                break;
            default:
                break;
        }


    }

    public void HandleMessage(MessageInfo message)
    {
        switch (message.messageType)
        {
            //case MessageType.NetPlay:
            //    // ロビー開始
            //    //uiManager.AppearLobby();
            //    networkManager.gameObject.SetActive(true);
            //    offlinePlayers.SetActive(false);
            //    state = State.Matching;
            //    return;

            //case MessageType.OfflinePlay:
            //    networkManager.gameObject.SetActive(false);
            //    offlinePlayers.SetActive(true);
            //    state = State.Start;
            //    return;

            case MessageType.SyncPoint:
                {
                    if (message.exInfo == null)
                        return;

                    // byte[]→構造体
                    PointInfo pointInfo = new PointInfo();
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(pointInfo));
                    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(pointInfo));
                    pointInfo = (PointInfo)Marshal.PtrToStructure(ptr, pointInfo.GetType());
                    Marshal.FreeHGlobal(ptr);

                    pointManager.randomIndexArray = pointInfo.points;
                }
                return;
        }

        playerManager.HandleMessage(message);
    }
}
