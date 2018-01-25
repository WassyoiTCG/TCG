using System;
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
    List<Card> cemeteryCards = new List<Card>();    // 墓地のカード
    Queue<Card> expulsionCards = new Queue<Card>(); // 追放のカード
    public Card fieldStrikerCard { get; private set; }
    public Card fieldEventCard { get; private set; }

    public Vector3 handShift;           // カード位置補正
    public Vector3 handCardDir;         // カードの向き(法線的な)

    readonly Vector3 yamahudaField = new Vector3(15, 0, -7);      // 山札の位置
    readonly Vector3 strikerField = new Vector3(0, 0, -4);     // ストライカーカードをセットする位置
    readonly Vector3 eventField = new Vector3(-5, 0, -4);       // イベントカードをセットする位置

    public Vector3 cemeteryPosition = Vector3.zero;

    public Texture strikerFrame, abilityStrikerFrame, jokerFrame, supportFrame, eventFrame;

    // Use this for initialization
    void Start()
    {
        // 墓地の位置を取ってくる

        //// カードの実体を生成

        //// 手札カード
        //handCards.Clear();
        ////for (int i=0;i< handCards.Length;i++)
        ////{
        ////    handCards[i] = Instantiate(cardPrefab, transform);
        ////    handCards[i].name = "HandCard" + i;
        ////    handCards[i].gameObject.SetActive(false);
        ////}

        //// 山札カード
        //yamahudaCards.Clear();
        ////for (int i=0;i< yamahudaCards.Length;i++)
        ////{
        ////    yamahudaCards[i] = Instantiate(cardPrefab, transform);
        ////    yamahudaCards[i].name = "Card" + i;
        ////    yamahudaCards[i].gameObject.SetActive(false);
        ////}

        //// フィールドカード
        ////fieldStrikerCard = Instantiate(cardPrefab, transform);
        ////fieldStrikerCard.name = "FieldStrikerCard";
        ////fieldStrikerCard.gameObject.SetActive(false);
        ////fieldEventCard = Instantiate(cardPrefab, transform);
        ////fieldEventCard.name = "FieldEventCard";
        ////fieldEventCard.gameObject.SetActive(false);
    }


    public void Restart()
    {
        // 共通処理なのでラムダ
        Func<Card, int> addYamahuda = (card) => 
        {
            // アビリティから作られたカードなら削除
            if (card.isAbilityCreate)
            {
                GameObject.Destroy(card.gameObject);
            }
            else
            {
                // 山札に加える(再利用)
                yamahudaCards.Add(card);
            }

            return 0;
        };

        // フィールドカード
        //fieldStrikerCard.gameObject.SetActive(false);
        //fieldEventCard.gameObject.SetActive(false);

        if (fieldStrikerCard)
        {
            addYamahuda(fieldStrikerCard);
            //yamahudaCards.Add(fieldStrikerCard);
            fieldStrikerCard = null;
        }

        if (fieldEventCard)
        {
            addYamahuda(fieldEventCard);
            //yamahudaCards.Add(fieldEventCard);
            fieldEventCard = null;
        }

        // 手札を空にする
        while (handCards.Count > 0)
        {
            var card = handCards[0];
            handCards.Remove(card);
            card.gameObject.SetActive(true);
            addYamahuda(card);
            //yamahudaCards.Add(card);
        }

        // 墓地を空にする
        while (cemeteryCards.Count > 0)
        {
            var card = cemeteryCards[0];
            cemeteryCards.Remove(card);
            card.gameObject.SetActive(true);
            addYamahuda(card);
            //yamahudaCards.Add(card);
        }

        // 山札
        for (int i = 0; i < yamahudaCards.Count; i++)
        {
            var card = yamahudaCards[i];
            if (!card.gameObject.activeInHierarchy) break;

            // 位置情報設定
            card.ChangeState(CardObjectState.None.GetInstance());
            // ×エフェクトを消す
            card.banEffect.gameObject.SetActive(false);
            // 使えないフラグを戻す
            card.SetNotSelectFlag(false);
        }

        // 山札座標更新
        UpdateYamahudaPosition();
    }

    // Update is called once per frame
    void Update ()
    {
        // マイフレームチェック
       // CheakActiveUseCard();

    }

    public Card[] GetHandCardObject() { return handCards.ToArray(); }

    public Card DequeueHand(int handNo)
    {
        Card draw = null;
        if(handNo < handCards.Count)
        {
            draw = handCards[handNo];
            handCards.Remove(draw);

            // 手札位置更新
            UpdateHandPosition();
        }
        else Debug.Log("手札ないクマ");
        return draw;
    }

    public Card DequeueYamahuda(CardData cardData)
    {
        Card draw = null;
        if (yamahudaCards.Count > 0)
        {
            draw = yamahudaCards[yamahudaCards.Count - 1];
            yamahudaCards.Remove(draw);
            draw.SetCardData(cardData);
        }
        else Debug.Log("LOしてるグマ");

        return draw;
    }

    public Card DequeueCemetery(int cemeteryNo)
    {
        if (cemeteryNo == (int)IDType.NONE) return null;

        Card draw = null;
        if (cemeteryNo < cemeteryCards.Count)
        {
            draw = cemeteryCards[cemeteryNo];
            cemeteryCards.Remove(draw);
            draw.gameObject.SetActive(true);
        }
        else Debug.Log("墓地無いクマ");

        // 墓地位置更新
        UpdateCemeteryPosition();

        return draw;
    }

    // 手札をストライカーセット状態にする(コネクトとインターセプトを選択不可に)
    public void ChangeHandSetStrikerMode(Player player)
    {
        // 0123 パワー制限制限
        Func<int, bool> CheckLimitPower = null;
        int limitValue = player.GetLimitPowerData().value;
        bool isLimit = true;
        // ラムダ関数登録
        switch(player.GetLimitPowerData().type)
        {
            case Player.LimitPowerType.Ika:
                CheckLimitPower = (power) => { return (power <= limitValue); };
                break;

            case Player.LimitPowerType.Ijou:
                CheckLimitPower = (power) => { return (power >= limitValue); };
                break;

            case Player.LimitPowerType.Kisuu:
                CheckLimitPower = (power) => { return (power % 2 == 1); };
                break;

            case Player.LimitPowerType.Guusuu:
                CheckLimitPower = (power) => { return (power % 2 == 0); };
                break;

            case Player.LimitPowerType.NoneLimit:
            default:
                isLimit = false;
                break;
        }

        foreach(Card card in handCards)
        {
            switch(card.cardData.cardType)
            {
                // ストライカー系は無条件で選択可能
                case CardType.Fighter:
                    // パワー制限がある場合
                    if (isLimit)
                    {
                        // パワー制限に引っかかるかチェック
                        // リミット条件満たしていないなら選べなくする
                        if (!CheckLimitPower(card.cardData.power))
                        {
                            card.SetNotSelectFlag(true);
                            // ×発動
                            card.banEffect.gameObject.SetActive(true);
                            card.banEffect.Action();
                        }
                        else
                        {
                            card.SetNotSelectFlag(false);
                            // ×を消す
                            card.banEffect.gameObject.SetActive(false);
                        }
                    }
                    // 制限がない状態なら、選べるようにする
                    else
                    {
                        card.SetNotSelectFlag(false);
                        // ×を消す
                        card.banEffect.gameObject.SetActive(false);
                    }
                    break;
                case CardType.AbilityFighter:
                    // パワー制限がある場合
                    if (isLimit)
                    {
                        // パワー制限に引っかかるかチェック
                        // リミット条件満たしていないなら選べなくする
                        if (!CheckLimitPower(card.cardData.power))
                        {
                            card.SetNotSelectFlag(true);
                            // ×発動
                            card.banEffect.gameObject.SetActive(true);
                            card.banEffect.Action();
                        }
                        else
                        {
                            card.SetNotSelectFlag(false);
                            // ×を消す
                            card.banEffect.gameObject.SetActive(false);
                        }
                    }
                    // 制限がない状態なら、選べるようにする
                    else
                    {
                        card.SetNotSelectFlag(false);
                        // ×を消す
                        card.banEffect.gameObject.SetActive(false);
                    }
                    break;
                case CardType.Joker:
                    // パワー制限がある場合
                    if (isLimit)
                    {
                        // パワー制限に引っかかるかチェック
                        // リミット条件満たしていないなら選べなくする
                        if (!CheckLimitPower(card.cardData.power))
                        {
                            card.SetNotSelectFlag(true);
                            // ×発動
                            card.banEffect.gameObject.SetActive(true);
                            card.banEffect.Action();
                        }
                        else
                        {
                            card.SetNotSelectFlag(false);
                            // ×を消す
                            card.banEffect.gameObject.SetActive(false);
                        }
                    }
                    // 制限がない状態なら、選べるようにする
                    else
                    {
                        card.SetNotSelectFlag(false);
                        // ×を消す
                        card.banEffect.gameObject.SetActive(false);
                    }
                    break;

                case CardType.Support:
                case CardType.Connect:
                    bool hatsudouOK = false;
                    // はつどう条件を満たしているなら(今日は休みますならジョーカーが手札にあるかとか)
                    var abilityes = card.cardData.GetEventCard().abilityDatas;
                    foreach (CardAbilityData ability in abilityes)
                    {
                        if (ability.HatsudouOK(player))
                        {
                            card.SetNotSelectFlag(false);
                            hatsudouOK = true;
                            break;
                        }
                    }
                    if(!hatsudouOK)card.SetNotSelectFlag(true);
                    break;

                    // インターセプトは無条件で選択不可能
                case CardType.Intercept:
                    card.SetNotSelectFlag(true);
                    break;
            }
        }
    }

    public void ChangeHandSetInterceptMode(Player player)
    {
        foreach (Card card in handCards)
        {
            switch (card.cardData.cardType)
            {
                // インターセプト以外は選択不可能
                case CardType.Fighter:
                case CardType.AbilityFighter:
                //case CardType.Joker:
                case CardType.Support:
                case CardType.Connect:
                    card.SetNotSelectFlag(true);
                    break;

                // インターセプト
                case CardType.Intercept:
                // 1219編集
                    bool hatsudouOK = false;
                    // はつどう条件を満たしているなら(今日は休みますならジョーカーが手札にあるかとか)
                    var abilityes = card.cardData.interceptCard.abilityDatas;
                    foreach (CardAbilityData ability in abilityes)
                    {
                        if (ability.HatsudouOK(player))
                        {
                            card.SetNotSelectFlag(false);
                            hatsudouOK = true;
                            break;
                        }
                    }
                    if (!hatsudouOK) card.SetNotSelectFlag(true);
                    break;

                case CardType.Joker:
                     bool hatsudouOK2 = false;
                    // はつどう条件を満たしているなら(今日は休みますならジョーカーが手札にあるかとか)
                     var abilityes2 = card.cardData.jokerCard.abilityDatas;
                     foreach (CardAbilityData ability in abilityes2)
                    {
                        if (ability.HatsudouOK(player))

                        {
                            card.SetNotSelectFlag(false);
                            hatsudouOK2 = true;
                            break;
                        }
                    }
                    if (!hatsudouOK2) card.SetNotSelectFlag(true);
                    break;
            }
        }
    }

    public void ChangeHandNoneSelect()
    {
        // 全部選択できないように
        foreach (Card card in handCards)
        {
            card.SetNotSelectFlag(true);
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
    public void UpdateHandPosition()
    {
        for(int i = 0; i < handCards.Count; i++)
        {
            var card = handCards[i];

            //if (!handCards[i].isActiveAndEnabled) break;

            // 表
            card.SetUraomote(true);
            // 描画順
            card.SetOrder(i);

            MakeHandTransform(i, handCards.Count, card);
        }
    }

    public void MakeHandTransform(int handNo, int numHand, Card card)
    {
        var position = Vector3.zero;
        var shift = handShift;
        var dir = handCardDir;
        var angle = Vector3.zero;
        angle.y = 180;

        const float width = 3.25f;
        position.x = width * handNo - (numHand * width / 2) + (width / 2);
        position.x += 1;
        position.z = -20.0f;

        // 逆サイド処理
        if (card.isMyPlayerSide == false)
        {
            // チートモード
            if(false)
            {
                card.SetUraomote(true);
                angle.x = 0;
                angle.y = 0;
                position.z = 14;
            }
            else
            {
                card.SetUraomote(false);
                angle.x = 180;
                position.z = -position.z;
            }


            position.x = -position.x;
            position.y = 1;
            shift.z = -shift.z;
            dir.z = -dir.z;


            position.z += shift.z;
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

    void UpdateCemeteryPosition()
    {
        for (int i = 0; i < cemeteryCards.Count; i++)
        {
            var card = cemeteryCards[i];
            
            //  墓地表向き防止

            // 表
            //card.SetUraomote(true);
            // 描画順
            //card.SetOrder(i);

            // 位置設定
            var position = cemeteryPosition;
            var angle = Vector3.zero;
            angle.y = 180;
            angle.z = 0;
            position.y = i * 0.25f;

            //card.handPosition = position;
            card.cacheTransform.localPosition = position;
           // card.cacheTransform.localEulerAngles = angle;
        }
    }

    public int GetNumCemetery() { return cemeteryCards.Count; }

    public void FieldSet(int handNo, bool isMyPlayer)
    {
        var type = handCards[handNo].cardData.cardType;
        var card = handCards[handNo];

        // 最下層にする
        card.SetOrder(0);

        var position = Vector3.zero;
        var angle = card.cacheTransform.localEulerAngles;
        angle.x = 0;

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
                    //fieldStrikerCard.nextPosition = position;
                    //fieldStrikerCard.cacheTransform.localEulerAngles = angle;

                    // 裏にする
                    angle.z = 180;
                    fieldStrikerCard.SetUraomote(false);

                    // セットのステートにさせる
                    fieldStrikerCard.FieldSet(position, angle);
                }
                break;

            case CardType.Support:
                //// フィールドにおいてるイベントのカード
                //fieldEventCard = card;
                // サポート発動モード移動設定
                card.SetSupport();
                break;
            case CardType.Intercept:
                // フィールドにおいてるイベントのカード
                fieldEventCard = card;
                // カードを指定の位置にセット
                position = eventField;
                // 逆サイド処理
                if (isMyPlayer == false)
                {
                    position.x = -position.x;
                    position.z = -position.z;
                    angle.z = 180;
                }

                // 表にする
                angle.z = 0;
                fieldEventCard.SetUraomote(true);

                // セットのステートにさせる
                fieldEventCard.FieldSet(position, angle);
                break;
        }

        // ★★★手札から消す
        handCards.Remove(card);
    }


    // インターセプト時にフィールドに置いた時
    public void FieldSetToIntercept(int handNo, bool isMyPlayer)
    {
        var type = handCards[handNo].cardData.cardType;
        var card = handCards[handNo];

        // 最下層にする
        card.SetOrder(0);

        var position = Vector3.zero;
        var angle = card.cacheTransform.localEulerAngles;
        angle.x = 0;

        switch (type)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:

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
                    //fieldStrikerCard.nextPosition = position;
                    //fieldStrikerCard.cacheTransform.localEulerAngles = angle;

                    // 裏にする
                    angle.z = 180;
                    fieldStrikerCard.SetUraomote(false);

                    // セットのステートにさせる
                    fieldStrikerCard.FieldSet(position, angle);
                }
                break;

            case CardType.Support:
                //// フィールドにおいてるイベントのカード
                //fieldEventCard = card;
                // サポート発動モード移動設定
                card.SetSupport();
                break;
            case CardType.Joker:
            case CardType.Intercept:
                // フィールドにおいてるイベントのカード
                fieldEventCard = card;
                // カードを指定の位置にセット
                position = eventField;
                // 逆サイド処理
                if (isMyPlayer == false)
                {
                    position.x = -position.x;
                    position.z = -position.z;
                    angle.z = 180;
                }

                // 表にする
                angle.z = 0;
                fieldEventCard.SetUraomote(true);

                // セットのステートにさせる
                fieldEventCard.FieldSet(position, angle);
                break;
        }

        // ★★★手札から消す
        handCards.Remove(card);
    }


    public void BackToHand(/*List<CardData> hand, DeckManager deckManager*/)
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
        UpdateHandPosition();
        //UpdateHand(hand, player);
    }

    public void ReFieldSetStriker(bool isMyPlayer)
    {
        if(fieldStrikerCard/*.gameObject.activeInHierarchy*/)
        {
            var position = Vector3.zero;
            var angle = fieldStrikerCard.cacheTransform.localEulerAngles;
            angle.x = 0;

            // 裏にする
            angle.z = 180;
            fieldStrikerCard.SetUraomote(false);

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
            //fieldStrikerCard.gameObject.SetActive(false);

            fieldStrikerCard.SetPower(fieldStrikerCard.cardData.power); // パワーの数値もとに戻す
            cemeteryCards.Add(fieldStrikerCard);
            fieldStrikerCard.MoveToCemetery(/*cemeteryCards.Count*/);
            fieldStrikerCard = null;
        }
        if(fieldEventCard)
        {
            //fieldEventCard.gameObject.SetActive(false);
            cemeteryCards.Add(fieldEventCard);
            fieldEventCard.MoveToCemetery(/*cemeteryCards.Count*/);
            fieldEventCard = null;
        }
        //fieldStrikerCard.gameObject.SetActive(false);
        //fieldEventCard.gameObject.SetActive(false);
    }

    public void AddHand(Card card)
    {
        // 手札に加える
        handCards.Add(card);
        // カードをドローの動きにする
        card.Draw(handCards.Count - 1, handCards.Count);
        // 手札位置更新
        UpdateHandPosition();
        
    }

    public void AddCemetery(Card card)
    {
        cemeteryCards.Add(card);
        card.MoveToCemetery(/*cemeteryCards.Count*/);
    }

    public void AddExpulsion(Card card)
    {
        expulsionCards.Enqueue(card);
        card.Expulsion();
    }

    public bool isSetEndStriker()
    {
        if(fieldStrikerCard != null)
        {
            return fieldStrikerCard.isSetField;
        }
        return false;
    }


    public void SetDeckData(CardData[] yamahuda, bool isMyPlayer)
    {
        // ★墓地の座標を保存
        if(isMyPlayer)
        {
            cemeteryPosition = GameObject.Find("myCemetery").transform.localPosition;
        }
        else
        {
            cemeteryPosition = GameObject.Find("cpuCemetery").transform.localPosition;
        }
        cemeteryPosition.y -= 0.01f;

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

            // アビリティから新規作成ではない
            card.isAbilityCreate = false;

            card.isMyPlayerSide = isMyPlayer;

            //card.SetCardData(yamahuda[i]);
        }

        // 山札位置更新
        UpdateYamahudaPosition();
    }

    public void Draw(CardData[] cards)
    {
        var orgNumHand = handCards.Count;

        foreach (CardData card in cards)
        {
            if (card == null) continue;

            Card draw;
            if (yamahudaCards.Count > 0)
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

            // カードの動きをドローモードにする
            draw.Draw(index, orgNumHand + cards.Length);

            // 手札位置更新
            UpdateHandPosition();
        }

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
            if (card.isInMovingState()) return true;
        }
        foreach(Card card in handCards)
        {
            // 何かしらの動いてるステートにいる
            if (card.isInMovingState()) return true;
        }
        if(fieldStrikerCard)
        {
            // 何かしらの動いてるステートにいる
            if (fieldStrikerCard.isInMovingState()) return true;
        }
        if(fieldEventCard)
        {
            // 何かしらの動いてるステートにいる
            if (fieldEventCard.isInMovingState()) return true;
        }

        // 誰も動いてるステートにいない
        return false;
    }

    // カードエフェクト　カード使用可能
    public void CheakActiveUseCard(bool CardHold = false)
    {

        if (CardHold == false)
        {
        
        // 手札かつ 使用可能なら
        foreach (Card card in handCards)
        {
            if (card.notSelectFlag == false)
            {
                card.ActiveUseCard();
            }else 
            {
                card.StopUseCard();

            }         
        }

        }else {

            // 手札かつ 使用可能なら
            foreach (Card card in handCards)
            {
                card.StopUseCard();
                
            }

        }
           
        // それ以外は全部光を消す
        foreach (Card card in yamahudaCards)
        {
            card.StopUseCard();
        }

        foreach (Card card in cemeteryCards)
        {
            card.StopUseCard();
        }
        
        if (fieldStrikerCard)
        {
            fieldStrikerCard.StopUseCard();
        }

        if (fieldEventCard)
        {
            fieldEventCard.StopUseCard();
        }
    }


    // 全部止める
    public void StopActiveUseCard()
    {

        // 手札かつ 使用可能なら
        foreach (Card card in handCards)
        {
            card.StopUseCard();
        }

        // それ以外は全部光を消す
        foreach (Card card in yamahudaCards)
        {
            card.StopUseCard();
        }

        foreach (Card card in cemeteryCards)
        {
            card.StopUseCard();
        }

        if (fieldStrikerCard)
        {
            fieldStrikerCard.StopUseCard();
        }

        if (fieldEventCard)
        {
            fieldEventCard.StopUseCard();
        }
    }

    // アビリティの生成系で使う
    public Card CreateCardObject(CardData data, bool isMyPlayer)
    {
        Card newCreateCard = Instantiate(cardPrefab, transform);
        newCreateCard.gameObject.SetActive(true);
        newCreateCard.isMyPlayerSide = isMyPlayer;
        // カードデータセット
        newCreateCard.SetCardData(data);
        // アビリティから作られたフラグoN
        newCreateCard.isAbilityCreate = true;
        return newCreateCard;
    }

    // 出せるストライカーカードがあるか
    public bool isHaveSetStrikerOKCard()
    {
        // 1枚でもストライカーカードを持っているならtrue
        foreach(Card card in handCards)
        {
            // (isStrikerはジョーカーがfalseになるのでこの書き方)
            if (card.cardData.isEventCard()) continue;

            // 選べるストライカーカードがある
            if (!card.notSelectFlag)
                return true;
        }
        return false;
    }

    //// カードエフェクト　止める
    //public void StopUseCard()
    //{
    //    foreach (Card card in handCards)
    //    {
    //        card.StopUseCard();
    //    }
    //}

}
