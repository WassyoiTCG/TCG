using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cost
{
    public enum PowerRange
    {
        Pinpoint,
        Ijou,
        Ika,
    }

    // 発動チェック関数を作りたかったので自前で作る
    public abstract class Base
    {
        protected int step;
        protected bool endFlag;

        public virtual void Enter(CardAbilityData abilityData) { step = 0; endFlag = false; }
        public abstract void Execute(CardAbilityData abilityData);
        public abstract bool OnMessage(CardAbilityData abilityData, MessageInfo message);
        // 発動条件を満たしているかどうか(主にイベントカードが出せるかどうかに使う)
        public abstract bool HatsudouCheck(CardAbilityData abilityData, Player player);
        public bool EndCheck() { return endFlag; }

        protected bool CheckRange(PowerRange range, int value0, int value1)
        {
            switch (range)
            {
                case PowerRange.Pinpoint:
                    Debug.Log(value0 + "==" + value1);
                    return (value0 == value1);

                case PowerRange.Ijou:
                    Debug.Log(value0 + ">=" + value1);
                    return (value0 >= value1);

                case PowerRange.Ika:
                    Debug.Log(value0 + "<=" + value1);
                    return (value0 <= value1);

                default:
                    Debug.LogWarning("パワーレンジの値おかしいクマ");
                    return false;
            }
        }
    }

    // 条件なし
    public class NoneLimit : Base
    {
        static NoneLimit instance;
        public static NoneLimit GetInstance() { if (instance == null) { instance = new NoneLimit(); } return instance; }

        int iWaitFrame = 0;
        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0;

            base.Enter(abilityData);

            // ゴリゴリのゴリ[12/16] 発動元のカードタイプが知りたかったとさ

            Card FieldCard = abilityData.myPlayer.cardObjectManager.fieldStrikerCard;
            Card EventCard = abilityData.myPlayer.cardObjectManager.fieldEventCard;
            if (FieldCard != null && EventCard == null) 
            {
                // Ability列車 勝利の雄たけびの成功時
                ActionEffectUVInfo info = new ActionEffectUVInfo();
                info.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
                info.fPosX = abilityData.GetFieldStrikerPosition().x;
                info.fPosY = abilityData.GetFieldStrikerPosition().y;
                info.fPosZ = abilityData.GetFieldStrikerPosition().z;

                MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, info);
            }

            if (EventCard != null)
            {
                // Ability列車 勝利の雄たけびの成功時
                ActionEffectUVInfo info = new ActionEffectUVInfo();
                info.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
                info.fPosX = abilityData.GetFieldEventPosition().x;
                info.fPosY = abilityData.GetFieldEventPosition().y;
                info.fPosZ = abilityData.GetFieldEventPosition().z;

                MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, info);

            }


            // 効果発動SE
            oulAudio.PlaySE("action_ability");
        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int iMaxFrame = 40;
            iWaitFrame++;
            if (iWaitFrame >= iMaxFrame)
            {

                // 即終了
                endFlag = true;
                // 条件OK
                abilityData.isJoukenOK = true;
            }
        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player) { return true; }
    }

    // 勝利の雄たけび
    public class Otakebi : Base
    {
        static Otakebi instance;
        public static Otakebi GetInstance() { if (instance == null) { instance = new Otakebi(); } return instance; }

        int iWaitFrame = 0;
         
        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0; // 初期化

            base.Enter(abilityData);

            var range = (DifferenceRangeType)abilityData.c_value0;  // 条件タイプ
            var difference = abilityData.c_value1;                  // 条件の差

            var myPower = abilityData.myPlayer.jissainoPower;
            var youPower = abilityData.youPlayer.jissainoPower;

            // ジョーカーvs10チェック(負けてるのに勝った扱いになるから)
            var myCard = abilityData.myPlayer.GetFieldStrikerCard();
            // 自分自身のパワーが10なら
            if (myCard.cardData.power == 10)
            {
                // 相手がジョーカーだったら効果は発動しない
                var youCard = abilityData.youPlayer.GetFieldStrikerCard();
                if (youCard)
                {
                    if (youCard.cardData.cardType == CardType.Joker)
                    {
                        abilityData.isJoukenOK = false;
                        return;
                    }
                }
            }

            bool ok = (myPower > youPower || youPower == Player.noSetStrikerPower);    // 勝ってたらtrue
            if (!ok)
            {
                abilityData.isJoukenOK = false;
                return;
            }

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    abilityData.isJoukenOK = true;
                    break;
                case DifferenceRangeType.In:
                    abilityData.isJoukenOK = (myPower - youPower <= difference);
                    break;
                case DifferenceRangeType.Out:
                    abilityData.isJoukenOK = (myPower - youPower >= difference);
                    break;
                default:
                    Debug.LogError("勝利の雄たけびのswitch文でエラークマ");
                    abilityData.isJoukenOK = false;
                    break;
            }

            // 条件満たしていないなら終了
            if (!abilityData.isJoukenOK)
            {
                endFlag = true;
                return;
            }

            // Ability列車 勝利の雄たけびの成功時
            ActionEffectUVInfo info = new ActionEffectUVInfo();
            info.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
            info.fPosX = abilityData.GetFieldStrikerPosition().x;
            info.fPosY = abilityData.GetFieldStrikerPosition().y;
            info.fPosZ = abilityData.GetFieldStrikerPosition().z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, info);

            // 効果発動SE
            oulAudio.PlaySE("action_ability");

            int a = 0;
            a++;

        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int iMaxFrame = 40;

            iWaitFrame++;
            // Ability列車 勝利の雄たけびの演出終了判定
            if (iWaitFrame >= iMaxFrame)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player) { return true; }
    }

    // 爪痕
    public class Tsumeato : Base
    {
        static Tsumeato instance;
        public static Tsumeato GetInstance() { if (instance == null) { instance = new Tsumeato(); } return instance; }

        int iWaitFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0;

            base.Enter(abilityData);

            var range = (DifferenceRangeType)abilityData.c_value0;  // 条件タイプ
            var difference = abilityData.c_value1;                  // 条件の差

            var myPower = abilityData.myPlayer.jissainoPower;
            var youPower = abilityData.youPlayer.jissainoPower;
            bool ok = (myPower < youPower);    // 負けてたらtrue
            if (!ok)
            {
                abilityData.isJoukenOK = false;
                return;
            }

            switch (range)
            {
                case DifferenceRangeType.NoneLimit:
                    abilityData.isJoukenOK = true;
                    break;
                case DifferenceRangeType.In:
                    abilityData.isJoukenOK = (youPower - myPower <= difference);
                    break;
                case DifferenceRangeType.Out:
                    abilityData.isJoukenOK = (youPower - myPower >= difference);
                    break;
                default:
                    Debug.LogError("爪痕のswitch文でエラークマ");
                    abilityData.isJoukenOK = false;
                    break;
            }


            // 条件満たしていないなら終了
            if (!abilityData.isJoukenOK)
            {
                endFlag = true;
                return;
            }

            // Ability列車 爪痕の成功時
            ActionEffectUVInfo info = new ActionEffectUVInfo();
            info.iEffectType = (int)UV_EFFECT_TYPE.SKILL_LOSE;
            info.fPosX = abilityData.GetFieldStrikerPosition().x;
            info.fPosY = abilityData.GetFieldStrikerPosition().y;
            info.fPosZ = abilityData.GetFieldStrikerPosition().z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, info);

            // 効果発動SE
            oulAudio.PlaySE("action_ability");

            int a = 0;
            a++;
        }

        public override void Execute(CardAbilityData abilityData)
        {

            const int iMaxFrame = 40;

            iWaitFrame++;
            // Ability列車 爪痕の演出終了判定
            if (iWaitFrame >= iMaxFrame)
            {
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player) { return true; }
    }

    //public class Sousai : BaseEntityState<CardAbilityData>
    //{
    //    public Sousai(CardAbilityData owner) : base(owner) { }

    //    public override bool CheckJouken()
    //    {
    //        int myPower = 0;
    //        int youPower = 0;
    //        return (myPower == youPower);
    //    }
    //}

    // バースト
    public class Burst : Base
    {
        static Burst instance;
        public static Burst GetInstance() { if (instance == null) { instance = new Burst(); } return instance; }

        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            step = 0;
            drawCard = null;

            // Ability列車 バースト発動した瞬間
        }

        public override void Execute(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            switch (step)
            {
                case 0:
                    // ドロー！
                    drawCard = abilityData.myPlayer.deckManager.DequeueYamahuda();
                    drawCard.ShowDraw(0.75f, true);

                    //// データ上で引いたカードを墓地に送る
                    //abilityData.myPlayer.deckManager.AddBochi(drawCard.cardData);

                    step++;
                    break;

                case 1:
                    // カードの動き終わったら
                    if (!drawCard.isInMovingState())
                    {
                        // 引いたカードを墓地に送る動き
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // モンスターカード！なら次のステート(isStrikerはジョーカーがfalseになるのでこの書き方)
                        if (!drawCard.cardData.isEventCard())
                        {
                            step++;
                        }
                        else
                        {
                            step--;
                        }
                    }
                    break;

                case 2:
                    if (drawCard != null)
                    {
                        // パワーを足す用の変数に格納
                        abilityData.delvValue0 = drawCard.cardData.power;
                    }
                    else Debug.LogWarning("バースト: 引いたカードがnullクマ");

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            // 山札が1以上のときに発動可能
            // 墓地にジョーカーがいたら発動OK
            return (
                player.deckManager.isExistBochiCardType(CardType.Joker) &&
                player.deckManager.GetNumYamahuda() > 0);
        }
    }

    // 宝箱
    public class Treasure : Base
    {
        static Treasure instance;
        public static Treasure GetInstance() { if (instance == null) { instance = new Treasure(); } return instance; }

        int selectNumber;   // プレイヤーの選択した番号
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;
            drawCard = null;

            // 数字選択UI表示
            CardAbilityData.uiManager.AppearSelectNumberUI(abilityData.myPlayer.deckManager, abilityData.isMyPlayer);
            // 相手がが選択中ですのUI
            if (!abilityData.isMyPlayer) CardAbilityData.uiManager.AppearSelectNumberWaitUI();

            // Ability列車 宝箱発動した瞬間
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    // 数字選択中
                    break;
                case 1:
                    // 数字UIの演出が終わるまでまつ
                    if (CardAbilityData.uiManager.isSelectNumberUIEnd())
                        step++;
                    break;
                case 2:

                    // ドロー！モンスターカード！が終わったら
                    // ドロー！
                    drawCard = abilityData.myPlayer.deckManager.DequeueYamahuda();
                    drawCard.ShowDraw(0.75f, true);

                    // データ上で引いたカードを墓地に送る
                    //abilityData.myPlayer.deckManager.AddBochi(drawCard.cardData);

                    step++;
                    break;

                case 3:
                    if (!drawCard.isInMovingState())
                    {
                        // モンスターカード！なら次のステート(isStrikerはジョーカーがfalseになるのでこの書き方)
                        if (!drawCard.cardData.isEventCard())
                        {
                            step++;
                        }
                        else
                        {
                            step--;

                            // 引いたカードを墓地に送る
                            abilityData.myPlayer.deckManager.AddBochi(drawCard);
                        }
                    }
                    break;

                case 4:
                    // プレイヤーの選択した数字と引いたストライカーの数字が同じなら
                    if (selectNumber == drawCard.cardData.power)
                    {
                        // デッキにある枚数分の得点を得る
                        abilityData.delvValue0 = abilityData.myPlayer.deckManager.GetNumYamahuda() * 10;

                        // デッキに加える
                        abilityData.myPlayer.deckManager.AddHand(drawCard);

                        // 成功時の効果に移行
                        abilityData.skillNumber = abilityData.c_value0;
                    }
                    // 残念
                    else
                    {
                        // 引いたカードを墓地に送る
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // 引いたカードを墓地に送る動き
                        //drawCard.MoveToCemetery();

                        // 失敗時の効果に移行
                        abilityData.skillNumber = abilityData.c_value1;
                    }

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectNumber:
                    {
                        // ★(2回選択しても入ってこれないようにする)
                        if (step > 0) return true;

                        SelectNumberInfo selectNumberInfo = new SelectNumberInfo();
                        message.GetExtraInfo<SelectNumberInfo>(ref selectNumberInfo);

                        // 選択した番号保存
                        selectNumber = selectNumberInfo.selectNumber;

                        // UI非表示
                        CardAbilityData.uiManager.DisAppearSelectNumberUI(selectNumber);
                        CardAbilityData.uiManager.DisAppearSelectNumberWaitUI();

                        // 次のステップへ
                        step++;
                    }
                    return true;
            }

            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            // 山札が1以上で、ストライカーカードが存在するときに発動可能
            for (int i = 0; i < player.deckManager.GetNumYamahuda(); i++)
            {
                // イベントじゃないカードがあるのでOK
                if (player.deckManager.GetYamahudaCard(i).isEventCard() == false) return true;
            }

            return false;
            //return (player.deckManager.GetNumYamahuda() > 0);
        }
    }

    // 起死回生の一手
    public class Kisikaisei : Base
    {
        static Kisikaisei instance;
        public static Kisikaisei GetInstance() { if (instance == null) { instance = new Kisikaisei(); } return instance; }

        //int selectNumber;   // プレイヤーの選択した番号

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            step = 0;

            // 数字選択UI表示
            //CardAbilityData.uiManager.DisAppearSelectNumberUI();
        }

        public override void Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    // 数字選択中
                    break;
                case 1:
                    if (true)
                    {
                        step++;
                    }
                    break;

                case 2:
                    // プレイヤーの選択した数字と相手のストライカーのパワーが同じなら
                    if (true)
                    {
                        // 成功時の効果に移行
                        abilityData.skillNumber = abilityData.c_value0;
                    }
                    // 残念
                    else
                    {
                        // 失敗時の効果に移行
                        //abilityData.skillNumber = abilityData.c_value1;
                    }

                    // コスト処理終了
                    endFlag = true;
                    abilityData.isJoukenOK = true;
                    break;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectNumber:
                    {
                        SelectNumberInfo selectNumberInfo = new SelectNumberInfo();
                        message.GetExtraInfo<SelectNumberInfo>(ref selectNumberInfo);

                        // 選択した番号保存
                        //selectNumber = selectNumberInfo.selectNumber;

                        // 次のステップへ
                        step++;
                    }
                    break;
            }

            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player) { return true; }
    }

    // 仲間と共に
    public class Nakama : Base
    {
        static Nakama instance;
        public static Nakama GetInstance() { if (instance == null) { instance = new Nakama(); } return instance; }

        float timer;

        Card drawCard;
        int iWaitFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0;

            base.Enter(abilityData);

            timer = 0;
            step = 0;
            drawCard = null;

            // 墓地からドローする番号を決定(無造作)
            if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
            {
                SelectCardIndexInfo info = new SelectCardIndexInfo();
                info.index = abilityData.myPlayer.deckManager.GetCemeteryRandomNumberStriker();
                if (info.index == (int)IDType.NONE)
                {
                    Debug.LogWarning("墓地にストライカーがいないのに引こうとしている。事前にその効果を発動しないようにしましょう。");

                    // 終了
                    endFlag = true;
                    return;
                }
                // メッセージ送信
                MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectCemetery, info);
            }

            // Ability列車 仲間と共に発動した瞬間
            ActionEffectUVInfo UVInfo = new ActionEffectUVInfo();
            UVInfo.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
            UVInfo.fPosX = abilityData.GetFieldEventPosition().x;
            UVInfo.fPosY = abilityData.GetFieldEventPosition().y;
            UVInfo.fPosZ = abilityData.GetFieldEventPosition().z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, UVInfo);

            // 効果発動SE
            oulAudio.PlaySE("action_ability");

            int a = 0;
            a++;
        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int iMAXFRAME = 40;
            iWaitFrame++;
            if (iWaitFrame>= iMAXFRAME)
            {

                switch (step)
                {
                    case 0:
                        // メッセージ待ち
                        break;
                    case 1:
                        // 例外処理
                        if (!drawCard)
                        {
                            Debug.LogWarning("墓地にストライカーがいないのに引こうとしている。事前にその効果を発動しないようにしましょう。");

                            // 終了
                            endFlag = true;

                            return;
                        }

                        // パワーを足す用の変数に格納
                        abilityData.delvValue0 = drawCard.cardData.power;

                        // 見せる用のドローの動きにする
                        drawCard.ShowDraw(0.75f, true);

                        step++;
                        break;
                    case 2:
                        // ドローの動きが終わったら
                        if (!drawCard.isInMovingState())
                        {
                            step++;
                        }
                        break;

                    case 3:
                        // 若干待ってみる
                        if ((timer += Time.deltaTime) > 1)
                        {
                            step++;
                        }
                        break;
                    case 4:

                        // 引いたカードを墓地に送る
                        abilityData.myPlayer.deckManager.AddBochi(drawCard);

                        // 引いたカードを墓地に送る動き
                        //drawCard.MoveToCemetery();

                        step++;
                        break;
                    case 5:
                        // ドローの動きが終わったら
                        if (!drawCard.isInMovingState())
                        {
                            // 終了
                            endFlag = true;
                            abilityData.isJoukenOK = true;
                        }
                        break;
                }

            }

        }


        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectCemetery:
                    {
                        SelectCardIndexInfo selectCardIndexInfo = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref selectCardIndexInfo);

                        // 墓地から無造作にストライカーを引く
                        drawCard = abilityData.myPlayer.deckManager.DequeCemetery(selectCardIndexInfo.index);

                        // 次のステップへ
                        step++;
                    }
                    break;
            }

            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            // 墓地にストライカー系がいないと発動できない
            if (player.deckManager.isExistBochiCardType(CardType.Fighter)) { return (player.jissainoPower == 1); }
            if (player.deckManager.isExistBochiCardType(CardType.AbilityFighter)) { return (player.jissainoPower == 1); }
            if (player.deckManager.isExistBochiCardType(CardType.Joker)) { return (player.jissainoPower == 1); }

            return false;

            // 実際のパワーが1の人だったら発動OK
            //return (abilityData.myPlayer.jissainoPower == 1);
        }
    }


    // 相手のストライカーのパワーがx
    public class YouStrikerPower : Base
    {
        static YouStrikerPower instance;
        public static YouStrikerPower GetInstance() { if (instance == null) { instance = new YouStrikerPower(); } return instance; }

        int iWaitFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0;

            base.Enter(abilityData);

            var striker = abilityData.youPlayer.GetFieldStrikerCard();
            if (!striker)
            {
                JoukenNG(abilityData);
                return;
            }

            var power = abilityData.c_value0;
            //var hantei = false;
            //switch ((PowerRange)abilityData.c_value1)
            //{
            //    case PowerRange.Pinpoint:
            //        hantei = (striker.cardData.power == power);
            //        break;

            //    case PowerRange.Ijou:
            //        hantei = (striker.cardData.power >= power);
            //        break;

            //    case PowerRange.Ika:
            //        hantei = (striker.cardData.power <= power);
            //        break;

            //    default:
            //        Debug.LogWarning("パワーレンジの値おかしいクマ");
            //        break;
            //}

            if (CheckRange((PowerRange)abilityData.c_value1, striker.cardData.power, power))
                JoukenOK(abilityData);
            else
                JoukenNG(abilityData);
        }

        void JoukenOK(CardAbilityData abilityData)
        {
            // 成功時の効果に移行
            abilityData.skillNumber = abilityData.c_value2;

            // Ability列車 パワー条件成功時

            // [1216] ゴリ　効果がモンスターかイベントか知りたい 
            // Ability列車 仲間と共に発動した瞬間
            ActionEffectUVInfo UVInfo = new ActionEffectUVInfo();
            UVInfo.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
            UVInfo.fPosX = abilityData.GetFieldStrikerPosition().x;
            UVInfo.fPosY = abilityData.GetFieldStrikerPosition().y;
            UVInfo.fPosZ = abilityData.GetFieldStrikerPosition().z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, UVInfo);

            // 効果発動SE
            oulAudio.PlaySE("action_ability");
        }
        void JoukenNG(CardAbilityData abilityData)
        {
            // 失敗時の効果に移行
            abilityData.skillNumber = abilityData.c_value3;

            // Ability列車 パワー条件失敗時
            // 終了
            endFlag = true;
            abilityData.isJoukenOK = true;
        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int MaxFrame = 40;
            iWaitFrame++;

            // Ability列車 パワー判定の演出終わったら
            if (iWaitFrame >= MaxFrame)
            {
                // 終了
                endFlag = true;
                abilityData.isJoukenOK = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            return true;
        }
    }

    // パワーxのストライカーが
    public class PowerStriker : Base
    {
        static PowerStriker instance;
        public static PowerStriker GetInstance() { if (instance == null) { instance = new PowerStriker(); } return instance; }

        Card suteCard;
        //int selectHandIndex;

        int iFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iFrame = 0;

            base.Enter(abilityData);

            suteCard = null;


            // [1216] ゴリ　効果がモンスターかイベントか知りたい 
            // Ability列車 仲間と共に発動した瞬間
            ActionEffectUVInfo UVInfo = new ActionEffectUVInfo();
            UVInfo.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
            UVInfo.fPosX = abilityData.GetFieldEventPosition().x;
            UVInfo.fPosY = abilityData.GetFieldEventPosition().y;
            UVInfo.fPosZ = abilityData.GetFieldEventPosition().z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, UVInfo);

            // 効果発動SE
            oulAudio.PlaySE("action_ability");


            // (TODO)手札からドローする番号を決定(無造作)
            if (abilityData.isMyPlayer || !SelectData.isNetworkBattle)
            {
                var deckManager = abilityData.myPlayer.deckManager;
                var numHand = deckManager.GetNumHand();
                var selectHandIndex = (int)IDType.NONE;
                for (int i = 0; i < numHand; i++)
                {
                    var card = deckManager.GetHandCard(i);
                    if (card.isEventCard()) continue;
                    // 条件満たしたら
                    if (CheckRange((PowerRange)abilityData.c_value1, card.power, abilityData.c_value0))
                    {
                        selectHandIndex = i;
                        break;
                    }
                }

                if(selectHandIndex == (int)IDType.NONE)
                {
                    Debug.LogWarning("炎の剣の捨て札で意図しない値");
                    // 終了
                    abilityData.isJoukenOK = true;
                    endFlag = true;
                }

                SelectCardIndexInfo info = new SelectCardIndexInfo();
                info.index = selectHandIndex;
                // メッセージ送信
                MessageManager.Dispatch(abilityData.myPlayer.playerID, MessageType.SelectHand, info);
                return;
            }
        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int iMaxFrame = 40;
            iFrame++;
            if (iFrame >= iMaxFrame)
            {
                switch (step)
                {
                    case 0: // カード選択中
                        break;
                    case 1: // 捨てカード移動中
                        if (!suteCard.isInMovingState())
                        {
                            // 墓地に送る
                            abilityData.myPlayer.deckManager.AddBochi(suteCard);
                            // 墓地に行く動き
                            //suteCard.MoveToCemetery();
                            step++;
                        }
                        break;
                    case 2: // 墓地に行くカードの動きが終わったら
                        if (!suteCard.isInMovingState())
                        {
                            // 終了
                            abilityData.isJoukenOK = true;
                            endFlag = true;
                        }
                        break;
                }

            }

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectHand:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        suteCard = abilityData.myPlayer.deckManager.DequeueHand(info.index);
                        suteCard.ShowDraw(0.75f, false);
                        step++;
                    }
                    return true;
            }
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            //Func<int, int, bool> check;
            //switch ((PowerRange)abilityData.c_value1)
            //{
            //    case PowerRange.Pinpoint:
            //        check = (a, b) => { return (a == b); };
            //        break;

            //    case PowerRange.Ijou:
            //        check = (a, b) => { return (a >= b); };
            //        break;

            //    case PowerRange.Ika:
            //        check = (a, b) => { return (a <= b); };
            //        break;

            //    default:
            //        Debug.LogWarning("おかしいおかしい");
            //        return false;
            //}

            var deckManager = player.deckManager;
            var numHand = deckManager.GetNumHand();
            for (int i = 0; i < numHand; i++)
            {
                var card = deckManager.GetHandCard(i);
                if (card.isEventCard()) continue;
                // 条件満たしたら
                if (CheckRange((PowerRange)abilityData.c_value1, card.power, abilityData.c_value0))
                {
                    //selectHandIndex = i;
                    return true;
                }
            }

            return false;
        }
    }

    // ターン条件
    public class Turn : Base
    {
        static Turn instance;
        public static Turn GetInstance() { if (instance == null) { instance = new Turn(); } return instance; }

        int iWaitFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iWaitFrame = 0;

            base.Enter(abilityData);

            // 現在のターンを取得
            int turn = GameObject.Find("GameMain").GetComponent<SceneMain>().turn;
            int joukenTurn = abilityData.c_value0;
            PowerRange range = (PowerRange)abilityData.c_value1;

            Debug.Log("ターン条件チェックするクマ");

            // 条件チェック
            bool isOK = CheckRange(range, turn, joukenTurn);

            if(isOK)
            {
                Debug.Log("条件満たしてる");

                // 成功時の効果に移行
                abilityData.skillNumber = abilityData.c_value2;

                // Ability列車 ターン条件効果発動(条件成功時)
                ActionEffectUVInfo info = new ActionEffectUVInfo();
                info.iEffectType = (int)UV_EFFECT_TYPE.SKILL_WIN;
                info.fPosX = abilityData.GetFieldStrikerPosition().x;
                info.fPosY = abilityData.GetFieldStrikerPosition().y;
                info.fPosZ = abilityData.GetFieldStrikerPosition().z;

                MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(abilityData.isMyPlayer), MessageType.ActionEffectUV, info);

                // 効果発動SE
                oulAudio.PlaySE("action_ability");
            }
            else
            {
                Debug.Log("条件満たしていない");

                // 失敗時の効果に移行
                abilityData.skillNumber = abilityData.c_value3;

                // Ability列車 ターン条件効果発動(条件満たしてない時)

            }

        }

        public override void Execute(CardAbilityData abilityData)
        {
            const int iMaxFrame = 50;
            iWaitFrame++;
            // Ability列車 ターン条件効果終了時
            if (iWaitFrame >= iMaxFrame)
            {
                // 終了
                abilityData.isJoukenOK = true;
                endFlag = true;
            }
        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }

        public override bool HatsudouCheck(CardAbilityData abilityData, Player player)
        {
            return true;
        }
    }
}