using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public enum Result
    {
        None,       // 特に問題なし
        EndGame,    // HPが0になったとかでゲーム終了
    }

    public abstract class Base
    {
        protected bool endFlag;
        protected int step;

        public virtual void Enter(CardAbilityData abilityData) { step = 0; endFlag = false; }
        public abstract Result Execute(CardAbilityData abilityData);
        public abstract void Exit(CardAbilityData abilityData);
        public abstract bool OnMessage(CardAbilityData abilityData, MessageInfo message);
        // 終了チェック
        public bool EndCheck() { return endFlag; }

        public void ActionUVEffect(Card card, UV_EFFECT_TYPE type, bool isMyPlayer)
        {
            // エフェクト発動！
            //+-------------------------------------------
            // 演出
            // Ability列車
            ActionEffectUVInfo info = new ActionEffectUVInfo();
            info.iEffectType = (int)type;
            Vector3 vPos = Vector3.zero;/* = targetPlayer.GetFieldStrikerCard().cacheTransform.localPosition*/;

            // もし相手のフィールドにストライカーが居なかったら
            if (card == null)
            {
                // 定数からとってくる
                vPos = CardObjectManager.strikerField;

                // 逆サイド
                if (!isMyPlayer) vPos.z = -vPos.z;
            }
            // フィールドにストライカーがいた時
            else
            {
                vPos = card.cacheTransform.localPosition;

                // 相手と味方でZ値変える
                if (isMyPlayer == true)
                {
                    vPos.z -= card.rootTransform.localPosition.z;
                }
                else
                {
                    vPos.z += card.rootTransform.localPosition.z;
                }
            }

            //const int AbjustY = 10;
            info.fPosX = vPos.x;
            info.fPosY = vPos.y + 1.0f;
            info.fPosZ = vPos.z;

            MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(isMyPlayer),
             MessageType.ActionEffectUV, info);
            // SE
            oulAudio.PlaySE("Heal");
        }
    }

    // 効果なし
    public class None : Base
    {
        static None instance;
        public static None GetInstance() { if (instance == null) { instance = new None(); } return instance; }

        public override void Enter(CardAbilityData abilityData) { base.Enter(abilityData); }

        public override Result Execute(CardAbilityData abilityData)
        {
            // 無条件終了
            endFlag = true;

            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }


    public static class ValueChange
    {
        public static int Enzan(Arithmetic arithmetic, int value0, int value1)
        {
            // 演算タイプに応じてラムダ関数登録
            switch (arithmetic)
            {
                case Arithmetic.Addition:
                    //enzankun = (a, b) => { return a + b; };
                    return value0 + value1;
                case Arithmetic.Subtraction:
                    //enzankun = (a, b) => { return Mathf.Max(a - b, 0); };
                    return value0 - value1;
                case Arithmetic.Multiplication:
                    //enzankun = (a, b) => { return a * b; };
                    return value0 * value1;
                case Arithmetic.Division:
                    //enzankun = (a, b) => { return (b != 0) ? a / b : 0; };
                    return (value1 != 0) ? value0 / value1 : 0;
                default:
                    Debug.LogWarning("演算enumでエラークマ");
                    return 0;
            }
        }

        public static int GetCheckRefValue(CardAbilityData abilityData, int value)
        {
            // ★参照かどうかチェック
            var bit = (value ^ CardAbilityData.RefValueFlag);
            if (bit < CardAbilityData.RefValueFlag)
            {
                switch (bit)
                {
                    case 0:
                        value = abilityData.delvValue0;
                        break;

                    case 1:
                        value = abilityData.delvValue1;
                        break;

                    case 2:
                        value = abilityData.delvValue2;
                        break;

                    case 3:
                        value = abilityData.delvValue3;
                        break;

                    case 4:
                        // 相手のパワー×10
                        value = abilityData.youPlayer.jissainoPower * 10;
                        break;

                    default:
                        Debug.LogWarning("参照関係のビット演算でエラークマ");
                        break;
                }
            }

            return value;
        }
    }

    // ライフ系
    public class Score : Base
    {
        static Score instance;
        public static Score GetInstance() { if (instance == null) { instance = new Score(); } return instance; }

        int iFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iFrame = 0;
            base.Enter(abilityData);
        }

        bool Enzan(Player player, UIManager uiManager, Arithmetic arithmetic, int value)
        {
            Card strikerCard = player.cardObjectManager.fieldStrikerCard;
            // 現在のスコア
            var score = uiManager.GetScore(player.isMyPlayer);
            var isEnd = false;

            if (strikerCard != null)
            {
                // Ability列車 ライフ変化
                Vector3 strikerPosition = strikerCard.cacheTransform.localPosition;
                switch (arithmetic)
                {
                    case Arithmetic.Addition:
                        // 足し算なので回復
                        uiManager.Heal(player.isMyPlayer, value);

                        //+-------------------------------------------
                        // 演出
                        // Ability列車
                        ActionEffectPanelInfo info = new ActionEffectPanelInfo();
                        info.iEffectType = (int)PANEL_EFFECT_TYPE.STAR;
                        Vector3 vPos = player.GetFieldStrikerCard().cacheTransform.localPosition;
                        // 相手と味方でZ値変える
                        if (player.isMyPlayer == true)
                        {
                            vPos.z -= strikerCard.rootTransform.localPosition.z;
                        }
                        else
                        {
                            vPos.z += strikerCard.rootTransform.localPosition.z;
                        }
                        
                        info.fPosX = vPos.x;
                        info.fPosY = vPos.y;
                        info.fPosZ = vPos.z;

                        MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(player.isMyPlayer),
                         MessageType.ActionEffectPanel, info);
                        // SE
                        oulAudio.PlaySE("Heal");


                        break;

                    case Arithmetic.Subtraction:
                        // 引き算なのでダメージ
                        isEnd = uiManager.Damage(player.isMyPlayer, value);

                        //+-------------------------------------------
                        // 演出
                        // Ability列車
                        ActionEffectPanelInfo info2 = new ActionEffectPanelInfo();
                        info2.iEffectType = (int)PANEL_EFFECT_TYPE.DAMAGE;
                        Vector3 vPos2 = player.GetFieldStrikerCard().cacheTransform.localPosition;
                        // 相手と味方でZ値変える
                        if (player.isMyPlayer == true)
                        {
                            vPos2.z -= strikerCard.rootTransform.localPosition.z;
                        }
                        else
                        {
                            vPos2.z += strikerCard.rootTransform.localPosition.z;
                        }

                        info2.fPosX = vPos2.x;
                        info2.fPosY = vPos2.y;
                        info2.fPosZ = vPos2.z;

                        MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(player.isMyPlayer),
                         MessageType.ActionEffectPanel, info2);
                        // SE
                        oulAudio.PlaySE("Attack_Middle");

                        // ブラ―&カメラ
                        Camera.main.GetComponent<RadialBlur>().SetBlur(0, 0, 5);
                        Camera.main.GetComponent<ShakeCamera>().Set();

                        break;

                    case Arithmetic.Multiplication:
                        // 掛け算(あまり使わない)
                        var mult = score * value;
                        if (mult > score) uiManager.Heal(player.isMyPlayer, mult - score);
                        else if (mult < score) isEnd = uiManager.Damage(player.isMyPlayer, score - mult);
                        break;

                    case Arithmetic.Division:
                        // 割り算(あまり使わない)
                        var div = score / value;
                        if (div > score) uiManager.Heal(player.isMyPlayer, div - score);
                        else if (div < score) isEnd = uiManager.Damage(player.isMyPlayer, score - div);
                        break;
                }
            }


            //// 演算して
            //var newScore = ValueChange.Enzan(arithmetic, score, value);
            //// セット
            //uiManager.SetHP(player.isMyPlayer, newScore);

            //var isEnd = (newScore <= 0 && score > 0);
            return isEnd;
        }

        public override Result Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                case 0:
                    {
                        var skillData = abilityData.GetCurrentSkillData();

                        // ※s_iValue0=演算タイプ, s_iValue1=演算する値, s_iValue2=効果の対象
                        var arithmetic = (Arithmetic)skillData.s_iValue0;
                        var value = ValueChange.GetCheckRefValue(abilityData, skillData.s_iValue1);
                        var abilityTarget = (AbilityTarget)skillData.s_iValue2;

                        switch (abilityTarget)
                        {
                            case AbilityTarget.Me:
                                if (Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            case AbilityTarget.You:
                                if (Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            case AbilityTarget.Select:
                                break;

                            case AbilityTarget.All:
                                if (Enzan(abilityData.myPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                if (Enzan(abilityData.youPlayer, CardAbilityData.uiManager, arithmetic, value)) return Result.EndGame;
                                break;

                            default:
                                Debug.LogWarning("スコア変更で意図しない値:" + (int)abilityTarget);
                                break;
                        }

                        step++;
                    }
                    break;

                case 1:
                    const int iMaxFrame = 90;
                    iFrame++;
                    // Ability列車 ライフ変化の演出が終わったら
                    if (iFrame >= iMaxFrame)
                    {
                        endFlag = true;
                    }
                    break;
            }

            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }

    //+-----------------------------------------------------------------------
    // パワー変化系
    public class Power : Base
    {
        static Power instance;
        public static Power GetInstance() { if (instance == null) { instance = new Power(); } return instance; }

        int iFrame = 0;

        public override void Enter(CardAbilityData abilityData)
        {
            iFrame = 0;

            base.Enter(abilityData);

            var skillData = abilityData.GetCurrentSkillData();

            // ※s_iValue0=演算タイプ, s_iValue1=演算する値, s_iValue2=効果の対象
            var arithmetic = (Arithmetic)skillData.s_iValue0;
            var value = ValueChange.GetCheckRefValue(abilityData, skillData.s_iValue1);
            var abilityTarget = (AbilityTarget)skillData.s_iValue2;

            switch (abilityTarget)
            {
                case AbilityTarget.Me:
                    Enzan(abilityData.myPlayer, arithmetic, value);
                    break;

                case AbilityTarget.You:
                    Enzan(abilityData.youPlayer, arithmetic, value);
                    break;

                case AbilityTarget.Select:
                    break;

                case AbilityTarget.All:
                    Enzan(abilityData.myPlayer, arithmetic, value);
                    Enzan(abilityData.youPlayer, arithmetic, value);
                    break;

                default:
                    Debug.LogWarning("パワー変更で意図しない値:" + (int)abilityTarget);
                    break;
            }
        }

        void Enzan(Player player, Arithmetic arithmetic, int value)
        {
            Card strikerCard = player.cardObjectManager.fieldStrikerCard;
            if (strikerCard != null)
            {
                // Ability列車 パワー変化
                Vector3 strikerPosition = strikerCard.cacheTransform.localPosition;
                switch (arithmetic)
                {
                    case Arithmetic.Addition:
                        //// パワー加算エフェクト
                        ////+-------------------------------------------
                        base.ActionUVEffect(player.GetFieldStrikerCard(), UV_EFFECT_TYPE.UP_STATUS, player.isMyPlayer);
                        //// 演出
                        //// Ability列車
                        //ActionEffectUVInfo info = new ActionEffectUVInfo();
                        //info.iEffectType = (int)UV_EFFECT_TYPE.UP_STATUS;
                        //Vector3 vPos = player.GetFieldStrikerCard().cacheTransform.localPosition;
                        //// 相手と味方でZ値変える
                        //if (player.isMyPlayer == true)
                        //{
                        //    vPos.z -= strikerCard.rootTransform.localPosition.z;
                        //}
                        //else
                        //{
                        //    vPos.z += strikerCard.rootTransform.localPosition.z;
                        //}

                        ////const int AbjustY = 10;
                        //info.fPosX = vPos.x;
                        //info.fPosY = vPos.y;
                        //info.fPosZ = vPos.z;

                        //MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(player.isMyPlayer),
                        // MessageType.ActionEffectUV, info);
                        //// SE
                        //oulAudio.PlaySE("Heal");

                        break;

                    case Arithmetic.Subtraction:
                        //// パワー減算エフェクト
                        ////+-------------------------------------------
                        base.ActionUVEffect(player.GetFieldStrikerCard(), UV_EFFECT_TYPE.DOWN_STATUS, player.isMyPlayer);
                        //// 演出
                        //// Ability列車
                        //ActionEffectUVInfo info2 = new ActionEffectUVInfo();
                        //info2.iEffectType = (int)UV_EFFECT_TYPE.DOWN_STATUS;
                        //Vector3 vPos2 = player.GetFieldStrikerCard().cacheTransform.localPosition;
                        //// 相手と味方でZ値変える
                        //if (player.isMyPlayer == true)
                        //{
                        //    vPos2.z -= strikerCard.rootTransform.localPosition.z;
                        //}
                        //else
                        //{
                        //    vPos2.z += strikerCard.rootTransform.localPosition.z;
                        //}


                        //info2.fPosX = vPos2.x;
                        //info2.fPosY = vPos2.y;
                        //info2.fPosZ = vPos2.z;

                        //MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(player.isMyPlayer),
                        // MessageType.ActionEffectUV, info2);
                        //// SE
                        //oulAudio.PlaySE("Heal");

                        break;

                    case Arithmetic.Multiplication:
                        //// 掛け算(パワー0にするだったらvalueが0になってる)
                        ////+-------------------------------------------
                        base.ActionUVEffect(player.GetFieldStrikerCard(), UV_EFFECT_TYPE.UP_STATUS, player.isMyPlayer);
                        //// 演出
                        //// Ability列車
                        //ActionEffectUVInfo info3 = new ActionEffectUVInfo();
                        //info3.iEffectType = (int)UV_EFFECT_TYPE.UP_STATUS;
                        //Vector3 vPos3 = player.GetFieldStrikerCard().cacheTransform.localPosition;
                        //// 相手と味方でZ値変える
                        //if (player.isMyPlayer == true)
                        //{
                        //    vPos3.z -= strikerCard.rootTransform.localPosition.z;
                        //}
                        //else
                        //{
                        //    vPos3.z += strikerCard.rootTransform.localPosition.z;
                        //}

                        ////const int AbjustY = 10;
                        //info3.fPosX = vPos3.x;
                        //info3.fPosY = vPos3.y;
                        //info3.fPosZ = vPos3.z;

                        //MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(player.isMyPlayer),
                        // MessageType.ActionEffectUV, info3);
                        //// SE
                        //oulAudio.PlaySE("Heal");


                        break;

                    case Arithmetic.Division:
                        // 割り算(あまり使わない)
                        break;
                }
            }

            // 現在のパワーを取ってきて
            var power = player.jissainoPower;
            // 演算する
            player.SetPower(ValueChange.Enzan(arithmetic, power, value));
        }

        public override Result Execute(CardAbilityData abilityData)
        {
            const int MAXFrame = 40;
            iFrame++;

            // Ability列車 パワー変化の演出が終わったら
            if (iFrame >= MAXFrame)
            {
                endFlag = true;
            }
            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }

    // カード移動系系
    public class CardMove : Base
    {
        static CardMove instance;
        public static CardMove GetInstance() { if (instance == null) { instance = new CardMove(); } return instance; }

        public enum From
        {
            Hand,           // 手札から
            Cemetery,       // 墓地から
            Deck,           // 山札から
            CemeteryOrDeck, // 墓地または山札から
            NewCreate,      // 新たに生成
        }
        //From fromType;
        Player fromPlayer;

        public enum To
        {
            Hand,       // 手札に
            Cemetery,   // 墓地に
            Deck,       // 山札に
            Tsuihou,    // 追放
        }
        To toType;
        Player toPlayer;

        public enum SearchType
        {
            None,   // 無造作
            Saati,  // サーチ
        }
        //SearchType searchType;

        public enum SelectType
        {
            PlayerSelect,   // プレイヤー指定
            AutoRandom,     // 無造作(デッキならトップ)
            Highest,        // 最も高い
            Lowest          // 最も低い
        }

        public enum SearchMask
        {
            NoneLimit,          // 指定なし
            AllStriker,         // ストライカー全般
            NoAbilityStriker,   // 効果なしストライカー
            AbilityStriker,     // 効果ありストライカー
            Joker,              // ジョーカー
            Support,            // サポート
            Connect,            // コネクト
            Intercept,          // インターセプト
        }

        //public enum Siborikomi
        //{
        //    Power,      // パワー
        //    Syuzoku,    // 種族
        //    Name,       // ボーイと名のつくなど
        //}

        public struct SearchInfo
        {
            public SelectType selectType;
            public SearchMask searchMask;
            //public Siborikomi siborikomi;
            public string sPower;
            public string sSyuzoku;
            public string sName;
        }

        //float timer;
        SelectCardIndexInfo selectCardIndexInfo;
        MessageType messageType = MessageType.NoMessage;
        Card drawCard;

        public override void Enter(CardAbilityData abilityData)
        {
            base.Enter(abilityData);

            //timer = 0;
            //
            selectCardIndexInfo = new SelectCardIndexInfo();
            selectCardIndexInfo.index = (int)IDType.NONE;
            messageType = MessageType.NoMessage;

            Debug.Log("カードサーチ発動するクマ");

            var skillData = abilityData.GetCurrentSkillData();

            drawCard = null;

            // ※value0=どこから, value1=誰の, value2=どこに, value3=誰の
            var fromPlace = (From)skillData.s_iValue0;
            var fromTarget = (AbilityTarget)skillData.s_iValue1;
            var searchType = (SearchType)skillData.s_iValue2;
            toType = (To)skillData.s_iValue3;
            var toTarget = (AbilityTarget)skillData.s_iValue4;
            toPlayer = abilityData.GetPlayerByAbilitiTarget(toTarget);
            var searchInfo = new SearchInfo();
            searchInfo.selectType = (SelectType)skillData.s_iValue5;
            searchInfo.searchMask = (SearchMask)skillData.s_iValue6;
            //searchInfo.siborikomi = (Siborikomi)skillData.s_iValue7;
            //searchInfo.siborikomiValue = skillData.s_sValue0;
            searchInfo.sPower = skillData.s_sValue0;
            searchInfo.sSyuzoku = skillData.s_sValue1;
            searchInfo.sName = skillData.s_sValue2;

            // もふりとか、生成系の処理
            if (fromPlace == From.NewCreate)
            {
                CardData createCardData = CardDataBase.GetCreateOnlyCardData(skillData.s_iValue2);
                // 生成
                drawCard = toPlayer.cardObjectManager.CreateCardObject(createCardData, toPlayer.isMyPlayer);
                drawCard.ShowDraw(0.75f, false);
            }
            else
            {
                // Fromの処理
                /*var */
                fromPlayer = abilityData.GetPlayerByAbilitiTarget(fromTarget);

                Debug.Log("from処理するクマ");

                int searchIndex = (int)IDType.NONE;

                switch (fromPlace)
                {
                    case From.Hand:
                        messageType = MessageType.SelectHand;

                        // 手札から無造作に
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("手札から無造作クマ");

                            int numHand = fromPlayer.deckManager.GetNumHand();
                            if (numHand <= 0)
                            {
                                Debug.Log("手札ないクマ");
                                endFlag = true;
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                int r = UnityEngine.Random.Range(0, numHand - 1);
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = r;
                                //info.index = r;

                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectHand, info);
                            }
                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("手札からサーチクマ");

                            searchIndex = SearchHand(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = searchIndex;
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectHand, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかったので終了
                        }
                        break;
                    case From.Cemetery:
                        messageType = MessageType.SelectCemetery;

                        // 墓地から無造作に
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("墓地から無造作クマ");

                            int numCemetery = fromPlayer.deckManager.GetNumCemetery();
                            if (numCemetery <= 0)
                            {
                                Debug.Log("墓地ないクマ");
                                endFlag = true;
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                int r = UnityEngine.Random.Range(0, numCemetery - 1);
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = r;
                                //info.index = r;

                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                            }
                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("墓地からサーチクマ");

                            searchIndex = SearchCemetery(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                selectCardIndexInfo.index = searchIndex;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;  // 見つからなかったので終了
                        }
                        break;
                    case From.Deck:
                        messageType = MessageType.SelectYamahuda;

                        // 山札のトップ
                        if (searchType == SearchType.None)
                        {
                            Debug.Log("デッキのトップから引くクマ");

                            int numYamahuda = fromPlayer.deckManager.GetNumYamahuda();
                            if (numYamahuda != 0)
                            {
                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                selectCardIndexInfo.index = numYamahuda - 1;
                                //info.index = fromPlayer.deckManager.GetNumYamahuda() - 1;

                                if (toType == To.Hand && fromPlayer.playerID == toPlayer.playerID)
                                {
                                    //drawCard = fromPlayer.deckManager.DequeueYamahuda();
                                    //step = 2;
                                    selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.Draw;
                                    //info.iMoveFlag = (int)CardMoveFlag.Draw;
                                }
                                else
                                {
                                    selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                                    //info.iMoveFlag = (int)CardMoveFlag.Draw;
                                }
                                // メッセージ送信
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else
                            {
                                Debug.Log("山札がないのでドローしないクマ");
                                endFlag = true;
                            }

                        }
                        else if (searchType == SearchType.Saati)
                        {
                            Debug.Log("デッキからサーチするクマ");

                            searchIndex = SearchYamahuda(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                selectCardIndexInfo.index = searchIndex;
                                selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //info.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかったので終了
                            
                            
                        }
                        break;
                    case From.CemeteryOrDeck:
                        if (searchType != SearchType.Saati)
                        {
                            Debug.LogWarning("墓地または山札なのに無造作になっている");
                            endFlag = true;
                            // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                            //return;
                        }

                        Debug.Log("デッキか墓地からサーチするクマ");

                        // 山札カードから検索をかける
                        searchIndex = SearchYamahuda(searchInfo);
                        // 見つかった
                        if (searchIndex != (int)IDType.NONE)
                        {
                            // もういやだもうやりたくない体を壊すのはもう嫌だ
                            messageType = MessageType.SelectYamahuda;
                            selectCardIndexInfo.index = searchIndex;
                            selectCardIndexInfo.iMoveFlag = (int)CardMoveFlag.ShowDraw;

                            //SelectCardIndexInfo info = new SelectCardIndexInfo();
                            //info.index = searchIndex;
                            //info.iMoveFlag = (int)CardMoveFlag.ShowDraw;
                            //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, info);
                            // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                            //return;
                        }
                        else
                        {

                            // 墓地カードから検索を掛ける
                            searchIndex = SearchCemetery(searchInfo);

                            // 見つかった
                            if (searchIndex != (int)IDType.NONE)
                            {
                                messageType = MessageType.SelectCemetery;
                                selectCardIndexInfo.index = searchIndex;

                                //SelectCardIndexInfo info = new SelectCardIndexInfo();
                                //info.index = searchIndex;
                                //MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectCemetery, info);
                                // リターンは死んでどうぞなそぢｊむｄｆしなにはｗじぇちとどｗくだ
                                //return;
                            }
                            else endFlag = true;// 見つからなかった
                        }

                        break;
                    default:
                        Debug.LogError("FromTypeで想定されない値");
                        endFlag = true;
                        break;
                }

                // ネットプレーかつ自分じゃなかったら送らなくていい
                if (!abilityData.isMyPlayer && SelectData.isNetworkBattle)
                {
                    return;
                }

                // リターンコード書きまくってるので無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理無理
                // でもだいじょうぶ
                if (selectCardIndexInfo.index != (int)IDType.NONE)
                {
                    MessageManager.Dispatch(fromPlayer.playerID, messageType, selectCardIndexInfo);
                }
            }

        }

        public override Result Execute(CardAbilityData abilityData)
        {
            switch (step)
            {
                //case 0:
                //    step++;
                //    // メッセージおくる
                //    //if (selectCardIndexInfo.index != (int)IDType.NONE)
                //    //{
                //    //    // ゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリ
                //    //    if ((timer += Time.deltaTime) > 10.0f)
                //    //    {
                //    //        MessageManager.Dispatch(fromPlayer.playerID, MessageType.SelectYamahuda, selectCardIndexInfo);
                //    //        step++;
                //    //    }
                //    //}
                //    //else step++;
                //    break;
                case 0:// メッセージ待ち
                    if (drawCard != null) step++;
                    break;
                case 1:
                    // カードの動きが終わるまで待つ
                    if (!drawCard.isInMovingState())
                    {
                        step++;
                    }
                    break;

                case 2:
                    // どこに向かうか
                    {
                        //var skillData = abilityData.GetCurrentSkillData();

                        switch (toType)
                        {
                            case To.Hand:
                                toPlayer.deckManager.AddHand(drawCard);
                                break;
                            case To.Cemetery:
                                // データ上でカードを墓地に送る
                                toPlayer.deckManager.AddBochi(drawCard);
                                // カードを墓地に送る動き
                                //drawCard.MoveToCemetery();
                                break;
                            case To.Deck:
                                // (TODO)実装まだ
                                break;
                            case To.Tsuihou:
                                toPlayer.deckManager.AddExpulsion(drawCard);
                                break;
                            default:
                                Debug.LogError("ToTypeで想定されない値");
                                break;
                        }
                    }
                    step++;
                    break;

                case 3:
                    // カードの動きが終わるまで待つ
                    if (!drawCard.isInMovingState())
                    {
                        endFlag = true;
                    }
                    break;
            }
            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            switch (message.messageType)
            {
                case MessageType.SelectYamahuda:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeueYamahuda(index);
                        if (info.iMoveFlag == (int)CardMoveFlag.ShowDraw)
                            drawCard.ShowDraw(0.75f, false);
                        //step++;
                    }
                    break;
                case MessageType.SelectHand:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeueHand(index);
                        drawCard.ShowDraw(0.75f, false);
                        //step++;
                    }
                    break;

                case MessageType.SelectCemetery:
                    {
                        SelectCardIndexInfo info = new SelectCardIndexInfo();
                        message.GetExtraInfo<SelectCardIndexInfo>(ref info);
                        var index = info.index;
                        Debug.Assert(index != (int)IDType.NONE, "カードサーチのIDが不正");
                        drawCard = fromPlayer.deckManager.DequeCemetery(index);
                        drawCard.ShowDraw(0.75f, false);
                        //step++;
                    }
                    break;
            }
            return false;
        }

        int SearchHand(SearchInfo info)
        {
            var handCards = fromPlayer.deckManager.GetHand();
            return Search(handCards, fromPlayer, info);
        }

        int SearchYamahuda(SearchInfo info)
        {
            var yamahudaCards = fromPlayer.deckManager.GetYamahuda();
            return Search(yamahudaCards, fromPlayer, info);
        }

        int SearchCemetery(SearchInfo info)
        {
            var cemeteryCards = fromPlayer.deckManager.GetCemeteryCards();
            return Search(cemeteryCards, fromPlayer, info);
        }

        int Search(List<CardData> cards, Player player, SearchInfo searchInfo)
        {
            // カードタイプに一致するカードを格納
            Debug.Log("カードタイプ検索するクマ");
            var buf = new List<int>();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                switch (searchInfo.searchMask)
                {
                    case SearchMask.NoneLimit:
                        buf.Add(i);
                        break;
                    case SearchMask.AllStriker:
                        if (card.isStrikerCard())
                            buf.Add(i);
                        break;
                    case SearchMask.NoAbilityStriker:
                        if (card.cardType == CardType.Fighter)
                            buf.Add(i);
                        break;
                    case SearchMask.AbilityStriker:
                        if (card.cardType == CardType.AbilityFighter)
                            buf.Add(i);
                        break;
                    case SearchMask.Joker:
                        if (card.cardType == CardType.Joker)
                            buf.Add(i);
                        break;
                    case SearchMask.Support:
                        if (card.cardType == CardType.Support)
                            buf.Add(i);
                        break;
                    case SearchMask.Connect:
                        if (card.cardType == CardType.Connect)
                            buf.Add(i);
                        break;
                    case SearchMask.Intercept:
                        if (card.cardType == CardType.Intercept)
                            buf.Add(i);
                        break;
                    default:
                        break;
                }
            }
            // 何もヒットしなかったら終了
            if (buf.Count <= 0)
            {
                Debug.Log("ひっとしなかったクマ…");
                return (int)IDType.NONE;
            }

            Debug.Log(buf.Count + "件ヒットしたクマ");

            var searchIndex = (int)IDType.NONE;

            // 選択指定
            switch (searchInfo.selectType)
            {
                case SelectType.PlayerSelect:
                    searchIndex = PlayerSelectSearch(buf, cards, searchInfo);
                    break;

                case SelectType.AutoRandom:
                    var indices = AutoRandomSearch(buf, cards, searchInfo);
                    if (indices.Length > 0) searchIndex = indices[0];
                    break;

                case SelectType.Highest:
                    searchIndex = HighestSearch(buf, cards, searchInfo);
                    break;

                case SelectType.Lowest:
                    searchIndex = LowestSearch(buf, cards, searchInfo);
                    break;

                default:
                    Debug.LogWarning("SelectTypeおかしいクマ");
                    break;
            }

            buf.Clear();
            return searchIndex;
        }

        public int PlayerSelectSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.LogWarning("PlayerSelectSearchは未実装");
            return 0;
        }
        public int[] AutoRandomSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            List<int> searchIndices = new List<int>();
            searchIndices.Clear();

            foreach (int i in buf)
            {
                var searchOK = true;
                var card = cards[i];

                if (searchInfo.sPower != "")
                {
                    // パワー一致
                    var power = int.Parse(searchInfo.sPower);
                    Debug.Log("パワー一致するかみてみるクマ ");
                    if (card.power != power)
                    {
                        searchOK = false;
                        Debug.Log("してないグマ");
                    }
                    else Debug.Log("しテルクマ");
                }
                if (searchInfo.sSyuzoku != "")
                {
                    // 種族一致
                    var strikerCard = card.GetFighterCard();
                    if (strikerCard != null)
                    {
                        var syuzoku = (Syuzoku)int.Parse(searchInfo.sSyuzoku);
                        Debug.Log("種族一致するかみてみるクマ ");

                        var find = false;
                        foreach (Syuzoku s in strikerCard.syuzokus)
                        {
                            if (s == syuzoku)
                            {
                                find = true;
                                Debug.Log("しテルクマ");
                                break;
                            }
                        }

                        if (!find)
                        {
                            searchOK = false;
                            Debug.Log("してないグマ");
                        }
                    }
                }
                if (searchInfo.sName != "")
                {
                    Debug.Log("名前が含んでるかみてみるクマ ");

                    // 文字列含んでいるかどうか
                    if (!card.cardName.Contains(searchInfo.sName))
                    {
                        searchOK = false;
                        Debug.Log("含んでないクマ ");
                    }
                    else Debug.Log("含んでるクマ ");
                }
                if (searchOK)
                {
                    searchIndices.Add(i);
                    //break;
                }
            }
            return searchIndices.ToArray();
        }

        public int HighestSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.Log("パワーもっとも高いやつをサーチするクマ");

            var indices = AutoRandomSearch(buf, cards, searchInfo);

            var max = int.MinValue;
            var searchIndex = (int)IDType.NONE;

            foreach (int i in indices)
            {
                var card = cards[i];
                if (card.power > max)
                {
                    max = card.power;
                    searchIndex = i;
                }
            }

            return searchIndex;
        }

        public int LowestSearch(List<int> buf, List<CardData> cards, SearchInfo searchInfo)
        {
            Debug.Log("パワーもっとも低いやつをサーチするクマ");

            var indices = AutoRandomSearch(buf, cards, searchInfo);

            var min = int.MaxValue;
            var searchIndex = (int)IDType.NONE;

            foreach (int i in indices)
            {
                var card = cards[i];
                if (card.power < min)
                {
                    min = card.power;
                    searchIndex = i;
                }
            }

            return searchIndex;
        }
    }

    //+-----------------------------------------------------------------------
    // 次のターンのパワー制限
    public class LimitPower : Base
    {
        static LimitPower instance;
        public static LimitPower GetInstance() { if (instance == null) { instance = new LimitPower(); } return instance; }

        int iFrame;

        public override void Enter(CardAbilityData abilityData)
        {
            iFrame = 0;

            var skillData = abilityData.GetCurrentSkillData();

            Player.LimitPowerData limitPowerData = new Player.LimitPowerData();

            limitPowerData.type = (Player.LimitPowerType)skillData.s_iValue0;
            limitPowerData.value = skillData.s_iValue1;
            AbilityTarget target = (AbilityTarget)skillData.s_iValue2;
            Player targetPlayer = abilityData.GetPlayerByAbilitiTarget(target);
            // 対象プレイヤーに制限セット
            targetPlayer.SetLimitPowerData(limitPowerData);

            // バツを表示
            targetPlayer.cardObjectManager.SetLimitPower(limitPowerData.value, limitPowerData.type);

            //// エフェクト発動！
            base.ActionUVEffect(targetPlayer.GetFieldStrikerCard(), UV_EFFECT_TYPE.RESTRAIN, targetPlayer.isMyPlayer);

            ////+-------------------------------------------
            //// 演出
            //// Ability列車
            //ActionEffectUVInfo info = new ActionEffectUVInfo();
            //info.iEffectType = (int)UV_EFFECT_TYPE.RESTRAIN;
            //Vector3 vPos = Vector3.zero;/* = targetPlayer.GetFieldStrikerCard().cacheTransform.localPosition*/;

            //Card strikerCard = targetPlayer.GetFieldStrikerCard();
            
            //// もし相手のフィールドにストライカーが居なかったら
            //if (strikerCard == null)
            //{

            //}
            //// フィールドにストライカーがいた時
            //else
            //{
            //    vPos = targetPlayer.GetFieldStrikerCard().cacheTransform.localPosition;

            //    // 相手と味方でZ値変える
            //    if (targetPlayer.isMyPlayer == true)
            //    {
            //        vPos.z -= strikerCard.rootTransform.localPosition.z;
            //    }
            //    else
            //    {
            //        vPos.z += strikerCard.rootTransform.localPosition.z;
            //    }
            //}

            ////const int AbjustY = 10;
            //info.fPosX = vPos.x;
            //info.fPosY = vPos.y + 1.0f;
            //info.fPosZ = vPos.z;

            //MessageManager.DispatchOffline(CardAbilityData.playerManager.GetPlayerID(targetPlayer.isMyPlayer),
            // MessageType.ActionEffectUV, info);
            //// SE
            //oulAudio.PlaySE("Heal");

        }

        public override Result Execute(CardAbilityData abilityData)
        {
            const int MAXFrame = 40;
            iFrame++;

            // Ability列車 パワー変化の演出が終わったら
            if (iFrame >= MAXFrame)
            {
                endFlag = true;
            }
            return Result.None;
        }

        public override void Exit(CardAbilityData abilityData)
        {

        }

        public override bool OnMessage(CardAbilityData abilityData, MessageInfo message)
        {
            return false;
        }
    }
}