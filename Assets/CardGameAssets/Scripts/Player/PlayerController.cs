using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    Player myPlayer;                  // コントロールするプレイヤーの実体
    readonly Vector3 playerCameraPosition = new Vector3(0, 52.5f, -29.2f);

    readonly int fieldStrikerHoldNo = 114;
    readonly int fieldEventHoldNo = 514;
    int holdHandNo;                     // 手札の何番目のカードをつかんでいるか(ネット上では手札を同期させているのでメッセージで何番目の手札をつかったかの情報が欲しかった)
    readonly int noHoldCard = -1;       // 掴んでいないフラグ
    float setFieldBorderY;              // カード掴んでフィールドにセットできるライン
    Vector3 orgHoldCardPosition;
    Card holdCard;                      // 掴んでるカード

    bool cardSetOK;
    bool isFieldCardHold;

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

        var temp = myPlayer.cardObjectManager.handCardDir.y;
        myPlayer.cardObjectManager.handCardDir.y = myPlayer.cardObjectManager.handCardDir.z;
        myPlayer.cardObjectManager.handCardDir.z = temp;
    }


    // Use this for initialization
    void Start()
    {
        cardSetOK = false;
        myPlayer = GetComponent<Player>();

        holdHandNo = noHoldCard;

        setFieldBorderY = Screen.height / 4;

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
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        switch (myPlayer.state)
        {
            case PlayerState.FirstDraw:

                break;
            case PlayerState.SetStriker:
                CardTapUpdate();
                SetStrikerUpdate();
                break;
        }

        // カードタップ

	}

    void CardTapUpdate()
    {
        // 押した瞬間
        if(oulInput.GetTouchState() == oulInput.TouchState.Began)
        {
            var touchObject = oulInput.Collision3D();
            if (touchObject)
            {
                // レイに触れたオブジェクトがカードかどうかチェック
                if (touchObject.tag == "Card")
                {
                    var card = touchObject.GetComponent<Card>();

                    // インフォメーション表示
                    myPlayer.playerManager.uiManager.AppearBattleCardInfomation(card.cardData);
                }
                else
                {
                    // インフォメーション非表示
                    myPlayer.playerManager.uiManager.DisAppearBattleCardInfomation();
                }
            }
        }
    }

    protected virtual void SetStrikerUpdate()
    {
        //if (myPlayer.isSetStriker()) return;

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
                        holdCard.cashTransform.localPosition = orgHoldCardPosition;
                        // 描画順
                        holdCard.SetOrder(myPlayer.deckManager.GetNumHand());
                    }
                    // 手札から掴んでたら
                    else
                    {
                        holdCard.cashTransform.localPosition = orgHoldCardPosition;
                        // 描画順
                        holdCard.SetOrder(holdHandNo);
                    }
                    return;
                }

                // カード位置更新
                var newPosition = RaypickHand(currentPosition);
                holdCard.transform.localPosition = newPosition;

                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    // フィールドをセット
                    if(isFieldCardHold)
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
                        SendSetCard(holdHandNo);
                    }

                    else
                    {
                        SendSetCard(holdHandNo);
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
                    if(isFieldCardHold)
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
                    var hand = myPlayer.cardObjectManager.GetHandCardObject();
                    var fieldCard = myPlayer.cardObjectManager.fieldStrikerCard;
                    var card = tapObject.GetComponent<Card>();

                    if(fieldCard.gameObject.activeInHierarchy)
                    {
                        if(card.cardData.id == fieldCard.cardData.id)
                        {
                            holdCard = card;
                            holdCard.SetUraomote(true);
                            // ゴリ
                            // 手札の一番後ろ
                            myPlayer.cardObjectManager.MakeHandTransform(myPlayer.deckManager.GetNumHand(), holdCard);
                            orgHoldCardPosition = holdCard.cashTransform.localPosition;
                            // ここでレイピックでの座標更新
                            holdCard.cashTransform.localPosition = RaypickHand(oulInput.GetPosition(0, false));
                            isFieldCardHold = true;
                            holdHandNo = fieldStrikerHoldNo;
                            // 描画順をめっちゃ前にする
                            holdCard.SetOrder(114);
                            return;
                        }
                    }
                    for (int i = 0; i < hand.Length; i++)
                    {
                        if (card.cardData.id == hand[i].cardData.id)
                        {
                            holdCard = card;
                            isFieldCardHold = false;
                            holdHandNo = i;
                            // 掴んでる座標
                            orgHoldCardPosition = card.transform.localPosition;
                            break;
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
                    SendSetCard(holdHandNo);
                    // したのでメッセージおくる
                    MessageManager.Dispatch(myPlayer.playerID, MessageType.SetStrikerOK, null);

                    holdCard = null;
                    holdHandNo = noHoldCard;
                    return;
                }

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
                        SendSetCard(i);
                        break;
                    }
                }
            }
        }
        // したのでメッセージおくる
        MessageManager.Dispatch(myPlayer.playerID, MessageType.SetStrikerOK, null);
    }

    void SendSetCard(int handNo)
    {
        // ex構造体作成
        SetCardInfo exInfo = new SetCardInfo();
        exInfo.handNo = handNo;
        // メッセージ送信
        MessageManager.Dispatch(myPlayer.playerID, MessageType.SetCard, exInfo);
    }
}
