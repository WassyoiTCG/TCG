using System.Collections.Generic;
using UnityEngine;

//public class DeckData
//{
//    public CardData[] fighterCards = new CardData[10];         // ファイター(1～10のパワーそれぞれ1枚ずつ、効果あり4体まで)
//    public CardData[] eventCards = new CardData[4];            // イベントカード(0～4枚)
//    public CardData jorkerCard;                            // ジョーカー
//}


public class DeckManager
{
    const int Maxhand = 9;  // 手札の最大数
    List<CardData> hand = new List<CardData>();         // 手札
    Queue<CardData> yamahuda = new Queue<CardData>();   // 山札
    List<CardData> bochi = new List<CardData>();      // 墓地
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

    public void Marigan(SyncDeckInfo syncData)
    {
        Clear();

        foreach (int yamahudaID in syncData.yamahuda)
        {
            if (yamahudaID == -1) break;
            yamahuda.Enqueue(CardDataBase.GetCardData(yamahudaID));
        }

        cardObjectManager.Marigan(yamahuda.ToArray());
    }

    public void Sync(SyncDeckInfo syncData)
    {
        Clear();
        //foreach (int handID in syncData.hand)
        //{
        //    if (handID == -1) break;
        //    hand.Add(CardDataBase.GetCardData(handID));
        //}
        foreach (int yamahudaID in syncData.yamahuda)
        {
            if (yamahudaID == -1) break;
            yamahuda.Enqueue(CardDataBase.GetCardData(yamahudaID));
        }
        //foreach (int bochiID in syncData.bochi)
        //{
        //    if (bochiID == -1) break;
        //    bochi.Add(CardDataBase.GetCardData(bochiID));
        //}
        //foreach (int tuihouID in syncData.tuihou)
        //{
        //    if (tuihouID == -1) break;
        //    tsuihou.Enqueue(CardDataBase.GetCardData(tuihouID));
        //}

        // 向こうから来るやつにつかう
        cardObjectManager.SetDeckData(yamahuda.ToArray(), player.isMyPlayer);

        // その情報をもとにカードオブジェクト更新
        //cardObjectManager.UpdateHand(hand, player);
        //cardObjectManager.UpdateYamahuda(yamahuda, player);
    }

    public void Start(Player myPlayer, CardObjectManager objectManager)
    {
        player = myPlayer;
        cardObjectManager = objectManager;
    }

    public void Reset()
    {
        Clear();

        fieldStrikerCard = null;
        fieldEventCard = null;
        cardObjectManager.Restart();
    }

    public void SetDeckData(PlayerDeckData deckData, bool updateCardObject = true)
    {
        Clear();

        int[] cardID = deckData.GetArray();
        // デッキデータを山札に入れていく
        foreach (int id in cardID)
        {
            if (id != (int)IDType.NONE) yamahuda.Enqueue(CardDataBase.GetCardData(id));
        }
        //foreach (CardData card in deckData.eventCards)
        //{
        //    if (card == null) continue;
        //    yamahuda.Enqueue(card);
        //}
        //yamahuda.Enqueue(deckData.jorkerCard);
        // 山札シャッフル
        Shuffle(ref yamahuda);

        if (updateCardObject)
            cardObjectManager.SetDeckData(yamahuda.ToArray(), player.isMyPlayer);
    }
     
    public void Draw(int maisuu)
    {
        CardData[] ret = new CardData[maisuu];
        for (int i = 0; i < maisuu; i++)
        {
            ret[i] = GetDraw();
        }

        // 手札に加えナイト
        AddHand(ret);

        // カードオブジェクトにドローの動きをさせる
        cardObjectManager.Draw(ret);

        // 山札更新
        //cardObjectManager.UpdateYamahuda(yamahuda, player);

        //return ret;
    }

    public CardData GetDraw()
    {
        // 山札切れの場合、墓地から全部山札に戻す
        if (yamahuda.Count == 0)
        {
            // 墓地もない(全部追放されていたら)死ぬ
            if (bochi.Count == 0)
            {
                Debug.Log("ルナの負けだよ");
                return null;
            }

            // 墓地がなくなるまで
            while (bochi.Count > 0)
            {
                var lastIndex = bochi.Count - 1;
                // 墓地→山札
                yamahuda.Enqueue(bochi[lastIndex]);
                bochi.RemoveAt(lastIndex);
            }
            // 山札シャッフル
            Shuffle(ref yamahuda);
        }
        // 山札から1枚はき出す
        return yamahuda.Dequeue();
        //Debug.Log("Player[" + player.playerID + "]ドロー: " + ret[i].id);
        //ret[i].gameObject.SetActive(true);
    }

    public CardData DrawBochiStriker()
    {
        if (bochi.Count <= 0) return null;

        // 墓地から無造作にストライカーを探す
        int[] randomArray = oulRandom.GetRandomArray(0, bochi.Count - 1);
        foreach(int r in randomArray)
        {
            var card = bochi[r];
            // ストライカー系のカードだったら
            if(card.isStrikerCard())
            {
                bochi.RemoveAt(r);
                return card;
            }
        }

        return null;
    }

    void AddHand(CardData[] cards)
    {
        foreach (CardData card in cards)
            hand.Add(card);

        // カードオブジェクト更新
        //cardObjectManager.UpdateHand(hand, player);

        // 手札カードの位置更新
        //UpdateHandPosition();
    }

    public bool isSetStriker() { return (fieldStrikerCard != null); }

    public void AddHand(CardData card)
    {
        hand.Add(card);
    }

    public void AddBochi(CardData card)
    {
        bochi.Add(card);
    }

    public void TurnEnd()
    {
        if(fieldStrikerCard != null)
        {
            // 手札から消す
            hand.Remove(fieldStrikerCard);
            // 墓地に送る
            bochi.Add(fieldStrikerCard);
            fieldStrikerCard = null;
        }
        if (fieldEventCard != null)
        {
            // 手札から消す
            hand.Remove(fieldEventCard);
            // 墓地に送る
            bochi.Add(fieldEventCard);
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

    public CardData GetCemeteryCard(int bothiNo)
    {
        return bochi.ToArray()[bothiNo];
    }

    public CardData[] GetCemeteryCards() { return bochi.ToArray(); }

    public CardData GetExpulsionCard(int tuihouNo)
    {
        return tsuihou.ToArray()[tuihouNo];
    }

    public CardData[] GetExpulsionCards() { return tsuihou.ToArray(); }

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

        //cardObjectManager.UpdateHand(hand, player);
        cardObjectManager.UpdateHandPosition(this);
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
        cardObjectManager.BackToHand(this);
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

    //void Tuihou()
    //{
    //    if (bochi.Count > 0) tsuihou.Enqueue(bochi.Dequeue());
    //}

    // 手札から無造作に1枚吐き出す
    public CardData DequeueHand()
    {
        // (TODO)0の時は発動させないようにする？
        if(hand.Count > 0)
        {
            var card = hand[Random.Range(0, hand.Count - 1)];
            hand.Remove(card);
            // (TODO)手札カードオブジェクト更新
            //cardObjectManager.UpdateHand(hand, player);
            return card;
        }

        return null;
    }

    // 山札から1枚吐き出す
    public CardData DequeueYamahuda()
    {
        // (TODO)LO
        if (yamahuda.Count > 0)
        {
            var card = yamahuda.Dequeue();
            // 山札カードオブジェクト更新
            //cardObjectManager.UpdateYamahuda(yamahuda, player);
            return card;
        }

        return null;
    }

    public int GetNumHand() { return hand.Count; }
    public int GetNumYamahuda() { return yamahuda.Count; }
    public int GetNumBochi() { return bochi.Count; }
    public int GetNumTsuihou() { return tsuihou.Count; }

    // そのカードの属性が墓地にいるか
    public bool isExistBochiCardType(CardType type)
    {
        foreach(CardData card in bochi)
        {
            if (card.cardType == type) return true;
        }
        return false;
    }

    public bool isHaveStrikerCard()
    {
        // 1枚でもストライカーカードを持っているならtrue
        foreach(CardData card in hand)
        {
            // (isStrikerはジョーカーがfalseになるのでこの書き方)
            if (!card.isEventCard()) return true;
        }
        return false;
    }
}
