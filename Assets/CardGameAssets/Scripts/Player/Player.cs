using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Start,          // 開始
    FirstDraw,      // 最初のドロー、マリガン
    PointDraw,      // 点数カードをめくる
    Draw,           // 最初のターンと3ターンずつドローフェイズがある
    SetStriker,     // ファイターをセットする、イベントを発動などができる、互いがファイターを伏せ、準備完了となるまで難解でもファイターをセットしなおすことができる。
    BeforeOpen,     // オープン前
    Open,           // オープン
    AfterOpen,      // オープン後
    TurnEnd,        // ターン終了
    End             // 勝ったプレイヤーに点数を与える、相内の場合は次へ持ち越し(もう点数山札ないときは終了)
}

public class Player : MonoBehaviour
{
    public float moveSpeed = 1;

    public Vector3 move;
    //Transform cashTransform;

    public PlayerState state { get; private set; }                       // プレイヤーのステート
    public DeckData deckData = new DeckData();             // デッキの情報
    public DeckManager deckManager { get; private set; }    // デッキ管理さん(ドローとか)
    //public List<Card> hand { get; private set; }

    public CardObjectManager cardObjectManager;
    public PlayerManager playerManager { get; private set; }     // プレイヤー管理する人の実体

    readonly int firstDrawCount = 7;                            // 最初のドローする枚数

    public int playerID;    // ネット的なID
    public bool isMyPlayer; // 自分が操作しているプレイヤーかどうか

    public bool isFirstDrawEnd;
    public bool isSynced;
    public bool isPushedJunbiKanryo { get; private set; }

    public void JunbiKanryoON()
    {
        isPushedJunbiKanryo = true;
        var striker = GetFieldStrikerCard();
        if(striker != null)
        {
            jissainoPower = striker.power;
        }
    }
    //public void JubikanryoOFF()
    //{
    //    isPushedJunbiKanryo = false;
    //}

    public float waitTimer;

    public int jissainoPower;   // イベントとかでいろいろ演算した後の実際のパワー

    // Use this for initialization
    void Start ()
    {
        deckManager = new DeckManager();
        //hand = new List<Card>();


        // 仮でデッキデータ作成
        deckData.fighterCards[0] = CardDataBase.GetCardData(0);
        deckData.fighterCards[1] = CardDataBase.GetCardData(1);
        deckData.fighterCards[2] = CardDataBase.GetCardData(2);
        deckData.fighterCards[3] = CardDataBase.GetCardData(3);
        deckData.fighterCards[4] = CardDataBase.GetCardData(4);
        deckData.fighterCards[5] = CardDataBase.GetCardData(5);
        deckData.fighterCards[6] = CardDataBase.GetCardData(6);
        deckData.fighterCards[7] = CardDataBase.GetCardData(7);
        deckData.fighterCards[8] = CardDataBase.GetCardData(8);
        deckData.fighterCards[9] = CardDataBase.GetCardData(9);
        deckData.jorkerCard = CardDataBase.GetCardData(10);

        // キャッシュ
        //cashTransform = transform;

        // 初期化
        Restart();

        // プレイヤーマネージャーに追加
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        playerID = playerManager.AddPlayer(this);
	}

    public void Restart()
    {
        // デッキ管理さん初期化
        deckManager.Start(this, cardObjectManager);
        deckManager.Reset(deckData);

        var controller = GetComponent<PlayerController>();
        if(controller)
        {
            controller.Restart();
        }

        var AI = GetComponent<AIController>();
        if(AI)
        {
            AI.Restart();
        }

        // その他
        isFirstDrawEnd = false;
        isSynced = false;
        isPushedJunbiKanryo = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //var axis = Vector2.zero;

        //// 入力
        //if (Input.GetKey(KeyCode.UpArrow))   axis.y += 1;
        //if (Input.GetKey(KeyCode.DownArrow)) axis.y -= 1;
        //if (Input.GetKey(KeyCode.LeftArrow)) axis.x -= 1;
        //if (Input.GetKey(KeyCode.RightArrow))axis.x += 1;

        //// 軸正規化
        //if(axis != Vector2.zero) axis.Normalize();

        //// 移動量作成
        //move.x = axis.x * moveSpeed;
        //move.z = axis.y * moveSpeed;

        //var tapObject = oulInput.GetTapObject3D();
        //var card = tapObject.GetComponent<Card>();
        //if(card)
        //{
        //    
        //}
    }

    void FixedUpdate()
    {
    //    // 座標更新処理
    //    cashTransform.LookAt(cashTransform.localPosition + move);
    //    cashTransform.localPosition += move;
    //    move *= 0.5f;
    }

    public void ChangeState(PlayerState newState)
    {
        isSynced = false;

        // Exit処理
        switch (state)
        {
            case PlayerState.SetStriker:
                playerManager.uiManager.DisAppearWaitYouUI();
                break;
        }


        state = newState;

        // Enter処理
        switch (newState)
        {
            case PlayerState.Start:
                break;
            case PlayerState.FirstDraw:
                FirstDraw();
                break;
            case PlayerState.PointDraw:
                break;
            case PlayerState.Draw:
                break;
            case PlayerState.SetStriker:
                waitTimer = 0;
                break;
            case PlayerState.BeforeOpen:
                break;
            case PlayerState.Open:
                break;
            case PlayerState.AfterOpen:
                break;
            case PlayerState.TurnEnd:
                isPushedJunbiKanryo = false;
                deckManager.TurnEnd();
                // UI更新
                playerManager.uiManager.UpdateDeckUI(deckManager, isMyPlayer);
                break;
            default:
                break;
        }

    }

    public void FirstDraw()
    {
        // 最初の7枚ドロー
        deckManager.Draw(firstDrawCount);
    }

    public void Marigan()
    {
        // 手札とか山札リセット
        deckManager.Reset(deckData);
        // ドロー
        FirstDraw();
        // ドロー終了
        isFirstDrawEnd = true;

        // 手札・デッキ情報送信
        SendSyncDeckInfo();
    }

    public void NoMarigan()
    {
        isFirstDrawEnd = true;

        // 手札・デッキ情報送信
        SendSyncDeckInfo();
    }

    public void SendSyncDeckInfo()
    {
        // ネットワーク状態なら自分で相手のデッキ情報を送信するべきではない。
        if(MessageManager.isNetwork)
        {
            if (!isMyPlayer) return;
        }

        // カード情報をネットに送って相手と同期させる
        // ex構造体作成
        SyncDeckInfo exInfo = new SyncDeckInfo();
        exInfo.hand = new int[9];
        exInfo.yamahuda = new int[15];
        exInfo.bochi = new int[15];
        exInfo.tuihou = new int[15];

        // 手札同期
        var num = deckManager.GetNumHand();
        if (num == 0) exInfo.hand[0] = -1;
        else if (num < 9) exInfo.hand[num] = -1;
        for (int i = 0; i < num; i++)
        {
            exInfo.hand[i] = deckManager.GetHandCard(i).id;
        }

        // 山札同期
        num = deckManager.GetNumYamahuda();
        if (num == 0) exInfo.yamahuda[0] = -1;
        else if (num < 15) exInfo.yamahuda[num] = -1;
        for (int i = 0; i < num; i++)
        {
            exInfo.yamahuda[i] = deckManager.GetYamahudaCard(i).id;
        }

        // 墓地同期
        num = deckManager.GetNumBochi();
        if (num == 0) exInfo.bochi[0] = -1;
        else if (num < 15) exInfo.bochi[num] = -1;
        for (int i = 0; i < num; i++)
        {
            exInfo.bochi[i] = deckManager.GetCemeteryCard(i).id;
        }

        // 追放同期
        num = deckManager.GetNumTsuihou();
        if (num == 0) exInfo.tuihou[0] = -1;
        else if (num < 15) exInfo.tuihou[num] = -1;
        for (int i = 0; i < num; i++)
        {
            exInfo.tuihou[i] = deckManager.GetExpulsionCard(i).id;
        }

        //Info1 exInfo;
        //exInfo.a = 114;
        //exInfo.b = 514;
        //exInfo.pos = new Vector3(810, 19, 19);
        // メッセージ送信
        MessageManager.Dispatch(playerID, MessageType.SyncDeck, exInfo);

    }

    public void Draw()
    {
        // 1枚ドロー
        deckManager.Draw(1);

        // UI更新
        playerManager.uiManager.UpdateDeckUI(deckManager, isMyPlayer);
    }

    public void SetCard(SetCardInfo info)
    {
        deckManager.FieldSet(info.handNo);
    }

    public void BackToHand(BackToHandInfo info)
    {
        deckManager.BackToHand(info);
    }

    public bool isSetStriker() { return deckManager.isSetStriker(); }


    public bool isSetStrikerOK()
    {
        return (isSetStriker() && cardObjectManager.isSetEndStriker() && isPushedJunbiKanryo);
    }

    public void SetStrikerTimeOver()
    {
        var controller = GetComponent<PlayerController>();
        Debug.Assert(controller != null, "controller is null.");
        controller.SetStrikerTimeOver();
    }

    public void OpenStriker()
    {
        cardObjectManager.fieldStrikerCard.Open();
    }

    // 手札や山札の情報のセット
    public void SyncDeck(SyncDeckInfo syncData)
    {
        // 手札・山札etc情報同期
        deckManager.Sync(syncData);
    }

    public CardData GetFieldStrikerCard()
    {
        return deckManager.fieldStrikerCard;
    }

    public bool isHaveStrikerCard()
    {
        return deckManager.isHaveStrikerCard();
    }
}
