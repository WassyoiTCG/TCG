using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    Player myPlayer;                  // コントロールするプレイヤーの実体
    readonly Vector3 playerCameraPosition = new Vector3(0, 52.5f, -29.2f);

    int holdHandNo;                     // 手札の何番目のカードをつかんでいるか(ネット上では手札を同期させているのでメッセージで何番目の手札をつかったかの情報が欲しかった)
    readonly int noHoldCard = -1;       // 掴んでいないフラグ
    readonly float moveWidthSetOK = 10; // シャドバのカードセット可能Y移動距離
    Vector3 cardHoldPosition;           // カード掴んで移動用

    Vector3 orgHoldCardPosition;

    bool cardSetOK;

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
    }


    // Use this for initialization
    void Start()
    {
        cardSetOK = false;
        myPlayer = GetComponent<Player>();

        holdHandNo = noHoldCard;

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
                SetStrikerUpdate();
                break;
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
                var holdCard = myPlayer.cardObjectManager.GetHandCardObject()[holdHandNo];

                // 手札に戻る判定
                if (currentPosition.y - cardHoldPosition.y <= moveWidthSetOK)
                {
                    cardSetOK = false;
                    // カードの座標を手札に戻す
                    holdCard.cashTransform.localPosition = orgHoldCardPosition;
                    return;
                }

                // カード位置更新
                var newPosition = oulMath.ScreenToWorldPlate(currentPosition, Vector3.up, myPlayer.cardObjectManager.handShift.magnitude);
                holdCard.transform.localPosition = newPosition;

                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    // すでにフィールドにセットされているなら
                    if(myPlayer.deckManager.isSetStriker())
                    {

                    }

                    // ex構造体作成
                    SetCardInfo exInfo = new SetCardInfo();
                    exInfo.handNo = holdHandNo;
                    // メッセージ送信
                    MessageManager.Dispatch(myPlayer.playerID, MessageType.SetCard, exInfo);
                    // 掴んでるカードを離す
                    holdCard.isHold = false;
                    holdCard = null;
                    holdHandNo = noHoldCard;
                }
            }
            else
            {
                // マウスクリック放したら
                if (oulInput.GetTouchState() == oulInput.TouchState.Ended)
                {
                    holdHandNo = noHoldCard;
                    // ここでデッキデータを更新しよう(フィールド→手札考慮)
                    return;
                }
                // 移動量を計算
                if (currentPosition.y - cardHoldPosition.y > moveWidthSetOK)
                {
                    cardSetOK = true;
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

                    if(fieldCard)
                    {
                        if(card.cardData.id == fieldCard.cardData.id)
                        {

                        }
                    }
                    for (int i = 0; i < hand.Length; i++)
                    {
                        if (card.cardData.id == hand[i].cardData.id)
                        {
                            holdHandNo = i;
                            card.isHold = true;

                            // 掴んでる座標
                            cardHoldPosition = oulInput.GetPosition(0, false);

                            orgHoldCardPosition = card.transform.localPosition;
                            break;
                        }
                    }
                }

      
            }
        }
    }
}
