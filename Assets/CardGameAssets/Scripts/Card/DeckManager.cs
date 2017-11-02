using System.Collections.Generic;
using UnityEngine;

public class DeckData
{
    public CardData[] fighterCards = new CardData[10];         // ファイター(1～10のパワーそれぞれ1枚ずつ、効果あり4体まで)
    public CardData[] eventCards = new CardData[4];            // イベントカード(0～4枚)
    public CardData jorkerCard;                            // ジョーカー
}


public class DeckManager
{
    const int Maxhand = 9;  // 手札の最大数
    List<CardData> hand = new List<CardData>();         // 手札
    Queue<CardData> yamahuda = new Queue<CardData>();   // 山札
    Queue<CardData> bochi = new Queue<CardData>();      // 墓地
    Queue<CardData> tsuihou = new Queue<CardData>();     // 追放

    Player player;
    CardObjectManager cardObjectManager;

    // フィールドにあるカード
    public CardData fieldStrikerCard { get; private set; }
    public CardData fieldEventCard { get; private set; }

    void Clear()
    {
        // スタック初期化
        hand.Clear();
        yamahuda.Clear();
        bochi.Clear();
        tsuihou.Clear();
    }

    public void Sync(SyncDeckInfo syncData)
    {
        Clear();
        foreach (int handID in syncData.hand)
        {
            if (handID == -1) break;
            hand.Add(CardDataBase.GetCardData(handID));
        }
        foreach (int yamahudaID in syncData.yamahuda)
        {
            if (yamahudaID == -1) break;
            yamahuda.Enqueue(CardDataBase.GetCardData(yamahudaID));
        }
        foreach (int bochiID in syncData.bochi)
        {
            if (bochiID == -1) break;
            bochi.Enqueue(CardDataBase.GetCardData(bochiID));
        }
        foreach (int tuihouID in syncData.tuihou)
        {
            if (tuihouID == -1) break;
            tsuihou.Enqueue(CardDataBase.GetCardData(tuihouID));
        }

        // その情報をもとにカードオブジェクト更新
        cardObjectManager.UpdateHand(hand, player);
        cardObjectManager.UpdateYamahuda(yamahuda, player);
    }

    public void Start(Player myPlayer, CardObjectManager objectManager)
    {
        player = myPlayer;
        cardObjectManager = objectManager;
    }

    public void Reset(DeckData deckData)
    {
        Clear();
        // デッキデータを山札に入れていく
        foreach (CardData card in deckData.fighterCards)
        {
            yamahuda.Enqueue(card);
        }
        foreach (CardData card in deckData.eventCards)
        {
            if (card == null) continue;
            yamahuda.Enqueue(card);
        }
        yamahuda.Enqueue(deckData.jorkerCard);
        // 山札シャッフル
        Shuffle(ref yamahuda);
    }

    public void Draw(int maisuu)
    {
        CardData[] ret = new CardData[maisuu];
        for (int i = 0; i < maisuu; i++)
        {
            // 山札切れの場合、墓地から全部山札に戻す
            if (yamahuda.Count == 0)
            {
                // 墓地もない(全部追放されていたら)死ぬ
                if (bochi.Count == 0)
                {
                    Debug.Log("ルナの負けだよ");
                    break;
                }

                // 墓地がなくなるまで
                while (bochi.Count > 0)
                {
                    // 墓地→山札
                    yamahuda.Enqueue(bochi.Dequeue());
                }
                // 山札シャッフル
                Shuffle(ref yamahuda);
            }
            // 山札から1枚はき出す
            ret[i] = yamahuda.Dequeue();
            //Debug.Log("Player[" + player.playerID + "]ドロー: " + ret[i].id);
            //ret[i].gameObject.SetActive(true);
        }

        // 手札に加えナイト
        AddHand(ret);

        // 山札更新
        cardObjectManager.UpdateYamahuda(yamahuda, player);

        //return ret;
    }

    void AddHand(CardData[] cards)
    {
        foreach (CardData card in cards)
            hand.Add(card);

        // カードオブジェクト更新
        cardObjectManager.UpdateHand(hand, player);

        // 手札カードの位置更新
        //UpdateHandPosition();
    }

    public bool isSetStriker() { return (fieldStrikerCard != null); }

    public void AddBochi(CardData card)
    {
        bochi.Enqueue(card);
    }

    public void TurnEnd()
    {
        if(fieldStrikerCard != null)
        {
            // 手札から消す
            hand.Remove(fieldStrikerCard);
            // 墓地に送る
            bochi.Enqueue(fieldStrikerCard);
            fieldStrikerCard = null;
        }
        if (fieldEventCard != null)
        {
            // 手札から消す
            hand.Remove(fieldEventCard);
            // 墓地に送る
            bochi.Enqueue(fieldEventCard);
            fieldEventCard = null;
        }
        // カードオブジェクト更新
        cardObjectManager.TurnEnd();
        //cardObjectManager.UpdateHand(hand, player);
    }

    public CardData GetHandCard(int handNo)
    {
        return hand[handNo];
    }

    public List<CardData> GetHand() { return hand; }

    public CardData GetYamahudaCard(int yamahudaNo)
    {
        return yamahuda.ToArray()[yamahudaNo];
    }

    public CardData GetBochiCard(int bothiNo)
    {
        return bochi.ToArray()[bothiNo];
    }

    public CardData GetTuihouCard(int tuihouNo)
    {
        return tsuihou.ToArray()[tuihouNo];
    }

    public void FieldSet(int handNo)
    {
        if (handNo < 0 || handNo >= hand.Count)
        {
            Debug.LogWarning("配列参照外" + handNo);
            return;
        }

        var card = hand[handNo];
        var type = card.cardType;
        switch (type)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
            case CardType.Joker:
                // フィールドにおいてるストライカーのカード
                fieldStrikerCard = card;
                break;

            //case CardType.Event:
            //    // フィールドにおいてるストライカーのカード
            //    fieldEventCard = card;
            //    break;
        }

        cardObjectManager.FieldSet(handNo, player.isMyPlayer);
       
        // ★★★手札から消す
        hand.Remove(card);

        cardObjectManager.UpdateHand(hand, player);
    }

    public void BackToHand(BackToHandInfo info)
    {
        switch((CardType)info.iCardType)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
            case CardType.Joker:
                // 一番後ろに
                hand.Add(fieldStrikerCard);
                fieldStrikerCard = null;
                break;

            case CardType.Support:
            case CardType.Connect:
            case CardType.Intercept:

                break;
        }
        cardObjectManager.BackToHand(hand, player);
    }

    void Shuffle(ref Queue<CardData> deck)
    {
        if (deck.Count == 0) return;

        // スタック→配列
        CardData[] array = deck.ToArray();
        // スタック空にする
        deck.Clear();
        // ランダム配列作成
        int[] randomArray = oulRandom.GetRandomArray(0, array.Length);
        // スタック再挿入
        foreach (int i in randomArray)
        {
            deck.Enqueue(array[i]);
        }
    }

    void Tuihou()
    {
        if (bochi.Count > 0) tsuihou.Enqueue(bochi.Dequeue());
    }

    public int GetNumHand() { return hand.Count; }
    public int GetNumYamahuda() { return yamahuda.Count; }
    public int GetNumBochi() { return bochi.Count; }
    public int GetNumTsuihou() { return tsuihou.Count; }

    public bool isHaveStrikerCard()
    {
        // 1枚でもストライカーカードを持っているならtrue
        foreach(CardData card in hand)
        {
            if (card.isStrikerCard()) return true;
        }
        return false;
    }
}
