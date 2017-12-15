using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    Player myPlayer;                  // コントロールするプレイヤーの実体
    readonly Vector3 playerCameraPosition = new Vector3(0, 52.5f, -29.2f);

    readonly int fieldStrikerHoldNo = 114;
    //readonly int fieldEventHoldNo = 514;
    public int holdHandNo;                     // 手札の何番目のカードをつかんでいるか(ネット上では手札を同期させているのでメッセージで何番目の手札をつかったかの情報が欲しかった)
    readonly int noHoldCard = -1;       // 掴んでいないフラグ
    float setFieldBorderY;              // カード掴んでフィールドにセットできるライン
    Vector3 orgHoldCardPosition;
    public Card holdCard;                      // 掴んでるカード

    bool cardSetOK;
    public bool isFieldCardHold;

    public bool isCardHold = false;  // 真のカード掴んで動かしているフラグ

    // ステートポインタ
    //FirstDrawState firstDrawState;
    //BasePlayerState statePointer;       // ステートポインタ

    // ★★★ローカルプレイヤー(自分で動かすプレイヤーのみ入る関数)
    public override void OnStartLocalPlayer()
    {
        this.enabled = true;

        myPlayer = GetComponent<Player>();
        myPlayer.isMyPlayer = true;

        var uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        // マッチング待機中表示
        uiManager.AppearMatchingWait();

        //var temp = myPlayer.cardObjectManager.handCardDir.y;
        //myPlayer.cardObjectManager.handCardDir.y = myPlayer.cardObjectManager.handCardDir.z;
        //myPlayer.cardObjectManager.handCardDir.z = temp;
    }


    // Use this for initialization
    void Start()
    {
        Restart();

        isCardHold = false;

        cardSetOK = false;
        myPlayer = GetComponent<Player>();

        setFieldBorderY = Screen.height / 6;

        //battleCardInfomation = GameObject.Find("BattleCardInfomation").GetComponent<BattleCardInfomation>();

        // カメラ位置
        var cameraTransform = Camera.main.transform;
        var cameraPosition = playerCameraPosition;
        var cameraAngle = cameraTransform.localEulerAngles;

        // PlayerID→isMyPlayer
        if (myPlayer.isMyPlayer)
        {
            cameraAngle.y = 0;
        }
        else
        {
            cameraPosition.z = -cameraPosition.z;
            cameraAngle.y = 180;
        }
    }

    public void Restart()
    {
        if (!enabled) return;

        cardSetOK = false;
        holdHandNo = noHoldCard;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // ストライカーセットするステートにいたら
        if (myPlayer.stateMachine.isInState(PlayerState.SetStriker.GetInstance()))
        {
            // もういや
            SetStrikerUpdate();
            CardTapUpdate();
        }
        // インターセプトセットするステートにいたら
        if(myPlayer.stateMachine.isInState(PlayerState.SetIntercept.GetInstance()))
        {
            CardTapUpdate();
            SetInterceptUpdate();
        }
        if (myPlayer.stateMachine.isInState(PlayerState.SupportWait.GetInstance()))
        {
            // なんかカード持ってたら
            if (holdHandNo == noHoldCard) return;

            // ★★★カードの座標を強制的に手札に戻す
            // フィールドから掴んでたら
            if (isFieldCardHold)
            {
                // フィールドにもう一度セット
                myPlayer.cardObjectManager.ReFieldSetStriker(true);
            }
            // 手札から掴んでたら
            else
            {
                // 手札の座標に戻る
                holdCard.cacheTransform.localPosition = orgHoldCardPosition;
                // 描画順
                holdCard.SetOrder(holdHandNo);
            }

            // 掴んでるカードIDを離す
            holdHandNo = noHoldCard;
        }
	}

    void CardTapUpdate()
    {
        //List<RaycastResult> raycastResults = new List<RaycastResult>();
        //PointerEventData eventData = new PointerEventData(EventSystem.current);
        //EventSystem.current.RaycastAll(eventData, raycastResults);

        // 押した瞬間
        if (oulInput.GetTouchState() == oulInput.TouchState.Began)
        {
            // UIには触れていない
            var touchObject = oulInput.Collision3D();
            if (touchObject)
            {
                // レイに触れたオブジェクトがカードかどうかチェック
                if (touchObject.tag == "Card")
                {
                    var card = touchObject.GetComponent<Card>();
                    if (!card.uraomoteFlag) return;

                    // インフォメーション表示
                    myPlayer.playerManager.uiManager.AppearBattleCardInfomation(card.cardData);
                }
                else
                {
                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                }

                //+-------------------------------------------------------------------
                //      全てのダメージポイント確認
                //+-------------------------------------------------------------------
                if (touchObject.tag == "DamagePoint")
                {
                    myPlayer.playerManager.uiManager.remainingPoints.TouchBegan();
                }

                //+-------------------------------------------------------------------
                //      相手の効果モンスター見る
                //+-------------------------------------------------------------------
                if (touchObject.tag == "CpuWatchCard")
                {
                    var PlayerMgr = myPlayer.playerManager;
                    var cpuPlayer = PlayerMgr.GetCPUPlayer();
                    PlayerMgr.uiManager.cpuWatchCard.Appear(cpuPlayer.deckManager.GetHaveAbilityCards());

                }
                else
                {
                    myPlayer.playerManager.uiManager.cpuWatchCard.DisAppear();
                }

                var ui = EventSystem.current.currentSelectedGameObject;

                // マウスがUIにポイントしていたら
                if (ui != null /*raycastResults.Count > 0*/)
                {
                    return;
                }

                // 墓地オブジェクトかどうか
                if (touchObject.tag == "Cemetery")
                {
                    if (touchObject.name == "myCemetery")
                        myPlayer.playerManager.uiManager.cemeteryInfoUIManager.Appear(myPlayer.deckManager.GetCemeteryCards(), myPlayer.deckManager.GetExpulsionCards());
                    else if (touchObject.name == "cpuCemetery")
                    {
                        var cpuPlayer = myPlayer.playerManager.GetCPUPlayer();
                        myPlayer.playerManager.uiManager.cemeteryInfoUIManager.Appear(cpuPlayer.deckManager.GetCemeteryCards(), cpuPlayer.deckManager.GetExpulsionCards());
                    }
                    else Debug.LogError("墓地の名前が違う");
                }
                else
                {
                    myPlayer.playerManager.uiManager.cemeteryInfoUIManager.DisAppear();
                }
            }
            else
            {
                myPlayer.playerManager.uiManager.cemeteryInfoUIManager.DisAppear();
                myPlayer.playerManager.uiManager.cpuWatchCard.DisAppear();
            }
        }
        // 離した瞬間
        if(oulInput.GetTouchState() == oulInput.TouchState.Ended)
        {
            // 閉じる
            if (myPlayer.playerManager.uiManager.remainingPoints.gameObject.activeSelf)
                myPlayer.playerManager.uiManager.remainingPoints.TouchEnded();
        }
    }

    //+-------------------------------------------------------------
    //  ストライカー
    //+-------------------------------------------------------------
    void SetStrikerUpdate()
    {
        if (myPlayer.isPushedJunbiKanryo)
        {
            // 1秒待ってまだ相手が設置してないのでUIをだす
            if((myPlayer.waitTimer += Time.deltaTime) > 0.5f)
            {
                myPlayer.playerManager.uiManager.AppearWaitYouUI();
            }
            return;
        }

        // カード掴んでたら
        if (holdHandNo != noHoldCard)
        {
            var currentPosition = oulInput.GetPosition(0, false);

            if (cardSetOK)
            {
                // 手札に戻る判定
                if (currentPosition.y <= setFieldBorderY)
                {
                    cardSetOK = false;
                    

                    // カードの座標を手札に戻す
                    // フィールドから掴んでたら
                    if (isFieldCardHold)
                    {
                        holdCard.cacheTransform.localPosition = orgHoldCardPosition;
                        // 描画順
                        holdCard.SetOrder(myPlayer.deckManager.GetNumHand());
                        // UVエフェクト発動
                        //holdCard.ActiveUseCard();
                    }
                    // 手札から掴んでたら
                    else
                    {
                        holdCard.cacheTransform.localPosition = orgHoldCardPosition;
                        // 描画順
                        holdCard.SetOrder(holdHandNo);
                    }
                    return;
                }
               
                // ここが真にカードを掴んでいる所さん。
                isCardHold = true;

                // カード位置更新
                var newPosition = RaypickHand(currentPosition);
                holdCard.cacheTransform.localPosition = newPosition;

                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    // SE
                    oulAudio.PlaySE("card_draw0");

                    // ★サポートカード処理はメッセージ受信時に行う
                    if (holdCard.cardData.cardType == CardType.Support)
                    {
                        SendSetCard(holdHandNo, MessageType.SetSupport);
                    }

                    // フィールドをセット
                    else if(isFieldCardHold)
                    {
                        myPlayer.cardObjectManager.ReFieldSetStriker(true);
                    }

                    // すでにフィールドにセットされているなら
                    else if(myPlayer.deckManager.isSetStriker())
                    {
                        // ex構造体作成
                        BackToHandInfo exInfo = new BackToHandInfo();
                        exInfo.iCardType = (int)myPlayer.deckManager.fieldStrikerCard.cardType;
                        // メッセージ送信
                        MessageManager.Dispatch(myPlayer.playerID, MessageType.BackToHand, exInfo);

                        // カードセット
                        SendSetCard(holdHandNo, MessageType.SetStriker);
                    }

                    else
                    {
                        SendSetCard(holdHandNo, MessageType.SetStriker);
                    }

                    // 掴んでるカードを離す
                    holdHandNo = noHoldCard;

                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                    
                }
            }
            // 手札から動かないモード
            else
            {
                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {

                    // ここでデッキデータを更新しよう(フィールド→手札考慮)
                    if (isFieldCardHold)
                    {
                        // ex構造体作成
                        BackToHandInfo exInfo = new BackToHandInfo();
                        exInfo.iCardType = (int)holdCard.cardData.cardType;
                        // メッセージ送信
                        MessageManager.Dispatch(myPlayer.playerID, MessageType.BackToHand, exInfo);
                    }
                    // 掴んでるカードを離す
                    holdHandNo = noHoldCard;

                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                    return;
                }
                // 選択不可のカードなら情報は出すけどセットはできない
                if (holdCard.notSelectFlag) return;
                // 移動量を計算
                if (currentPosition.y > setFieldBorderY)
                {
                    cardSetOK = true;
                    // 一番前
                    holdCard.SetOrder(114);

                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                    
                }
            }


        }
        else
        {

            isCardHold = false;
            //// マウスがUIにポイントしていたら
            //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            //{
            //    return;
            //}

            // マウスクリックした瞬間
            if (oulInput.GetTouchState() == oulInput.TouchState.Began)
            {
                // 手札のカードとあたり判定を取る
                //var hand = myPlayer.cardObjectManager.GetHandCardObject();
                //for (int i = 0; i < hand.Length; i++)
                //{
                //    var card = hand[i];

                //    // 空だったら終了
                //    if (!card.gameObject.activeInHierarchy) break;

                //    var camera = Camera.main;

                //    // レイ情報作成(マウス位置からレイを飛ばす)
                //    Ray ray = camera.ScreenPointToRay(oulInput.GetPosition(0, false));

                //    // レイ(マウス)とBox(カード)で判定を取る
                //    var cardBoxCollider = card.GetComponent<BoxCollider>();
                //    var intersectionPoint = Vector3.zero;
                //    if(oulMath.LineVsBox(cardBoxCollider, camera.transform.localPosition, ray.direction, out intersectionPoint))
                //    {
                //        holdHandNo = i;
                //        card.isHold = true;
                //        // 掴んでる座標
                //        cardHoldPosition = oulInput.GetPosition(0, false);
                //    }
                //}

                var tapObject = oulInput.Collision3D();
                if (!tapObject) return;

                // レイに触れたオブジェクトがカードかどうかチェック
                if (tapObject.tag == "Card")
                {
                    var card = tapObject.GetComponent<Card>();
                    // 相手サイドは見れない
                    if (!card.isMyPlayerSide) return;

                    var hand = myPlayer.cardObjectManager.GetHandCardObject();
                    var fieldCard = myPlayer.cardObjectManager.fieldStrikerCard;

                    if (fieldCard != null/*.gameObject.activeInHierarchy*/)
                    {
                        if(card.cardData.id == fieldCard.cardData.id)//←[1210]なんかエラー出てた。 
                        {

                            // 戻す
                            //myPlayer.cardObjectManager.ActiveUseCard();

                            //+-------------------------------------------------------
                            // SE
                            oulAudio.PlaySE("card_hold");

                            holdCard = card;
                            holdCard.SetUraomote(true);
                            // ゴリ
                            // 手札の一番後ろ
                            var numHand = myPlayer.deckManager.GetNumHand();
                            myPlayer.cardObjectManager.MakeHandTransform(numHand, numHand + 1, holdCard);
                            orgHoldCardPosition = holdCard.cacheTransform.localPosition;
                            // ここでレイピックでの座標更新
                            holdCard.cacheTransform.localPosition = RaypickHand(oulInput.GetPosition(0, false));
                            //holdCard.cacheTransform.localPosition = orgHoldCardPosition;
                            isFieldCardHold = true;
                            holdHandNo = fieldStrikerHoldNo;
                            // 描画順をめっちゃ前にする
                            holdCard.SetOrder(114);

                            // めり込みバグ修正
                            holdCard.ChangeState(CardObjectState.None.GetInstance());
                            
                            return;
                        }
                    }

                    // 裏になってるカードは見れない
                    if (!card.uraomoteFlag) return;

                    for (int i = 0; i < hand.Length; i++)
                    {
                        if (card.cardData.id == hand[i].cardData.id)
                        {
                            
                            // カード押した
                            // SE
                            oulAudio.PlaySE("card_hold");

                            holdCard = card;
                            isFieldCardHold = false;
                            holdHandNo = i;
                            // 掴んでる座標
                            orgHoldCardPosition = card.transform.localPosition;
                            return;
                        }
                    }
                }

      
            }
        }
    }

    //+-------------------------------------------------------------
    //  インターセプト
    //+-------------------------------------------------------------
    void SetInterceptUpdate()
    {
        if (myPlayer.isPushedJunbiKanryo) return;

        // マウスクリック放したら
        if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
        {
            // インフォメーション非表示
            myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
        }

        if (holdHandNo != noHoldCard)
        {
            var currentPosition = oulInput.GetPosition(0, false);

            if (cardSetOK)
            {
                // 手札に戻る判定
                if (currentPosition.y <= setFieldBorderY)
                {
            
                    cardSetOK = false;

                    // カードの座標を手札に戻す
                    holdCard.cacheTransform.localPosition = orgHoldCardPosition;
                    // 描画順
                    holdCard.SetOrder(holdHandNo);
                    return;
                }

                // こ↑こ↓でカード動かしている
                isCardHold = true;

                // カード位置更新
                var newPosition = RaypickHand(currentPosition);
                holdCard.cacheTransform.localPosition = newPosition;

                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    // ex構造体作成
                    SelectCardIndexInfo exInfo = new SelectCardIndexInfo();
                    exInfo.index = holdHandNo;
                    // メッセージ送信
                    MessageManager.Dispatch(myPlayer.playerID, MessageType.SetIntercept, exInfo);

                    // 掴んでるカードを離す
                    holdHandNo = noHoldCard;
                }
            }
            // 手札から動かないモード
            else
            {
                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    // 掴んでるカードを離す
                    holdHandNo = noHoldCard;

                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                    return;
                }
                // 選択不可のカードなら情報は出すけどセットはできない
                if (holdCard.notSelectFlag) return;
                // 移動量を計算
                if (currentPosition.y > setFieldBorderY)
                {
                    cardSetOK = true;
                    // 一番前
                    holdCard.SetOrder(114);

                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                    
                }
            }
        }
        else
        {

            isCardHold = false;

            // マウスがUIにポイントしていたら
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // マウスクリックした瞬間
            if (oulInput.GetTouchState() == oulInput.TouchState.Began)
            {
                var tapObject = oulInput.Collision3D();
                if (!tapObject) return;

                // レイに触れたオブジェクトがカードかどうかチェック
                if (tapObject.tag == "Card")
                {
                    var card = tapObject.GetComponent<Card>();

                    // 相手サイドは見れない
                    if (!card.isMyPlayerSide) return;

                    var hand = myPlayer.cardObjectManager.GetHandCardObject();
                    var fieldCard = myPlayer.cardObjectManager.fieldEventCard;

                    if (fieldCard != null/*.gameObject.activeInHierarchy*/)
                    {
                        return;
                    }

                    // 裏になってるカードは見れない
                    if (!card.uraomoteFlag) return;

                    for (int i = 0; i < hand.Length; i++)
                    {
                        if (card.cardData.id == hand[i].cardData.id)
                        {
                            holdCard = card;
                            isFieldCardHold = false;
                            holdHandNo = i;
                            // 掴んでる座標
                            orgHoldCardPosition = card.transform.localPosition;
                            return;
                        }
                    }
                }
            }
        }
    }

    public Vector3 RaypickHand(Vector3 screenPosition)
    {
        return oulMath.ScreenToWorldPlate(screenPosition, Vector3.up, myPlayer.cardObjectManager.handShift.magnitude + 2);
    }

    public void SetStrikerTimeOver()
    {
        // フィールドにセットしてないか確認
        if (!myPlayer.deckManager.isSetStriker())
        {
            // 掴んでるカードを離す
            if (holdHandNo != noHoldCard)
            {
                if (holdCard.cardData.isStrikerCard())
                {
                    SendSetCard(holdHandNo, MessageType.SetStriker);
                    // したのでメッセージおくる
                    MessageManager.Dispatch(myPlayer.playerID, MessageType.SetStrikerOK, null);

                    holdCard = null;
                    holdHandNo = noHoldCard;
                    return;
                }

                // (TODO)イベントカード、フィールドから掴んだカードの場合

                holdCard = null;
                holdHandNo = noHoldCard;
            }

            // ストライカーカードを持っているなら
            if (myPlayer.isHaveStrikerCard())
            {
                // 手札のランダムのストライカーを出す
                for (int i = 0; i < myPlayer.deckManager.GetNumHand(); i++)
                {
                    if (myPlayer.deckManager.GetHandCard(i).isStrikerCard())
                    {
                        SendSetCard(i, MessageType.SetStriker);
                        break;
                    }
                }
            }
        }
        // したのでメッセージおくる
        MessageManager.Dispatch(myPlayer.playerID, MessageType.SetStrikerOK, null);
    }

    void SendSetCard(int handNo, MessageType messageType)
    {
        // ex構造体作成
        SelectCardIndexInfo exInfo = new SelectCardIndexInfo();
        exInfo.index = handNo;
        // メッセージ送信
        MessageManager.Dispatch(myPlayer.playerID, messageType, exInfo);
    }
}
