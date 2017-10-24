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
    void Start ()
    {
        // カードの実体を生成

        // 手札カード
        for (int i=0;i< handCards.Length;i++)
        {
            handCards[i] = Instantiate(cardPrefab, transform);
        }

        // 山札カード
        for (int i=0;i< yamahudaCards.Length;i++)
        {
            yamahudaCards[i] = Instantiate(cardPrefab, transform);
        }
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
                handCards[i].SetCardData(hand[i]);
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
            card.SetOrder(i + 1);
            // 位置設定
            var position = Vector3.zero;
            var shift = handShift;
            var dir = handCardDir;
            var angle = card.cashTransform.localEulerAngles;
            angle.y = 180;
            position.x = -5 + ((float)i / maxHand) * 20;
            position.z = -20.0f;
            // 逆サイド処理
            if (player.isMyPlayer == false)
            {
                position.x = -position.x;
                position.z = -position.z;
                //angle.y = 0;
                shift.z = -shift.z;
                dir.z = -dir.z;
            }

            // 位置補正
            //position.x += dir.x * (i * 1.0f);
            //position.y += dir.z * (i * 1.0f);
            position += shift;
            // 角度補正
            angle.x = Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg;

            card.cashTransform.localPosition = position;
            card.cashTransform.localEulerAngles = angle;
        }
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
                yamahudaCards[i].SetCardData(array[i]);
            }
            else
            {
                // カード非表示
                yamahudaCards[i].gameObject.SetActive(false);
            }
        }

        UpdateYamahudaPosition(player);
    }

    void UpdateYamahudaPosition(Player player)
    {
        for (int i = 0; i < maxYamahuda; i++)
        {
            var card = yamahudaCards[i];
            if (!card.isActiveAndEnabled) break;

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
            if (player.isMyPlayer == false)
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

    public void FieldSet(int handNo, Player player)
    {
        var type = handCards[handNo].cardData.cardType;
        var card = handCards[handNo];

        // 最下層にする
        card.SetOrder(0);

        // セットフラグ
        card.isSet = true;

        var position = Vector3.zero;
        var angle = card.cashTransform.localEulerAngles;
        angle.x = 0;

        switch (type)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
            case CardType.Joker:
                {
                    // フィールドにおいてるストライカーのカード
                    fieldStrikerCard = card;
                    // カードを指定の位置にセット
                    position = strikerField;
                    // 逆サイド処理
                    if (player.isMyPlayer == false)
                    {
                        position.x = -position.x;
                        position.z = -position.z;
                        angle.z = 180;
                    }
                    fieldStrikerCard.cashTransform.localPosition = position;
                    fieldStrikerCard.cashTransform.localEulerAngles = angle;
                }
                break;
        
            case CardType.Event:
                // フィールドにおいてるストライカーのカード
                fieldEventCard = card;
                // カードを指定の位置にセット
                position = eventField;
                // 逆サイド処理
                if (player.isMyPlayer == false)
                {
                    position.x = -position.x;
                    position.z = -position.z;
                    angle.z = 180;
                }
                fieldEventCard.cashTransform.localPosition = position;
                fieldEventCard.cashTransform.localPosition = angle;
                break;
        }


    }
}
