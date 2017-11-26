using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObjectManager : MonoBehaviour
{
    public Card cardPrefab;                 // カードの複製元

    const int maxHand = 9;      // 手札の最大数
    const int maxYamahuda = 15; // 山札の最大数
    //const int numCardObject = 10 + 1 + 4;  // 1 ~ 10 + ジョーカー + イベント
    List<Card> handCards = new List<Card>();        // 手札のカード
    List<Card> yamahudaCards = new List<Card>();    // 山札のカード
    List<Card> bochiCards = new List<Card>();       // 墓地のカード
    public Card fieldStrikerCard { get; private set; }
    public Card fieldEventCard { get; private set; }

    public Vector3 handShift;           // カード位置補正
    public Vector3 handCardDir;         // カードの向き(法線的な)

    readonly Vector3 yamahudaField = new Vector3(15, 0, -15);      // 山札の位置
    readonly Vector3 strikerField = new Vector3(0, 0, -10);     // ストライカーカードをセットする位置
    readonly Vector3 eventField = new Vector3(0, 0, -10);       // イベントカードをセットする位置

    // Use this for initialization
    //void Awake()
    //{
    //    // カードの実体を生成

    //    // 手札カード
    //    handCards.Clear();
    //    //for (int i=0;i< handCards.Length;i++)
    //    //{
    //    //    handCards[i] = Instantiate(cardPrefab, transform);
    //    //    handCards[i].name = "HandCard" + i;
    //    //    handCards[i].gameObject.SetActive(false);
    //    //}

    //    // 山札カード
    //    yamahudaCards.Clear();
    //    //for (int i=0;i< yamahudaCards.Length;i++)
    //    //{
    //    //    yamahudaCards[i] = Instantiate(cardPrefab, transform);
    //    //    yamahudaCards[i].name = "Card" + i;
    //    //    yamahudaCards[i].gameObject.SetActive(false);
    //    //}

    //    // フィールドカード
    //    //fieldStrikerCard = Instantiate(cardPrefab, transform);
    //    //fieldStrikerCard.name = "FieldStrikerCard";
    //    //fieldStrikerCard.gameObject.SetActive(false);
    //    //fieldEventCard = Instantiate(cardPrefab, transform);
    //    //fieldEventCard.name = "FieldEventCard";
    //    //fieldEventCard.gameObject.SetActive(false);
    //}


    public void Restart()
    {
        // フィールドカード
        //fieldStrikerCard.gameObject.SetActive(false);
        //fieldEventCard.gameObject.SetActive(false);

        // 手札を空にする
        while(handCards.Count > 0)
        {
            var card = handCards[0];
            handCards.Remove(card);
            yamahudaCards.Add(card);
        }

        // 墓地を空にする
        while (bochiCards.Count > 0)
        {
            var card = bochiCards[0];
            bochiCards.Remove(card);
            yamahudaCards.Add(card);
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public Card[] GetHandCardObject() { return handCards.ToArray(); }

    public void Draw(CardData[] cards)
    {
        var orgNumHand = handCards.Count;

        foreach(CardData card in cards)
        {
            Card draw;
            if(yamahudaCards.Count > 0)
            {
                draw = yamahudaCards[yamahudaCards.Count - 1];
                yamahudaCards.Remove(draw);
            }
            else draw = Instantiate(cardPrefab, transform);

            // カード情報設定
            draw.SetCardData(card);

            // 手札リストに突っ込む
            handCards.Add(draw);

            var index = handCards.Count - 1;

            // デッキにいる位置を保存
            var position = draw.cacheTransform.localPosition;
            var angle = draw.cacheTransform.localEulerAngles;

            // 手札の位置
            MakeHandTransform(index, orgNumHand + cards.Length, draw);
            draw.SetOrder(index);

            // カードの動きをドローモードにする
            draw.Draw(position, angle);
        }

    }

    //public void UpdateHand(List<CardData> hand, Player player)
    //{
    //    //for(int i=0;i<maxHand;i++)
    //    //{
    //    //    if (i < hand.Count)
    //    //    {
    //    //        // カード表示
    //    //        handCards[i].gameObject.SetActive(true);
    //    //        // カード情報設定
    //    //        handCards[i].SetCardData(hand[i], player.isMyPlayer);
    //    //    }
    //    //    else
    //    //    {
    //    //        // カード非表示
    //    //        handCards[i].gameObject.SetActive(false);
    //    //    }
    //    //}

    //    // 位置更新
    //    UpdateHandPosition(player);
    //}

    // 手札の位置更新
    public void UpdateHandPosition(DeckManager deckManager)
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            var card = handCards[i];

            if (!handCards[i].isActiveAndEnabled) break;

            // 表
            card.SetUraomote(true);
            // 描画順
            card.SetOrder(i);

            MakeHandTransform(i, deckManager.GetNumHand(), card);
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

            card.cacheTransform.localPosition = position;
            card.cacheTransform.localEulerAngles = angle;

            return;
        }
        position += shift + (handNo * 0.01f * shift);
        // 角度補正
        angle.x = Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg;

        card.cacheTransform.localPosition = position;
        card.cacheTransform.localEulerAngles = angle;
    }

    public void MakeYamahudaTransform(int yamahudaNo, Card card)
    {
        // 裏
        card.SetUraomote(false);
        // 描画順
        card.SetOrder(yamahudaNo);

        // 位置設定
        var position = yamahudaField;
        var angle = Vector3.zero;
        angle.y = 180;
        angle.z = 180;
        position.y = yamahudaNo * 0.25f;
        // 逆サイド処理
        if (card.isMyPlayerSide == false)
        {
            position.x = -position.x;
            position.z = -position.z;
            angle.y = 0;
        }

        //card.handPosition = position;
        card.cacheTransform.localPosition = position;
        card.cacheTransform.localEulerAngles = angle;
    }

    //public void UpdateYamahuda()
    //{
    //    CardData[] array = yamahuda.ToArray();

    //    for (int i = 0; i < maxYamahuda; i++)
    //    {
    //        if (i < array.Length)
    //        {
    //            // カード表示
    //            yamahudaCards[i].gameObject.SetActive(true);
    //            // カード情報設定
    //            yamahudaCards[i].SetCardData(array[i]);
    //        }
    //        else
    //        {
    //            // カード非表示
    //            yamahudaCards[i].gameObject.SetActive(false);
    //        }
    //    }

    //    UpdateYamahudaPosition();
    //}

    void UpdateYamahudaPosition()
    {
        for (int i = 0; i < yamahudaCards.Count; i++)
        {
            var card = yamahudaCards[i];
            if (!card.gameObject.activeInHierarchy) break;

            // 位置情報設定
            MakeYamahudaTransform(i, card);
        }
    }

    public void FieldSet(int handNo, bool isMyPlayer)
    {
        var type = handCards[handNo].cardData.cardType;
        var card = handCards[handNo];

        // 最下層にする
        card.SetOrder(0);

        var position = Vector3.zero;
        var angle = card.cacheTransform.localEulerAngles;
        angle.x = 0;
        angle.z = 180;

        switch (type)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
            case CardType.Joker:
                {
                    // フィールドにおいてるストライカーのカード
                    fieldStrikerCard = card;
                    //fieldStrikerCard.gameObject.SetActive(true);
                    //fieldStrikerCard.SetCardData(card.cardData, isMyPlayer);
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
                    fieldStrikerCard.cacheTransform.localEulerAngles = angle;

                    // セットのステートにさせる
                    fieldStrikerCard.ChangeState(CardObjectState.Set.GetInstance());
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

        // ★★★手札から消す
        handCards.Remove(card);
    }

    public void BackToHand(/*List<CardData> hand,*/ DeckManager deckManager)
    {
        // カード非表示
        if (fieldStrikerCard)
        {
            // ステートを切る(Setステート中にやるとぬけてたから悲惨になった)
            fieldStrikerCard.ChangeState(CardObjectState.None.GetInstance());

            handCards.Add(fieldStrikerCard);
            fieldStrikerCard = null;
        }
        //fieldStrikerCard.gameObject.SetActive(false);
        // 手札に戻す
        UpdateHandPosition(deckManager);
        //UpdateHand(hand, player);
    }

    public void ReFieldSetStriker(bool isMyPlayer)
    {
        if(fieldStrikerCard/*.gameObject.activeInHierarchy*/)
        {
            var position = Vector3.zero;
            var angle = fieldStrikerCard.cacheTransform.localEulerAngles;
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
            fieldStrikerCard.cacheTransform.localEulerAngles = angle;

            // セットのステートにさせる
            fieldStrikerCard.ChangeState(CardObjectState.Set.GetInstance());
        }
    }

    public void TurnEnd()
    {
        // フィールドに存在するカードを墓地に送る
        if(fieldStrikerCard)
        {
            fieldStrikerCard.gameObject.SetActive(false);
            bochiCards.Add(fieldStrikerCard);
            fieldStrikerCard = null;
        }
        if(fieldEventCard)
        {
            fieldEventCard.gameObject.SetActive(false);
            bochiCards.Add(fieldEventCard);
            fieldEventCard = null;
        }
        //fieldStrikerCard.gameObject.SetActive(false);
        //fieldEventCard.gameObject.SetActive(false);
    }

    public bool isSetEndStriker()
    {
        if(fieldStrikerCard != null)
        {
            return fieldStrikerCard.isSetField;
        }
        return false;
    }

    public void Draw()
    {

    }


    public void SetDeckData(CardData[] yamahuda, bool isMyPlayer)
    {
        // デッキデータ
        for (int i = 0; i < yamahuda.Length; i++)
        {
            Card card;

            if (i >= yamahudaCards.Count)
            {
                card = Instantiate(cardPrefab, transform);
                yamahudaCards.Add(card);
            }
            else card = yamahudaCards[i];

            card.isMyPlayerSide = isMyPlayer;
            //card.SetCardData(yamahuda[i]);
        }

        // 山札位置更新
        UpdateYamahudaPosition();
    }

    public void Marigan(CardData[] yamahuda)
    {
        var orgNumYamahuda = yamahudaCards.Count;

        while(handCards.Count > 0)
        {
            // 手札→山札
            var card = handCards[handCards.Count - 1];
            handCards.Remove(card);
            yamahudaCards.Add(card);
        }

        for(int i= orgNumYamahuda; i<yamahudaCards.Count;i++)
        {
            var card = yamahudaCards[i];

            // 手札にいる位置を保存
            var position = card.cacheTransform.localPosition;
            var angle = card.cacheTransform.localEulerAngles;

            // デッキ位置をセット
            MakeYamahudaTransform(i, card);

            // マリガンステートセット
            card.Marigan(position, angle);
        }
    }

    public bool isInMovingState()
    {
        foreach(Card card in yamahudaCards)
        {
            // 何かしらの動いてるステートにいる
            if (!card.stateMachine.isInState(CardObjectState.None.GetInstance())) return true;
        }
        foreach(Card card in handCards)
        {
            // 何かしらの動いてるステートにいる
            if (!card.stateMachine.isInState(CardObjectState.None.GetInstance())) return true;
        }
        // 誰も動いてるステートにいない
        return false;
    }
}
