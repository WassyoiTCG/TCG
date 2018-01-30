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
    List<CardData> hand = new List<CardData>();                         // 手札
    List<CardData> yamahuda = new List<CardData>();                     // 山札
    List<CardData> cemetery = new List<CardData>();                     // 墓地
    List<CardData> expulsion = new List<CardData>();                  // 追放

    private List<CardData> haveAbilityCards = new List<CardData>();     // 所持効果持ちモンスター

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
        cemetery.Clear();
        expulsion.Clear();

    }

    public void Marigan(SyncDeckInfo syncData)
    {
        Clear();

        foreach (int yamahudaID in syncData.yamahuda)
        {
            if (yamahudaID == -1) break;
            yamahuda.Add(CardDataBase.GetCardData(yamahudaID));
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
            // ★★★嫌な予感がする…(参照)
            //var cardData = new CardData();
            //cardData = CardDataBase.GetCardData(yamahudaID);
            //var abilityData = cardData.GetAbilityData();
            //if(abilityData != null)
            //{
            //    abilityData.SetMyPlayerFlag(player.isMyPlayer);
            //}

            var cardData = CardDataBase.GetCardData(yamahudaID);
            yamahuda.Add(cardData);
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
        cardObjectManager.Restart(this);
    }

    public void SetDeckData(PlayerDeckData deckData, bool updateCardObject = true)
    {
        Clear();

        int[] cardID = deckData.GetArray();
        // デッキデータを山札に入れていく
        foreach (int id in cardID)
        {
            if (id != (int)IDType.NONE)
            {
                //// ★★★嫌な予感がする…(参照)
                //var cardData = new CardData();
                //cardData = CardDataBase.GetCardData(id);
                //var abilityData = cardData.GetAbilityData();
                //if (abilityData != null)
                //{
                //    abilityData.SetMyPlayerFlag(player.isMyPlayer);
                //}

                var cardData = CardDataBase.GetCardData(id);
                yamahuda.Add(cardData);

                //// 所持効果持ちファイターを保存
                //if (cardData.cardType == CardType.AbilityFighter )
                //{
                //    haveAbilityCards.Add(cardData);
                //}
                
            }
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

    public void SarchAbilityCards(PlayerDeckData deckData)
    {
        haveAbilityCards.Clear();
     
        int[] cardID = deckData.GetArray();
        
        // デッキデータを山札に入れていく
        foreach (int id in cardID)
        {
            if (id != (int)IDType.NONE)
            {
                var cardData = CardDataBase.GetCardData(id);

                // 所持効果持ちファイターを保存
                if (cardData.cardType == CardType.AbilityFighter)
                {
                    haveAbilityCards.Add(cardData);
                }

            }
        }

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

    CardData GetDraw()
    {
        // 山札切れの場合、墓地から全部山札に戻す
        if (yamahuda.Count == 0)
        {
            //// 墓地もない(全部追放されていたら)死ぬ
            //if (cemetery.Count == 0)
            //{
            //    Debug.Log("ルナの負けだよ");
            //    return null;
            //}

            //// 墓地がなくなるまで
            //while (cemetery.Count > 0)
            //{
            //    var lastIndex = cemetery.Count - 1;
            //    // 墓地→山札
            //    yamahuda.Enqueue(cemetery[lastIndex]);
            //    cemetery.RemoveAt(lastIndex);
            //}
            //// 山札シャッフル
            //Shuffle(ref yamahuda);

            // デッキ補充
            //cardObjectManager.Restart();

            // ★デッキ補充なし
            return null;
        }
        var ret = yamahuda[yamahuda.Count - 1];
        yamahuda.Remove(ret);
        // 山札から1枚はき出す
        return ret;
        //Debug.Log("Player[" + player.playerID + "]ドロー: " + ret[i].id);
        //ret[i].gameObject.SetActive(true);
    }

    // 墓地からランダムにストライカーを引くときに使う
    public int GetCemeteryRandomNumberStriker()
    {
        if (cemetery.Count <= 0) return (int)IDType.NONE;

        int cemeteryIndex = (int)IDType.NONE;

        // 墓地から無造作にストライカーを探す
        int[] randomArray = oulRandom.GetRandomArray(0, cemetery.Count);
        foreach(int r in randomArray)
        {
            var card = cemetery[r];
            // ストライカー系のカードだったら
            if(card.isStrikerCard())
            {
                //cemetery.RemoveAt(r);
                cemeteryIndex = r;
                break;
            }
        }

        return cemeteryIndex;
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

    public void AddHand(Card card)
    {
        hand.Add(card.cardData);
        cardObjectManager.AddHand(card);
    }

    public void AddBochi(Card card)
    {
        cemetery.Add(card.cardData);
        cardObjectManager.AddCemetery(card);
    }

    public void AddExpulsion(Card card)
    {
        expulsion.Add(card.cardData);
        cardObjectManager.AddExpulsion(card);
    }

    public void TurnEnd()
    {
        if(fieldStrikerCard != null)
        {
            // 手札から消す
            hand.Remove(fieldStrikerCard);
            // 墓地に送る
            cemetery.Add(fieldStrikerCard);
            fieldStrikerCard = null;
        }
        if (fieldEventCard != null)
        {
            // 手札から消す
            hand.Remove(fieldEventCard);
            // 墓地に送る
            cemetery.Add(fieldEventCard);
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
    public List<CardData> GetYamahuda() { return yamahuda; }

    public CardData GetCemeteryCard(int bothiNo)
    {
        return cemetery.ToArray()[bothiNo];
    }

    public List<CardData> GetCemeteryCards() { return cemetery; }

    public CardData GetExpulsionCard(int tuihouNo){ return expulsion[tuihouNo]; }

    public List<CardData> GetExpulsionCards() { return expulsion; }

    public List<CardData> GetHaveAbilityCards() { return haveAbilityCards; }

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
        cardObjectManager.UpdateHandPosition();
    }

    public void SetSupport(int handNo)
    {
        if (handNo < 0 || handNo >= hand.Count)
        {
            Debug.LogWarning("配列参照外" + handNo);
            return;
        }

        var card = hand[handNo];
        //var type = card.cardType;
        //switch (type)
        //{
        //    case CardType.Support:
        //        fieldEventCard = card;
        //        break;

        //    default:
        //        Debug.LogWarning("サポート以外がセットされてるクマ");
        //        break;
        //}

        cardObjectManager.FieldSet(handNo, player.isMyPlayer);

        // ★★★手札から消す
        hand.Remove(card);

        //cardObjectManager.UpdateHand(hand, player);
        cardObjectManager.UpdateHandPosition();
    }

    public void SetIntercept(int handNo)
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
            case CardType.Intercept:
                fieldEventCard = card;
                break;
            case CardType.Joker:
                fieldEventCard = card;
                Debug.Log("JOKERをインターセプトとしてセットしたんごクマ");
                break;
            default:
                Debug.LogWarning("インターセプト以外がセットされてるクマ");// ありがたい
                break;
        }

        // フィールドにカードを置く
        cardObjectManager.FieldSetToIntercept(handNo, player.isMyPlayer);

        // ★★★手札から消す
        hand.Remove(card);

        //cardObjectManager.UpdateHand(hand, player);
        cardObjectManager.UpdateHandPosition();
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
        cardObjectManager.BackToHand();
    }

    void Shuffle(ref List<CardData> deck)
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
            deck. Add(array[i]);
        }
    }

    //void Tuihou()
    //{
    //    if (bochi.Count > 0) tsuihou.Enqueue(bochi.Dequeue());
    //}

    // 手札から1枚吐き出す
    public Card DequeueHand(int handNo)
    {
        // (TODO)0の時は発動させないようにする？
        if(hand.Count > 0)
        {
            //var r = Random.Range(0, hand.Count - 1);
            var card = hand[handNo];
            hand.Remove(card);
            // (TODO)手札カードオブジェクト更新
            //cardObjectManager.UpdateHand(hand, player);
            return cardObjectManager.DequeueHand(handNo);
        }

        return null;
    }

    // CPUが出す用
    //public int GetHandNoRandomStriker()
    //{
    //    int[] randomArray = oulRandom.GetRandomArray(0, GetNumHand());

    //    foreach(int r in randomArray)
    //    {
    //        if (hand[r].isEventCard()) continue;
    //        if (hand[r].isNotSelect) continue;
    //        return r;
    //    }

    //    // 持ってない
    //    return (int)IDType.NONE;
    //}

    // 山札から1枚吐き出す
    public Card DequeueYamahuda()
    {
        //// (TODO)LO
        //if (yamahuda.Count > 0)
        //{
        //    var card = yamahuda.Dequeue();
        //    // 山札カードオブジェクト更新
        //    //cardObjectManager.UpdateYamahuda(yamahuda, player);
        //    return card;
        //}

        var draw = GetDraw();
        return cardObjectManager.DequeueYamahuda(draw);
    }

    public Card DequeueYamahuda(int yamahudaNumber)
    {
        // (TODO)0の時は発動させないようにする？
        if (yamahuda.Count > 0)
        {
            var card = yamahuda[yamahudaNumber];
            yamahuda.Remove(card);
            return cardObjectManager.DequeueYamahuda(card);
        }

        return null;
    }

    public Card DequeCemetery(int cemeteryNumber)
    {
        // (TODO)0の時は発動させないようにする？
        if (cemetery.Count > 0)
        {
            // らんだむなら無造作
            if(cemeteryNumber == (int)IDType.RANDOM)
            {
                cemeteryNumber = Random.Range(0, cemetery.Count - 1);
            }
            var card = cemetery[cemeteryNumber];
            cemetery.Remove(card);
            return cardObjectManager.DequeueCemetery(cemeteryNumber);
        }

        return null;
    }

    public int GetNumHand() { return hand.Count; }
    public int GetNumYamahuda() { return yamahuda.Count; }
    public int GetNumCemetery() { return cemetery.Count; }
    public int GetNumTsuihou() { return expulsion.Count; }

    // そのカードの属性が墓地にいるか
    public bool isExistBochiCardType(CardType type)
    {
        foreach(CardData card in cemetery)
        {
            if (card.cardType == type) return true;
        }
        return false;
    }

    //public bool isHaveStrikerCard()
    //{
    //    // 1枚でもストライカーカードを持っているならtrue
    //    foreach(CardData card in hand)
    //    {
    //        // (isStrikerはジョーカーがfalseになるのでこの書き方)
    //        if (!card.isEventCard()) return true;
    //    }
    //    return false;
    //}

    //public bool isHaveInterceptCard()
    //{
    //    // 1枚でもインターセプトカードを持っているなら
    //    foreach (CardData card in hand)
    //    {
    //        if (card.cardType != CardType.Intercept) continue;

    //        // ★さらにインターセプトの発動条件を満たしているかどうか(持ってても発動条件を満たしていないなら選べない)
    //        if (!card.interceptCard.abilityData.HatsudouOK(player)) continue;

    //        // OK
    //        return true;
    //    }
    //    return false;
    //}
}
