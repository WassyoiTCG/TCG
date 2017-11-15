using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDeck : MonoBehaviour {

    // メンバ変数
    public BaseEntityStateMachine<SceneDeck> m_pStateMachine;

    // カード表示ニキ
    public GameObject/*uGUICard*/ m_uGUICard;

    //　カードのプレート (カード最大分配列用意 
    public GameObject CardPlatePrefab;
    private GameObject[] m_aCardPlate = new GameObject[PlayerDeckData.deckMax];
    private GameObject[] m_aMyDeckCard = new GameObject[PlayerDeckData.deckMax];

    //  コレクションカード表示
    public GameObject CardPrefab;
    private GameObject[] m_aCollectCard = new GameObject[SelectData.DECK_COLLECTCARD_MAX];

    protected GameObject DragObj;
    public GameObject TouchCardObj;// タッチしたカードのアドレス保存用

    //  キャンバスさんも取得
    public GameObject Canvas;           // (11/12) いらなくなったかも 
    public GameObject CollectCardGroup; // コレクトカードを入れる親
    public GameObject DeckCardGroup;    // デッキカードを入れる親

    private bool m_bCardTap = false;// カード押したか？
    public bool IsCardTapFlag() { return m_bCardTap; }
    public void SetCardTapFlag(bool flag) { m_bCardTap = flag; }

    public Vector3 m_vTapPos;
    public Vector3 m_vCurrentPos;


    // Use this for initialization
    void Start () {
        m_vTapPos = new Vector3(0, 0, 0);
        m_vCurrentPos = new Vector3(0, 0, 0);

        

        m_bCardTap = false;

        // (TODO)(A列車)システム初期化  を全て共通に  
        // oulSystem;

        // カードのデータベースを初期化
        CardDataBase.Start();

        CardData data = CardDataBase.GetCardData(3);    // ID0番目のカードデータを取ってくる。
        CardData[] all = CardDataBase.GetCardList();    // カード情報を全て取得する。
        int size = all.Length;  // カードの最大数

        //　★Findは非アクティブ状態ではとれないから取ってから消すやで
        m_uGUICard = GameObject.Find("Canvas/uGUICard");    //.GetComponent<uGUICard>();
        m_uGUICard.SetActive(false);                        // 取得したから消すやで
        m_uGUICard.GetComponent<uGUICard>().SetCardData(data);

        for (int i = 0; i < PlayerDeckData.deckMax; i++)
        {
            // 変更前
            // m_pCardPlateArray[i] = (GameObject)Instantiate(CardPlatePrefab);
            // m_pCardPlateArray[i].transform.SetParent(, false);

            /*
            差異としては、as句の方が高速であることと、失敗時にキャスト演算は例外を発生させas句はnullを返します。
            例外は処理するのにもコストがかかるので、特別な事が無い限りはこの場合as句を利用するのが良いです。 
            */

            // 変更後
            m_aCardPlate[i] = Instantiate(CardPlatePrefab, DeckCardGroup.transform) as GameObject;
            m_aMyDeckCard[i] = Instantiate(CardPrefab, DeckCardGroup.transform) as GameObject;
            m_aMyDeckCard[i].GetComponent<uGUICard>().SetMyDeck(true);// ★★★このカードは自分のデッキかどうか

            int StartX = 100; int StartY = 130;
            m_aCardPlate[i].transform.localPosition = new Vector2(StartX + (150 * (i % 5)), StartY + (i / 5) * -185);
            m_aMyDeckCard[i].transform.localPosition = new Vector2(StartX + (150 * (i % 5)), StartY + (i / 5) * -185);


        }



        // (TODO)自分のデッキやで　仮0スロット目のデッキのカードのID取得
        int[] myDeckID = PlayerDataManager.GetPlayerData().GetDeckData(0).GetArray();

        // デッキ
        for (int i = 0; i < PlayerDeckData.deckMax/*PlayerDeckData.numStriker*/; i++)
        {
            // (11/13)(TODO)取得したデータが何も書かれていなかったら
            // SetActiveで消す

            int id = myDeckID[i];
            //CardType type = CardType.Fighter;
            m_aMyDeckCard[i].GetComponent<uGUICard>().SetCardData(CardDataBase.GetCardData(id)); //;= strikerCards[i];
          
        }

        // コレクション
        for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
        {

            m_aCollectCard[i] = Instantiate(CardPrefab, CollectCardGroup.transform) as GameObject;

            int StartX = -620; int StartY = 128;
            m_aCollectCard[i].transform.localPosition = new Vector2(StartX + (150 * (i % 4)), StartY + (i / 4) * -185);

            m_aCollectCard[i].GetComponent<uGUICard>().SetCardData(all[i]);

            // (11/13)うんこソート
            // 先にデッキに入っていないカード状態に
            m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_Off();
            for (int i2 = 0; i2 < PlayerDeckData.deckMax; i2++)
            {
                // ★コレクションにあるカードがすでに被ってたら暗くするソート
                if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.id == m_aMyDeckCard[i2].GetComponent<uGUICard>().cardData.id)
                {
                    // デッキに入っている状態
                    m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_On();
                    break;
                }
                    
                
            }// ソート

        }

        // カードタイプ
        if (CardDataBase.GetCardData(11).cardType == CardType.Fighter)
        {
            int a = 0;
            a++;
        }
        if (CardDataBase.GetCardData(11).cardType == CardType.Intercept)
        {
            int b = 0;
            b++;
        }

        //m_aMyDeckCard[PlayerDeckData.numStriker].GetComponent<uGUICard>().SetCardData;//PlayerDeckData.jorkerCard;
        //for (int i = 0; i < PlayerDeckData.numEvent; i++)
        //{
        //    cards[numStriker + numJoker + i] = eventCards[i];
        //}




        // ステートマシンの初期化や切り替えは最後に行う
        m_pStateMachine = new BaseEntityStateMachine<SceneDeck>(this);

        m_pStateMachine.globalState = SceneDeckState.Global.GetInstance();
        m_pStateMachine.ChangeState(SceneDeckState.Intro.GetInstance());
        return; 
    }

    // Update is called once per frame
    void Update () {
        // これより上に書かない。
        oulInput.Update();

        //GUI.Label(
        //new Rect(0.0f, 0.0f, Screen.width, Screen.height),
        //"マウスX: ");

        // アニメーション用
        CollectCardGroup.GetComponent<ScreenOutAppeared>().SelfUpdate();

        m_pStateMachine.Update();
        return;

    }


    // ラインボタンに触れた時
    public void ClickLineButton(int no)
    {
        ChangeLine tagLineChange;

        // セレクトナンバー設定
        tagLineChange.iNextLine = no;

        //ClickMenuButton;
        switch ((CHANGE_LINE_TYPE)no)
        {
            case CHANGE_LINE_TYPE.BACK:
                break;
            case CHANGE_LINE_TYPE.NEXT:
                break;
            case CHANGE_LINE_TYPE.END:
                Debug.LogWarning("SceneMenu: それ以上のタイプはない。");
                break;
            default:
                break;
        }



        // メッセージ作成
        var message = new MessageInfo();
        message.messageType = MessageType.ClickLineButton;

        // エクストラインフォに構造体を詰め込む
        message.SetExtraInfo(tagLineChange);

        HandleMessge(message);
    }

    // ライン変更
    public void ChangeLine(int iNextLine)
    {
        // (11/13)(TODO)この処理だと全部取ってきて重いので何とかする。
        // 解決案　最大サイズ取得したいだけなので最初にInt型にサイズを保存しておく。
        CardData[] all = CardDataBase.GetCardList();    // カード情報を全て取得する。
        int iMaxSize = all.Length;  // カードの最大数


        //  セレクトタイプによってステート変更
        switch ((CHANGE_LINE_TYPE)iNextLine)
        {
            case CHANGE_LINE_TYPE.BACK:

                SelectData.iDeckCollectLineNo -= 1;
                Debug.Log("SceneCollectState: LINE移動↑(マイナス)");

                //+--------------------------------------------
                //  ラインが0より低ければ0にして何もせずReturun
                if (SelectData.iDeckCollectLineNo <= -1)
                {
                    Debug.Log("ChangeLine:0よりマイナス行こうとしたため0に戻しReturun ");

                    SelectData.iDeckCollectLineNo = 0;
                    return;
                };

                // アニメーション開始
                CollectCardGroup.GetComponent<ScreenOutAppeared>().SetPos(new Vector3(0, 600, 0));
                //CollectCardGroup.GetComponent<ScreenOutAppeared>().SetNextPos(new Vector3(0, 0, 0));
                CollectCardGroup.GetComponent<ScreenOutAppeared>().Action();

                break;
            case CHANGE_LINE_TYPE.NEXT:
                SelectData.iDeckCollectLineNo += 1;
                Debug.Log("SceneCollectState: LINE移動↓(プラス)");

                //+--------------------------------------------
                //  ラインが0より低ければ0にして何もせずReturun
                if ((SelectData.iDeckCollectLineNo * SelectData.DECK_COLLECTCARD_MAX) >= iMaxSize)
                {
                    Debug.Log("ChangeLine:カード最大数を超えた空間に移動しようとしていたためReturn");
                    SelectData.iDeckCollectLineNo -= 1;
                    return;
                };

                // アニメーション開始
                CollectCardGroup.GetComponent<ScreenOutAppeared>().SetPos(new Vector3(0, -600, 0));
                //CollectCardGroup.GetComponent<ScreenOutAppeared>().SetNextPos(new Vector3(0, 0, 0));
                CollectCardGroup.GetComponent<ScreenOutAppeared>().Action();

                break;
            case CHANGE_LINE_TYPE.END:
                Debug.LogWarning("SceneCollectState: それ以上のタイプはない。");
                break;
            default:
                break;
        }

 


        int iNextNo = SelectData.iDeckCollectLineNo * SelectData.DECK_COLLECTCARD_MAX;

  
  
        for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
        {
            m_aCollectCard[i].SetActive(true);//絵が表示されていない可能性があるため毎回True

            // 最大カード数を超えたら
            if ((iNextNo + i) >= iMaxSize)
            {
                m_aCollectCard[i].SetActive(false); //絵を表示しない
                continue;
            }

            m_aCollectCard[i].GetComponent<uGUICard>().SetCardData(CardDataBase.GetCardData(iNextNo + i));



            // (11/13)最後にいつものうんこソート
            // 先にデッキに入っていないカード状態に
            m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_Off();
            for (int i2 = 0; i2 < PlayerDeckData.deckMax; i2++)
            {
                // ★コレクションにあるカードがすでに被ってたら暗くするソート
                if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.id == m_aMyDeckCard[i2].GetComponent<uGUICard>().cardData.id)
                {
                    // デッキに入っている状態
                    m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_On();
                    break;
                }
            }// ソート

        }

        

     }

    //+--------------------------------------
    // デッキのカード変更
    public void DeckSet()
    {
        uGUICard GraspCard = m_uGUICard.GetComponent<uGUICard>();

        // 握ってるカードがファイターなら
        if (GraspCard.cardData.cardType == CardType.Fighter)
        {
            // パワーと同じ場所の所へ書き換え
            int iGraspCardPower = GraspCard.cardData.power;
            m_aMyDeckCard[iGraspCardPower - 1].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);

        }

        // 握ってるカードがJOKERなら
        if (GraspCard.cardData.cardType == CardType.Joker)
        {
            // JOKERゾーンを書き換え
            m_aMyDeckCard[PlayerDeckData.numStriker].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);

        }

        // 握ってるカードがイベントなら
        if (GraspCard.cardData.cardType == CardType.Support ||
        GraspCard.cardData.cardType == CardType.Connect ||
        GraspCard.cardData.cardType == CardType.Intercept)
        {
            // イベントゾーンを書き換え
            int iStartIndex = PlayerDeckData.numStriker + PlayerDeckData.numJoker;

            //// (TODO)エラー処理、後で消す
            //for (int i = 0; i < PlayerDeckData.numEvent; i++)
            //{
            //    if (m_aMyDeckCard[iStartIndex + i].GetComponent<uGUICard>().cardData.id == -1) 
            //    {
                    
            //    }

            //}

            // イベント
            for (int i = 0; i < PlayerDeckData.numEvent; i++)
            {
                // 空き枠を探す
                if (m_aMyDeckCard[iStartIndex + i].GetComponent<uGUICard>().cardData.id == (int)IDType.NONE)
                {
                    Debug.Log(" イベント枠の空きを発見！-ChangeDeck");
                    m_aMyDeckCard[iStartIndex].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);
                }
                
            }
           

        }

        // いべんとやる　でっきからこれうともやる

        //+-----------------------------------------------------------------------
        // (11/13)(TODO)この処理だと全部取ってきて重いので何とかする。
        // 解決案　最大サイズ取得したいだけなので最初にInt型にサイズを保存しておく。
        //CardData[] all = CardDataBase.GetCardList();    // カード情報を全て取得する。
        //int iMaxSize = all.Length;  // カードの最大数

        int iNextNo = SelectData.iDeckCollectLineNo * SelectData.DECK_COLLECTCARD_MAX;
        for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
        {
            if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.id == GraspCard.cardData.id)
            {
                // デッキに入っている状態に
                m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_On();
                break;
            }

        }


    }


    //+--------------------------------------
    // デッキのカードを外す
    public void DeckOut()
    {
        uGUICard GraspCard = m_uGUICard.GetComponent<uGUICard>();

        // デッキ15枚から握っているIDを探し消す
        for (int i = 0; i < PlayerDeckData.deckMax; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.id ==
                 GraspCard.GetComponent<uGUICard>().cardData.id)
            {
                Debug.Log(" 握っているID発見、消します。-DeckOut");
                //m_aMyDeckCard[i].GetComponent<uGUICard>().NoneCard();
                m_aMyDeckCard[i].GetComponent<uGUICard>().MissingCard();
            }

        }



        //+-----------------------------------------------------------------------
        // (11/13)(TODO)この処理だと全部取ってきて重いので何とかする。
        // 解決案　最大サイズ取得したいだけなので最初にInt型にサイズを保存しておく。
        //CardData[] all = CardDataBase.GetCardList();    // カード情報を全て取得する。
        //int iMaxSize = all.Length;  // カードの最大数

        int iNextNo = SelectData.iDeckCollectLineNo * SelectData.DECK_COLLECTCARD_MAX;
        for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
        {
            if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.id == GraspCard.cardData.id)
            {
                // ★デッキに入っていない状態に
                m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_Off();
                break;
            }

        }


    }

    //+--------------------------------------
    // ★★★デッキのセーブ
    public void DeckSave()
    {
        // まず15毎のIDを保存
        int[] allDeckData = new int[15];
        for (int i = 0; i < PlayerDeckData.deckMax; i++)
        {
            allDeckData[i] = m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.id;
        }
        
    // (11/14)(TODO) 仮で0番目セーブ
        PlayerDataManager.DeckSave(0, allDeckData);

    }



    public bool HandleMessge(MessageInfo message)
    {
        return m_pStateMachine.HandleMessage(message);
    }

}
