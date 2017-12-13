using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1;

    public Vector3 move;
    //Transform cashTransform;

    public BaseEntityStateMachine<Player> stateMachine { get; private set; }                       // プレイヤーのステート
    public PlayerDeckData deckData;             // デッキの情報
    public DeckManager deckManager { get; private set; }    // デッキ管理さん(ドローとか)
    //public List<Card> hand { get; private set; }

    public CardObjectManager cardObjectManager;
    public PlayerManager playerManager { get; private set; }     // プレイヤー管理する人の実体

    public static readonly int noSetStrikerPower = -1;
    readonly int firstDrawCount = 6;                            // 最初のドローする枚数

    public int playerID;    // ネット的なID
    public bool isMyPlayer; // 自分が操作しているプレイヤーかどうか

    public bool isStateEnd;
    public bool isSyncDeck;
    public bool isPushedJunbiKanryo;
    bool isStart = false;

    public bool isPushedNextButton;
    
    // ステートマシン用
    public int step;
    public bool isMarigan;

    public void JunbiKanryoON()
    {
        isPushedJunbiKanryo = true;
    }

    public void PushedNextButtonON()
    {
        isPushedNextButton = true;
        //var striker = GetFieldStrikerCard();
        //if (striker != null)
        //{
        //    jissainoPower = striker.cardData.power;
        //}
        //else jissainoPower = noSetStrikerPower;
    }

    //public void JubikanryoOFF()
    //{
    //    isPushedJunbiKanryo = false;
    //}

    public float waitTimer;

    public int jissainoPower
    {
        get;
        private set;
    }
    public void SetPower(int power)
    {
        jissainoPower = power;
        var striker = GetFieldStrikerCard();
        if(striker)
        {
            striker.SetPower(power);
        }
    }
    // イベントとかでいろいろ演算した後の実際のパワー

    // Use this for initialization
    void Awake()
    {
        if (isStart) return;
        isStart = true;

        deckManager = new DeckManager();
        stateMachine = new BaseEntityStateMachine<Player>(this);
        stateMachine.currentState = PlayerState.None.GetInstance();
        //hand = new List<Card>();

        // 仮でデッキデータ作成
        //deckData = new PlayerDeckData();
        //deckData.strikerCards[0] = 0;
        //deckData.strikerCards[1] = 1;
        //deckData.strikerCards[2] = 2;
        //deckData.strikerCards[3] = 3;
        //deckData.strikerCards[4] = 4;
        //deckData.strikerCards[5] = 5;
        //deckData.strikerCards[6] = 6;
        //deckData.strikerCards[7] = 7;
        //deckData.strikerCards[8] = 8;
        //deckData.strikerCards[9] = 9;
        //deckData.jorkerCard = 10;
        //deckData.eventCards[0] = (int)IDType.NONE;
        //deckData.eventCards[1] = (int)IDType.NONE;
        //deckData.eventCards[2] = (int)IDType.NONE;
        //deckData.eventCards[3] = (int)IDType.NONE;

        // キャッシュ
        //cashTransform = transform;

        //// 初期化
        //Restart();

        InitializeDeck();

        // プレイヤーマネージャーに追加
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        playerID = playerManager.AddPlayer(this);
	}

    public void InitializeDeck()
    {
        // x番目のデッキを使用
        deckData = PlayerDataManager.GetPlayerData().GetDeckData(0);
        // ストライカーか抜けてたらの処理
        if (!deckData.isSetAllStriker())
        {
            deckData = new PlayerDeckData();
            deckData.strikerCards[0] = 0;
            deckData.strikerCards[1] = 1;
            deckData.strikerCards[2] = 2;
            deckData.strikerCards[3] = 3;
            deckData.strikerCards[4] = 4;
            deckData.strikerCards[5] = 5;
            deckData.strikerCards[6] = 6;
            deckData.strikerCards[7] = 7;
            deckData.strikerCards[8] = 8;
            deckData.strikerCards[9] = 9;
            deckData.jorkerCard = 10;
            deckData.eventCards[0] = (int)IDType.NONE;
            deckData.eventCards[1] = (int)IDType.NONE;
            deckData.eventCards[2] = (int)IDType.NONE;
            deckData.eventCards[3] = (int)IDType.NONE;
        }
    }

    public void Restart()
    {
        if (!isStart) Awake();

        // デッキ管理さん初期化
        deckManager.Start(this, cardObjectManager);
        deckManager.Reset();
        deckManager.SetDeckData(deckData);

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
        isStateEnd = false;
        isSyncDeck = false;
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

        // ステートマシン更新
        stateMachine.Update();
    }

    void FixedUpdate()
    {
    //    // 座標更新処理
    //    cashTransform.LookAt(cashTransform.localPosition + move);
    //    cashTransform.localPosition += move;
    //    move *= 0.5f;
    }

    public void ChangeState(BaseEntityState<Player> newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void FirstDraw()
    {
        // 最初の7枚ドロー
        deckManager.Draw(firstDrawCount);
    }

    public void Marigan(SyncDeckInfo info)
    {
        // 手札とか山札リセット
        //deckManager.SetDeckData(deckData);
        deckManager.Marigan(info);

        isMarigan = true;

        // 手札・デッキ情報送信
        //SendSyncDeckInfo();
    }

    public void NoMarigan()
    {
        isMarigan = false;

        isStateEnd = true;

        // 手札・デッキ情報送信
        //SendSyncDeckInfo();
    }

    public SyncDeckInfo GetSyncDeckInfo()
    {
        // ex構造体作成
        SyncDeckInfo info = new SyncDeckInfo();
        int num;
        //exInfo.hand = new int[9];

        //exInfo.bochi = new int[15];
        //exInfo.tuihou = new int[15];

        //// 手札同期
        //if (num == 0) exInfo.hand[0] = -1;
        //else if (num < 9) exInfo.hand[num] = -1;
        //for (int i = 0; i < num; i++)
        //{
        //    exInfo.hand[i] = deckManager.GetHandCard(i).id;
        //}

        // 山札同期
        info.yamahuda = new int[15];
        num = deckManager.GetNumYamahuda();
        if (num == 0) info.yamahuda[0] = -1;
        else if (num < 15) info.yamahuda[num] = -1;
        for (int i = 0; i < num; i++)
        {
            info.yamahuda[i] = deckManager.GetYamahudaCard(i).id;
        }

        //// 墓地同期
        //num = deckManager.GetNumBochi();
        //if (num == 0) exInfo.bochi[0] = -1;
        //else if (num < 15) exInfo.bochi[num] = -1;
        //for (int i = 0; i < num; i++)
        //{
        //    exInfo.bochi[i] = deckManager.GetCemeteryCard(i).id;
        //}

        //// 追放同期
        //num = deckManager.GetNumTsuihou();
        //if (num == 0) exInfo.tuihou[0] = -1;
        //else if (num < 15) exInfo.tuihou[num] = -1;
        //for (int i = 0; i < num; i++)
        //{
        //    exInfo.tuihou[i] = deckManager.GetExpulsionCard(i).id;
        //}

        //Info1 exInfo;
        //exInfo.a = 114;
        //exInfo.b = 514;
        //exInfo.pos = new Vector3(810, 19, 19);

        return info;
    }

    public void SendSyncDeckInfo()
    {
        // ネットワーク状態なら自分で相手のデッキ情報を送信するべきではない。
        if(MessageManager.isNetwork)
        {
            if (!isMyPlayer) return;
        }

        // カード情報をネットに送って相手と同期させる
        var exInfo = GetSyncDeckInfo();

        // メッセージ送信
        MessageManager.Dispatch(playerID, MessageType.SyncDeck, exInfo);

    }

    //public void Draw()
    //{
    //    // 1枚ドロー
    //    deckManager.Draw(1);

    //    // UI更新
    //    playerManager.uiManager.UpdateDeckUI(deckManager, isMyPlayer);
    //}

    public void SetCard(SelectCardIndexInfo info)
    {
        deckManager.FieldSet(info.index);
    }

    public void SetSupport(SelectCardIndexInfo info)
    {
        deckManager.SetSupport(info.index);
    }

    public void SetIntercept(SelectCardIndexInfo info)
    {
        deckManager.SetIntercept(info.index);
    }

    public void BackToHand(BackToHandInfo info)
    {
        deckManager.BackToHand(info);
    }

    public bool isSetStriker() { return deckManager.isSetStriker(); }


    public bool isSetStrikerOK()
    {
        return (/*isSetStriker() && cardObjectManager.isSetEndStriker() && */!cardObjectManager.isInMovingState() && isPushedJunbiKanryo);
    }

    public void SetStrikerTimeOver()
    {
        var controller = GetComponent<PlayerController>();
        Debug.Assert(controller != null, "controller is null.");
        controller.SetStrikerTimeOver();
    }

    public void OpenStriker()
    {
        var card = cardObjectManager.fieldStrikerCard;
        if (card) card.Open();
    }

    public void AttackStriker()
    {
        var card = cardObjectManager.fieldStrikerCard;
        if (card) card.Attack();
    }

    // 手札や山札の情報のセット
    public void SyncDeck(SyncDeckInfo syncData)
    {
        // 相手から送られてくる前提のコード、

        // 手札・山札etc情報同期
        deckManager.Sync(syncData);
    }

    public Card GetFieldStrikerCard()
    {
        return cardObjectManager.fieldStrikerCard;
    }

    public CardData GetFieldEventCard()
    {
        return deckManager.fieldEventCard;
    }

    public bool isHaveStrikerCard()
    {
        return deckManager.isHaveStrikerCard();
    }

    public bool isHaveInterceptCard()
    {
        return deckManager.isHaveInterceptCard();
    }
}
