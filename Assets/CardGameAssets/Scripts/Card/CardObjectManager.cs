using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObjectManager : MonoBehaviour
{
    public Card cardPrefab;                 // カードの複製元

    const int maxHand = 9;      // 手札の最大数
    const int maxYamahuda = 15; // 山札の最大数
    //const int numCardObject = 10 + 1 + 4;  // 1 ~ 10 + ジョーカー + イベント
    Card[] handCards = new Card[maxHand];           // 手札のカード
    Card[] yamahudaCards = new Card[maxYamahuda];   // 山札のカード
    public Card fieldStrikerCard { get; private set; }
    public Card fieldEventCard { get; private set; }

    public Vector3 handShift;           // カード位置補正
    public Vector3 handCardDir;         // カードの向き(法線的な)

    readonly Vector3 yamahudaField = new Vector3(15, 0, -15);      // 山札の位置
    readonly Vector3 strikerField = new Vector3(0, 0, -10);     // ストライカーカードをセットする位置
    readonly Vector3 eventField = new Vector3(0, 0, -10);       // イベントカードをセットする位置

    // Use this for initialization
    void Awake()
    {
        // カードの実体を生成

        // 手札カード
        for (int i=0;i< handCards.Length;i++)
        {
            handCards[i] = Instantiate(cardPrefab, transform);
            handCards[i].name = "HandCard" + i;
            handCards[i].gameObject.SetActive(false);
        }

        // 山札カード
        for (int i=0;i< yamahudaCards.Length;i++)
        {
            yamahudaCards[i] = Instantiate(cardPrefab, transform);
            yamahudaCards[i].name = "Yamahuda" + i;
            yamahudaCards[i].gameObject.SetActive(false);
        }

        // フィールドカード
        fieldStrikerCard = Instantiate(cardPrefab, transform);
        fieldStrikerCard.name = "FieldStrikerCard";
        fieldStrikerCard.gameObject.SetActive(false);
        fieldEventCard = Instantiate(cardPrefab, transform);
        fieldEventCard.name = "FieldEventCard";
        fieldEventCard.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public Card[] GetHandCardObject() { return handCards; }

    public void UpdateHand(List<CardData> hand, Player player)
    {
        for(int i=0;i<maxHand;i++)
        {
            if (i < hand.Count)
            {
                // カード表示
                handCards[i].gameObject.SetActive(true);
                // カード情報設定
                handCards[i].SetCardData(hand[i], player.isMyPlayer);
            }
            else
            {
                // カード非表示
                handCards[i].gameObject.SetActive(false);
            }
        }

        // 位置更新
        UpdateHandPosition(player);
    }

    // 手札の位置更新
    void UpdateHandPosition(Player player)
    {
        for(int i = 0; i < maxHand; i++)
        {
            var card = handCards[i];

            if (!handCards[i].isActiveAndEnabled) break;

            // 表
            card.SetUraomote(true);
            // 描画順
            card.SetOrder(i);

            MakeHandTransform(i, player.deckManager.GetNumHand(), card);
        }
    }

    public void MakeHandTransform(int handNo, int numHand, Card card)
    {
        var position = Vector3.zero;
        var shift = handShift;
        var dir = handCardDir;
        var angle = Vector3.zero;
        angle.y = 180;

        const float width = 2;
        position.x = width * handNo - (numHand * width / 2) + (width / 2);
        position.z = -20.0f;

        // 逆サイド処理
        if (card.isMyPlayerSide == false)
        {
            card.SetUraomote(false);

            position.x = -position.x;
            position.y = 1;
            position.z = -position.z;
            shift.z = -shift.z;
            dir.z = -dir.z;
            angle.x = 180;

            card.cashTransform.localPosition = position;
            card.cashTransform.localEulerAngles = angle;

            return;
        }
        position += shift + (handNo * 0.01f * shift);
        // 角度補正
        angle.x = Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg;

        card.cashTransform.localPosition = position;
        card.cashTransform.localEulerAngles = angle;
    }

    public void UpdateYamahuda(Queue<CardData> yamahuda, Player player)
    {
        CardData[] array = yamahuda.ToArray();

        for (int i = 0; i < maxYamahuda; i++)
        {
            if (i < array.Length)
            {
                // カード表示
                yamahudaCards[i].gameObject.SetActive(true);
                // カード情報設定
                yamahudaCards[i].SetCardData(array[i], player.isMyPlayer);
            }
            else
            {
                // カード非表示
                yamahudaCards[i].gameObject.SetActive(false);
            }
        }

        UpdateYamahudaPosition();
    }

    void UpdateYamahudaPosition()
    {
        for (int i = 0; i < maxYamahuda; i++)
        {
            var card = yamahudaCards[i];
            if (!card.gameObject.activeInHierarchy) break;

            // 裏
            card.SetUraomote(false);
            // 描画順
            card.SetOrder(i);
            // 位置設定
            var position = yamahudaField;
            var angle = Vector3.zero;
            angle.y = 180;
            angle.z = 180;
            position.y = i * 0.25f;
            // 逆サイド処理
            if (card.isMyPlayerSide == false)
            {
                position.x = -position.x;
                position.z = -position.z;
                angle.y = 0;
            }

            card.handPosition = position;
            card.cashTransform.localPosition = position;
            card.cashTransform.localEulerAngles = angle;
        }
    }

    public void FieldSet(int handNo, bool isMyPlayer)
    {
        var type = handCards[handNo].cardData.cardType;
        var card = handCards[handNo];

        // 最下層にする
        card.SetOrder(0);

        var position = Vector3.zero;
        var angle = card.cashTransform.localEulerAngles;
        angle.x = 0;
        angle.z = 180;

        switch (type)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
            case CardType.Joker:
                {
                    // フィールドにおいてるストライカーのカード
                    //fieldStrikerCard = card;
                    fieldStrikerCard.gameObject.SetActive(true);
                    fieldStrikerCard.SetCardData(card.cardData, isMyPlayer);
                    // カードを指定の位置にセット
                    position = strikerField;
                    // 逆サイド処理
                    if (isMyPlayer == false)
                    {
                        position.x = -position.x;
                        position.z = -position.z;
                    }
                    //fieldStrikerCard.cashTransform.localPosition = position;
                    fieldStrikerCard.nextPosition = position;
                    fieldStrikerCard.cashTransform.localEulerAngles = angle;

                    // セットのステートにさせる
                    fieldStrikerCard.stateMachine.ChangeState(CardObjectState.Set.GetInstance());
                }
                break;
        
            //case CardType.Event:
            //    // フィールドにおいてるストライカーのカード
            //    fieldEventCard = card;
            //    // カードを指定の位置にセット
            //    position = eventField;
            //    // 逆サイド処理
            //    if (isMyPlayer == false)
            //    {
            //        position.x = -position.x;
            //        position.z = -position.z;
            //        angle.z = 180;
            //    }
            //    fieldEventCard.cashTransform.localPosition = position;
            //    fieldEventCard.cashTransform.localPosition = angle;

            //    // 裏にする
            //    fieldEventCard.SetUraomote(false);
            //    break;
        }
    }

    public void BackToHand(List<CardData> hand, Player player)
    {
        // カード非表示
        fieldStrikerCard.gameObject.SetActive(false);
        UpdateHand(hand, player);
    }

    public void ReFieldSetStriker(bool isMyPlayer)
    {
        if(fieldStrikerCard.gameObject.activeInHierarchy)
        {
            var position = Vector3.zero;
            var angle = fieldStrikerCard.cashTransform.localEulerAngles;
            angle.x = 0;
            angle.z = 180;

            // カードを指定の位置にセット
            position = strikerField;
            // 逆サイド処理
            if (isMyPlayer == false)
            {
                position.x = -position.x;
                position.z = -position.z;
            }
            fieldStrikerCard.nextPosition = position;
            fieldStrikerCard.cashTransform.localEulerAngles = angle;

            // セットのステートにさせる
            fieldStrikerCard.stateMachine.ChangeState(CardObjectState.Set.GetInstance());
        }
    }

    public void TurnEnd()
    {
        fieldStrikerCard.gameObject.SetActive(false);
        fieldEventCard.gameObject.SetActive(false);
    }

    public bool isSetEndStriker()
    {
        if(fieldStrikerCard != null)
        {
            return fieldStrikerCard.isSetField;
        }
        return false;
    }

    public void Restart()
    {
        // フィールドカード
        fieldStrikerCard.gameObject.SetActive(false);
        fieldEventCard.gameObject.SetActive(false);
    }
}
