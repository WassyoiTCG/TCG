﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDeck : MonoBehaviour
{
    // メンバ変数
    public BaseEntityStateMachine<SceneDeck> m_pStateMachine;

    // カード表示ニキ
    public GameObject/*uGUICard*/ m_uGraspCard;
    public GameObject/*uGUICard*/ m_uGraspVanishCard;
    public GameObject/*uGUICard*/ m_uDetailCard;

    //　カードのプレート (カード最大分配列用意 
    public GameObject CardPlatePrefab;
    // private GameObject[] m_aCardPlate = new GameObject[PlayerDeckData.deckMax];
    public GameObject[] m_aMyDeckCard = new GameObject[PlayerDeckData.deckMax];

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

    public GameObject EffectRiipleCard;
    public GameObject EffectRiipleCardBack;
    public GameObject EffectRiipleGraspCardBack;

    public GameObject Arrow;
    public GameObject DeckPlateBoom;

    public GameObject BlackPanel;
    //+-----------------------------------
    // 詳細用グル-プ
    public GameObject DetailGroup;
    public Text NameText;
    public Text ClassText;
    public Text TypeText;
    public Text AbilityText;

    public GameObject SaveText;
    public GameObject DontSaveText;

    public Text SeigenNoText;

    // Use this for initialization
    void Start()
    {
        SelectData.iDeckCollectLineNo = 0;

        //TouchCardObj.GetComponent<uGUICard>().SetCardData(CardDataBase.GetCardData(4)); 
        //Arrow.SetActive(false);

        m_vTapPos = new Vector3(0, 0, 0);
        m_vCurrentPos = new Vector3(0, 0, 0);

        m_bCardTap = false;

        // (TODO)(A列車)システム初期化  を全て共通に  
        oulSystem.Initialize();

        //  BGM
        oulAudio.PlayBGM("CollectionBGM", true);

        // カードのデータベースを初期化
        //CardDataBase.Start();

        CardData data = CardDataBase.GetCardData(3);                // ID0番目のカードデータを取ってくる。
        CardData[] all = CardDataBase.GetDeckSortedCardData();      // カード情報を全て取得する。1/12 デッキ選択用のソートした配列を取得
        int size = all.Length;  // カードの最大数

        //　★Findは非アクティブ状態ではとれないから取ってから消すやで
        //m_uGraspCard=gameObject
        //m_uGraspCard = GameObject.Find("Canvas/uGUICard");    //.GetComponent<uGUICard>();
        m_uGraspCard.GetComponent<uGUICard>().SetCardData(data);
        //m_uGraspCard.GetComponent<Ripple>().SetAlpha(1.0f); // 透明に
        m_uGraspCard.SetActive(false);                        // 取得したから消すやで


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
            //m_aCardPlate[i] = Instantiate(CardPlatePrefab, DeckCardGroup.transform) as GameObject;
            m_aMyDeckCard[i] = Instantiate(CardPrefab, DeckCardGroup.transform) as GameObject;
            m_aMyDeckCard[i].GetComponent<uGUICard>().SetMyDeck(true);// ★★★このカードは自分のデッキかどうか

            int StartX = 100; int StartY = 130;
            //m_aCardPlate[i].transform.localPosition = new Vector2(StartX + (150 * (i % 5)), StartY + (i / 5) * -185);
            m_aMyDeckCard[i].transform.localPosition = new Vector2(StartX + (150 * (i % 5)), StartY + (i / 5) * -185);

            // マイデッキ
            m_aMyDeckCard[i].GetComponent<ScreenOutAppeared>().SetNextPos(new Vector3(StartX + (150 * (i % 5)), StartY + (i / 5) * -185, 0.0f));



        }



        // (TODO)自分のデッキやで　仮0スロット目のデッキのカードのID取得
        int[] myDeckID = PlayerDataManager.GetPlayerData().GetDeckData(0).GetArray();

        // ★デッキデータ
        for (int i = 0; i < PlayerDeckData.deckMax; i++)
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

        }// forコレクション

        EventMANTAN();
        AbilityMANTAN();
              //// カードタイプ
              //if (CardDataBase.GetCardData(11).cardType == CardType.Fighter)
              //{
              //    int a = 0;
              //    a++;
              //}
              //if (CardDataBase.GetCardData(11).cardType == CardType.Intercept)
              //{
              //    int b = 0;
              //    b++;
              //}

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

    // 終了時
    void OnDisable()
    {
        //  BGM
        oulAudio.StopBGM();

    }

    // Update is called once per frame
    void Update()
    {
        // これより上に書かない。
        oulInput.Update();

        SeigenNoTextUpdate();
        //GUI.Label(
        //new Rect(0.0f, 0.0f, Screen.width, Screen.height),
        //"マウスX: ");

        // アニメーション用
        CollectCardGroup.GetComponent<ScreenOutAppeared>().SelfUpdate();

        for (int i = 0; i < PlayerDeckData.deckMax; i++)
        {
            m_aMyDeckCard[i].GetComponent<ScreenOutAppeared>().SelfUpdate();
        }

        // エフェクト更新
        EffectRiipleCard.GetComponent<Ripple>().SelfUpdate();
        EffectRiipleCardBack.GetComponent<Ripple>().SelfUpdate();
        EffectRiipleGraspCardBack.GetComponent<Ripple>().SelfUpdate();


        // 持ってるカードの演出
        m_uGraspCard.GetComponent<BoyonAppeared>().SelfUpdate();
        m_uGraspCard.GetComponent<ScreenOutAppeared>().SelfUpdate();
        m_uGraspCard.GetComponent<ScaleAppeared>().SelfUpdate();

        m_uGraspVanishCard.GetComponent<Ripple>().SelfUpdate();
        m_uGraspVanishCard.GetComponent<uGUICard>().AlphaSetUpdate();


        Arrow.GetComponent<Shake>().SelfUpdate();
        DeckPlateBoom.GetComponent<AlphaWave>().SelfUpdate();

        m_pStateMachine.Update();

        return;

    }


    // 戻るボタンに触れた時
    public void ClickBackButton()
    {
        // メッセージ作成
        var message = new MessageInfo();
        message.messageType = MessageType.ClickBackButton;
        // エクストラインフォに構造体を詰め込む
        //message.SetExtraInfo(tagLineChange);
        HandleMessge(message);
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
        CardData[] all = CardDataBase.GetDeckSortedCardData();    // カード情報を全て取得する。1/12 デッキ選択用のソートした配列を取得
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

            // 01/12変更 ソートしたやつを左の表示をソートしたやつに変更
            m_aCollectCard[i].GetComponent<uGUICard>().SetCardData(CardDataBase.GetSortedCardData(iNextNo + i));



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
        EventMANTAN();
        AbilityMANTAN();
    }

    //+--------------------------------------
    // デッキのカード変更
    public void DeckSet()
    {
        uGUICard GraspCard = m_uGraspCard.GetComponent<uGUICard>();

        bool ChangeFlag = false;
        int ChangeCardID = 0;

        Vector3 EffectPos = new Vector3(0, 0, 0);

        // 握ってるカードがファイターor効果持ちファイターなら
        if (GraspCard.cardData.cardType == CardType.Fighter ||
            GraspCard.cardData.cardType == CardType.AbilityFighter)
        {
            // パワーと同じ場所の所へ書き換え
            int iGraspCardPower = GraspCard.cardData.power;

            // 変える前のカードに何か入ってたら
            if (m_aMyDeckCard[iGraspCardPower - 1].GetComponent<uGUICard>().cardData.id != (int)IDType.NONE)
            {
                ChangeFlag = true;
                ChangeCardID = m_aMyDeckCard[iGraspCardPower - 1].GetComponent<uGUICard>().cardData.id;
            }

            // 変える
            m_aMyDeckCard[iGraspCardPower - 1].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);


            // エフェクトポジション
            EffectPos = m_aMyDeckCard[iGraspCardPower - 1].transform.localPosition;

        }

        // 握ってるカードがJOKERなら
        if (GraspCard.cardData.cardType == CardType.Joker)
        {
            // 変える前のカードに何か入ってたら
            if (m_aMyDeckCard[PlayerDeckData.numStriker].GetComponent<uGUICard>().cardData.id != (int)IDType.NONE)
            {
                ChangeFlag = true;
                ChangeCardID = m_aMyDeckCard[PlayerDeckData.numStriker].GetComponent<uGUICard>().cardData.id;
            }

            // JOKERゾーンを書き換え
            m_aMyDeckCard[PlayerDeckData.numStriker].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);



            // エフェクトポジション
            EffectPos = m_aMyDeckCard[PlayerDeckData.numStriker].transform.localPosition;

        }

        // 握ってるカードがイベントなら
        if (GraspCard.cardData.cardType == CardType.Support ||
            GraspCard.cardData.cardType == CardType.Connect ||
            GraspCard.cardData.cardType == CardType.Intercept)
        {
            // イベントゾーンを書き換え
            int iStartIndex = PlayerDeckData.numStriker + PlayerDeckData.numJoker;

            // イベント
            for (int i = 0; i < PlayerDeckData.numEvent; i++)
            {
                // 空き枠を探す
                if (m_aMyDeckCard[iStartIndex + i].GetComponent<uGUICard>().cardData.id == (int)IDType.NONE)
                {
                    Debug.Log(" イベント枠の空きを発見！-ChangeDeck");
                    m_aMyDeckCard[iStartIndex + i].GetComponent<uGUICard>().SetCardData(GraspCard.cardData);

                    // エフェクトポジション
                    EffectPos = m_aMyDeckCard[iStartIndex + i].transform.localPosition;

                    break; // □一つ入れたらさいさいさいなら。
                }

                // 最後まで来てどこも枠が空いていなかったらReturn
                if (i == (PlayerDeckData.numEvent - 1))
                {
                    Debug.Log(" イベント入れれなかったら出ていけぇ！(仮)-ChangeDeck");
                    return;
                }

            }


        }

        // エフェクト
        EffectRiipleCard.transform.localPosition = EffectPos;
        EffectRiipleCard.GetComponent<Ripple>().Action();

        // SE
        oulAudio.PlaySE("edit_set");

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

        if (ChangeFlag == true)
        {

            iNextNo = SelectData.iDeckCollectLineNo * SelectData.DECK_COLLECTCARD_MAX;
            for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
            {
                if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.id == ChangeCardID)
                {
                    // ★デッキに入っていない状態に
                    m_aCollectCard[i].GetComponent<uGUICard>().NotGrasp_Off();

                    break;
                }

            }
        }

        EventMANTAN();
        AbilityMANTAN();

    }


    //+--------------------------------------
    // デッキのカードを外す
    public void DeckOut()
    {
        uGUICard GraspCard = m_uGraspCard.GetComponent<uGUICard>();

        m_uGraspVanishCard.GetComponent<uGUICard>().SetCardData(GraspCard.cardData);
        m_uGraspVanishCard.transform.localPosition = GraspCard.transform.localPosition;
        m_uGraspVanishCard.GetComponent<Ripple>().Action();
        //m_uGraspVanishCard.GetComponent<Ripple>().SetPos();

        // デッキ15枚から握っているIDを探し消す
        for (int i = 0; i < PlayerDeckData.deckMax; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.id ==
                 GraspCard.GetComponent<uGUICard>().cardData.id)
            {
                Debug.Log(" 握っているID発見、消します。-DeckOut");
                //m_aMyDeckCard[i].GetComponent<uGUICard>().NoneCard();

                // SE
                oulAudio.PlaySE("edit_out");

                m_aMyDeckCard[i].GetComponent<uGUICard>().MissingCard();

                // ★イベントは消したらずらす処理
                // 11以上のデータが消されたつまりイベントカードのデータを消したということ
                if (i >= PlayerDeckData.numStriker + PlayerDeckData.numJoker)
                {
                    //int iEventIndex = i++;  
                    // 消した後のカード達を動かしずらす処理 
                    for (int iEventIndex = i++; iEventIndex < PlayerDeckData.deckMax; iEventIndex++)
                    {

                        // 最後の配列の場合
                        if (iEventIndex == PlayerDeckData.deckMax - 1)
                        {
                            // 最後のカードを空白に
                            m_aMyDeckCard[iEventIndex].GetComponent<uGUICard>().MissingCard();

                        }
                        else
                        {
                            // まず次のデータを今のデータに保存
                            m_aMyDeckCard[iEventIndex].GetComponent<uGUICard>().SetCardData(m_aMyDeckCard[iEventIndex + 1].GetComponent<uGUICard>().cardData);

                        }

                        // 欠番はアニメさせない！　(なざならアニメをするとその関数の中でビョウガしてエウレシアが見えるから)
                        if (m_aMyDeckCard[iEventIndex].GetComponent<uGUICard>().cardData.id != (int)IDType.NONE)
                        {
                            m_aMyDeckCard[iEventIndex].transform.localPosition += new Vector3(120, 0, 0); //GetComponent<ScreenOutAppeared>().SetPos(new Vector3(400, 300, 0));
                                                                                                          // アニメーション常に実行(磁力の力だ！)
                            m_aMyDeckCard[iEventIndex].GetComponent<ScreenOutAppeared>().Action();
                        }



                    }

                }// イベントずらすif分


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

                // エフェクト
                EffectRiipleCardBack.transform.localPosition = m_aCollectCard[i].transform.localPosition;
                EffectRiipleCardBack.GetComponent<Ripple>().Action();

                EffectRiipleGraspCardBack.transform.localPosition = GraspCard.transform.localPosition;
                EffectRiipleGraspCardBack.GetComponent<Ripple>().Action();

                break;
            }

        }


        // イベント満タンを外す処理
        if (EventCardFullTankCheak() == false)// 逆に
        {
            // コレクションにあるイベント全部
            for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
            {
                if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.Support ||
                    m_aCollectCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.Connect ||
                    m_aCollectCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.Intercept)
                {
                    int iEventIndex = PlayerDeckData.numStriker + PlayerDeckData.numJoker;
                    bool bIDCheakOK = true;
                    for (int i2 = 0; i2 < PlayerDeckData.numEvent; i2++)
                    {
                        if (m_aMyDeckCard[iEventIndex + i2].GetComponent<uGUICard>().cardData.id ==
                        m_aCollectCard[i].GetComponent<uGUICard>().cardData.id)
                        {
                            bIDCheakOK = false;// デッキに同じIDあり
                        }

                    }

                    // IDがデッキになかったら
                    if (bIDCheakOK == true)
                    {
                        // 満杯マーク外す
                        m_aCollectCard[i].GetComponent<uGUICard>().EventFullInfo_Off();
                    }


                }

            }
        }// イベント満タンを外す処理

        // 効果満タンを外す処理
        if (AbilityCardFullTankCheak() == false)// 逆に
        {
            // コレクションにある＿＿全部
            for (int i = 0; i < SelectData.DECK_COLLECTCARD_MAX; i++)
            {
                if (m_aCollectCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.AbilityFighter)
                {
                    bool bIDCheakOK = true;
                    for (int i2 = 0; i2 < PlayerDeckData.numStriker; i2++)
                    {
                        if (m_aMyDeckCard[i2].GetComponent<uGUICard>().cardData.id ==
                        m_aCollectCard[i].GetComponent<uGUICard>().cardData.id)
                        {
                            bIDCheakOK = false;// デッキに同じIDあり
                        }

                    }

                    // IDがデッキになかったら
                    if (bIDCheakOK == true)
                    {
                        // 満杯マーク外す
                        m_aCollectCard[i].GetComponent<uGUICard>().AbilityFullInfo_Off();
                    }


                }

            }
        }// イベント満タンを外す処理
    }

    //+--------------------------------------
    // ★★★デッキのセーブ
    public void DeckSave()
    {


        bool bCheak = true;
        for (int i = 0; i < PlayerDeckData.numStriker + PlayerDeckData.numJoker; i++)
        {
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.id == (int)IDType.NONE )
            {
                bCheak = false;
            }
        }


        // 効果持ちカードの数値
        int iAbilityStrikerNom = 0;// PlayerDeckData.numStriker + PlayerDeckData.numJoker;
        for (int i = 0; i < PlayerDeckData.numStriker; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.AbilityFighter)
            {
                iAbilityStrikerNom++;
            }

        }// 効果
        // 5マイ入ってらアウト
        if (iAbilityStrikerNom >= 5)
        {
            bCheak = false;
        }

        if (bCheak)
        {
            // SE
            oulAudio.PlaySE("edit_end");


            // まず15毎のIDを保存
            int[] allDeckData = new int[15];
            for (int i = 0; i < PlayerDeckData.deckMax; i++)
            {
                allDeckData[i] = m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.id;
            }

            SaveText.GetComponent<TextAnimation>().Action();

            // (11/14)(TODO) 仮で0番目セーブ
            PlayerDataManager.DeckSave(0, allDeckData);

        }else 
        {
            DontSaveText.GetComponent<TextAnimation>().Action();
        }

    }

    // デッキにイベント満タンか
    public bool EventCardFullTankCheak()
    {

        // イベントカード上限いっぱいならカードを暗くする
        int iEventIndex = PlayerDeckData.numStriker + PlayerDeckData.numJoker;
        for (int i = 0; i < PlayerDeckData.numEvent; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[iEventIndex + i].GetComponent<uGUICard>().cardData.id == (int)IDType.NONE)
            {
                Debug.Log(" イベント空き枠まだあるやで。-EventCardFullTankCheak");
                return false; // まだイベント空き枠あるお。
            }

        }// イベント

        Debug.Log(" イベント空き枠もうない。-EventCardFullTankCheak");
        return true;    // 満タンや
    }

    // デッキにイベント満タンか
    public void EventMANTAN()
    {

        // イベント満タンやったら
        if (EventCardFullTankCheak() == true)
        {
            // コレクションにあるイベント全部暗くする
            for (int i3 = 0; i3 < SelectData.DECK_COLLECTCARD_MAX; i3++)
            {
                if (m_aCollectCard[i3].GetComponent<uGUICard>().cardData.cardType == CardType.Support ||
                    m_aCollectCard[i3].GetComponent<uGUICard>().cardData.cardType == CardType.Connect ||
                    m_aCollectCard[i3].GetComponent<uGUICard>().cardData.cardType == CardType.Intercept)
                {
                    int iEventIndex = PlayerDeckData.numStriker + PlayerDeckData.numJoker;
                    bool bIDCheakOK = true;
                    for (int i2 = 0; i2 < PlayerDeckData.numEvent; i2++)
                    {
                        if (m_aMyDeckCard[iEventIndex + i2].GetComponent<uGUICard>().cardData.id ==
                        m_aCollectCard[i3].GetComponent<uGUICard>().cardData.id)
                        {
                            bIDCheakOK = false;// デッキに同じIDあり
                        }

                    }

                    // IDがデッキになかったら
                    if (bIDCheakOK == true)
                    {
                        // 満杯マークや
                        m_aCollectCard[i3].GetComponent<uGUICard>().EventFullInfo_On();
                    }


                }

            }

        }// イベント満タンやったら

    }

    // デッキに効果満タンか
    public bool AbilityCardFullTankCheak()
    {

        // カード上限いっぱいならカードを暗くする
        int iAbilityStrikerNom = 0;// PlayerDeckData.numStriker + PlayerDeckData.numJoker;
        for (int i = 0; i < PlayerDeckData.numStriker; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.AbilityFighter)
            {
                iAbilityStrikerNom++;
            }

        }// 効果

        if (iAbilityStrikerNom >= PlayerDeckData.MaxAbilityStrikerNom)
        {
            Debug.Log(" 効果持ちモンスター空き枠もうない。-AbilityCardFullTankCheak");
            return true;    // 満タンや
        }

        Debug.Log(" 効果持ちモンスター空き枠まだあるやで。-AbilityCardFullTankCheak");
        return false; // まだイベント空き枠あるお。

    }

    // デッキにイベント満タンか
    public void AbilityMANTAN()
    {

        // イベント満タンやったら
        if (AbilityCardFullTankCheak() == true)
        {
            // コレクションにある_全部暗くする
            for (int i3 = 0; i3 < SelectData.DECK_COLLECTCARD_MAX; i3++)
            {
                if (m_aCollectCard[i3].GetComponent<uGUICard>().cardData.cardType == CardType.AbilityFighter)
                {
                    bool bIDCheakOK = true;
                    for (int i2 = 0; i2 < PlayerDeckData.numStriker; i2++)
                    {
                        if (m_aMyDeckCard[i2].GetComponent<uGUICard>().cardData.id ==
                        m_aCollectCard[i3].GetComponent<uGUICard>().cardData.id)
                        {
                            bIDCheakOK = false;// デッキに同じIDあり
                        }

                    }

                    // IDがデッキになかったら
                    if (bIDCheakOK == true)
                    {
                        // 満杯マークや
                        m_aCollectCard[i3].GetComponent<uGUICard>().AbilityFullInfo_On();
                    }


                }

            }

        }// 満タンやったら

    }

    // なんかのボタン達に触れた時
    public void ClickAnyButton(int no)
    {
        // シーンチェンジを行っていたら反応させない
        //if (m_bSceneChange) return;

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

    public bool HandleMessge(MessageInfo message)
    {
        return m_pStateMachine.HandleMessage(message);
    }

    public void SeigenNoTextUpdate()
    {

        // 効果持ちカードの数値
        int iAbilityStrikerNom = 0;// PlayerDeckData.numStriker + PlayerDeckData.numJoker;
        for (int i = 0; i < PlayerDeckData.numStriker; i++)
        {
            // 空き枠を探す
            if (m_aMyDeckCard[i].GetComponent<uGUICard>().cardData.cardType == CardType.AbilityFighter)
            {
                iAbilityStrikerNom++;
            }

        }// 効果

        SeigenNoText.text = iAbilityStrikerNom.ToString();
        SeigenNoText.text += "/4";

        if (iAbilityStrikerNom >= 5)
        {
            SeigenNoText.color = new Color(1, 0, 0, 1);
        }
        else {
            SeigenNoText.color = new Color(1, 1, 1, 1);
        }

    }

}
