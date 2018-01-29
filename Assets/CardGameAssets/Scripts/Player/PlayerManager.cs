using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    const int numPlayer = 2;

    public UIManager uiManager;
    public SceneMain sceneMain;

    //public List<Player> players { get; private set; }
    public Player[] players = new Player[numPlayer];

    //public Player myPlayer;         // 自分
    //public Player opponentPlayer;   // 対戦相手プレイヤー

    public bool isPlayesStandbyOK()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player) allOK = false;
        return allOK;
    }

    public bool isPlayerNameSync()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (player.playerName == "") allOK = false;
        return allOK;
    }

    public bool isSyncDeckOK()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isSyncDeck) allOK = false;
        return allOK;
    }

    public void SyncDeckOff()
    {
        foreach (Player player in players)
            player.isSyncDeck = false;
    }

    //public Player GetPlayer(int id)
    //{
    //    foreach (Player player in players)
    //        if (player.playerID == id) return player;

    //    Debug.LogWarning("プレイヤー取得null");
    //    return null;
    //}
    //public Player GetAitePlayer(int id)
    //{
    //    foreach (Player player in players)
    //        if (player.playerID != id) return player;

    //    Debug.LogWarning("プレイヤー取得null");
    //    return null;
    //}

    // 自分が操作しているプレイヤーのIDを取得
    public Player GetPlayer(int id) { return players[id]; }

    public int GetPlayerID(bool isMyPlayer)
    {
        foreach (Player player in players)
            if (player.isMyPlayer == isMyPlayer) return player.playerID;

        return (int)IDType.NONE;
    }

    // 自分が操作しているプレイヤーのIDを取得
    public int GetMyPlayerID()
    {
        foreach (Player player in players)
            if (player.isMyPlayer) return player.playerID;

        return -1;
    }

    public int GetCpuPlayerID()
    {
        foreach (Player player in players)
            if (!player.isMyPlayer) return player.playerID;

        return -1;
    }

    public Player GetMyPlayer()
    {
        foreach (Player player in players)
            if (player.isMyPlayer) return player;

        return null;
    }

    public Player GetCPUPlayer()
    {
        foreach (Player player in players)
            if (!player.isMyPlayer) return player;

        return null;
    }

    public Player GetCPUPlayerByID(int playerID)
    {
        foreach (Player player in players)
            if (player.playerID != playerID) return player;

        return null;
    }

    public void SetState(BaseEntityState<Player> state)
    {
        foreach (Player player in players)
            player.ChangeState(state);
    }

    public void SetState(BaseEntityState<Player> state, int no)
    {
        players[no].ChangeState(state);
    }

    public int AddPlayer(Player player)
    {
        int id = -1;
        for (int i = 0; i < numPlayer; i++)
        {
            if(players[i] == null)
            {
                id = i;
                // 子供にする
                player.transform.parent = transform;
                // リスト追加
                //players.Add(player);
                players[id] = player;
                break;
            }
        }

        return id;
    }


    public bool isStateEnd()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isStateEnd) allOK = false;
        return allOK;
    }

    public bool isSetStrikerEnd()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isSetStrikerOK()) allOK = false;
        return allOK;
    }

    public bool isPushedJunbiKanryo()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isPushedJunbiKanryo) allOK = false;
        return allOK;
    }

    public bool isPushedNextButton()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isPushedNextButton) allOK = false;
        return allOK;
    }

    public void OpenStriker()
    {
        foreach (Player player in players)
            player.OpenStriker();
    }

    public void AttackStriker()
    {
        foreach (Player player in players)
            player.AttackStriker();
    }

    public void LimitReset()
    {
        foreach (Player player in players)
            player.SetNoneLimitPower();
    }

    public void ActionIntercept()
    {
        var card0 = players[0].GetFieldEventCard();
        var card1 = players[1].GetFieldEventCard();
        var abilityManager = GameObject.Find("GameMain/AbilityManager").GetComponent<CardAbilityManager>();
        if (card0 == null)
        {
            if (card1 == null)
            {
            // どっちもいべんとつかってない

                return;
            }

            // [NEW] JOKERにインターセプトを適用したので分岐がいる
            if (card1.cardType == CardType.Joker)
            {

                var abilityes = card1.jokerCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[1])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[1].isMyPlayer);
                }
            }
            else if (card1.cardType == CardType.Intercept)
            {

                var abilityes = card1.interceptCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[1])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[1].isMyPlayer);
                }
            }
            else
            {
                Debug.LogWarning("INTERCEPT・JOKER以外の何かが来た");
            } 
            

        }
        else if(card1 == null)
        {
            // [NEW] JOKERにインターセプトを適用したので分岐がいる

            if (card0.cardType == CardType.Joker)
            {
                var abilityes = card0.jokerCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[0])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[0].isMyPlayer);
                }
            }
            else if (card0.cardType == CardType.Intercept)
            {
                var abilityes = card0.interceptCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[0])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[0].isMyPlayer);
                }
            }
            else
            {
                Debug.LogWarning("INTERCEPT・JOKER以外の何かが来た");
            }
        }
        else
        {
            // 相殺処理
            //if(card0.power == card1.power)
            //{

            //    return;
            //}

            //var abilityes0 = card0.interceptCard.abilityDatas;
            //var abilityes1 = card1.interceptCard.abilityDatas;

            //foreach(CardAbilityData ability in abilityes0)
            //{
            //    if (ability.HatsudouOK(players[0]))
            //        abilityManager.PushAbility(ability, players[0].isMyPlayer);
            //}
            //foreach(CardAbilityData ability in abilityes1)
            //{
            //    if (ability.HatsudouOK(players[1]))
            //        abilityManager.PushAbility(ability, players[1].isMyPlayer);
            //}


            //+---------------------------------------------------
            // [NEW] JOKERにインターセプトを適用したので分岐がいる
            if (card0.cardType == CardType.Joker)
            {
                var abilityes = card0.jokerCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[0])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[0].isMyPlayer);
                }
            }
            else if (card0.cardType == CardType.Intercept)
            {
                var abilityes = card0.interceptCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[0])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[0].isMyPlayer);
                }
            }
            else
            {
                Debug.LogWarning("INTERCEPT・JOKER以外の何かが来た");
            }

            //+---------------------------------------------------
            // [NEW] JOKERにインターセプトを適用したので分岐がいる
            if (card1.cardType == CardType.Joker)
            {

                var abilityes = card1.jokerCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[1])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[1].isMyPlayer);
                }
            }
            else if (card1.cardType == CardType.Intercept)
            {

                var abilityes = card1.interceptCard.abilityDatas;
                foreach (CardAbilityData ability in abilityes)
                {
                    // 効果の条件を満たしているかどうか(爪痕とかのチェック)
                    if (!ability.HatsudouOK(players[1])) return;
                    // 効果発動!
                    abilityManager.PushAbility(ability, players[1].isMyPlayer);
                }
            }
            else
            {
                Debug.LogWarning("INTERCEPT・JOKER以外の何かが来た");
            }

            //if (!ability0.HatsudouOK(players[0]))
            //{
            //    if (!ability1.HatsudouOK(players[1])) return;
            //    abilityManager.PushAbility(ability1, players[1].isMyPlayer);
            //}
            //else if (!ability1.HatsudouOK(players[1]))
            //{
            //    abilityManager.PushAbility(ability0, players[0].isMyPlayer);
            //}
            //// 両方発動できる
            //else
            //{
            //    // パワーの高い方から処理する
            //    if (card0.power > card1.power)
            //    {
            //        abilityManager.PushAbility(ability0, players[0].isMyPlayer);
            //        abilityManager.PushAbility(ability1, players[1].isMyPlayer);
            //    }
            //    else
            //    {
            //        abilityManager.PushAbility(ability1, players[1].isMyPlayer);
            //        abilityManager.PushAbility(ability0, players[0].isMyPlayer);
            //    }
            //}
        }
    }

    //public bool isHaveInterceptCard()
    //{
    //    foreach (Player player in players)
    //        if (player.isHaveInterceptCard()) return true;
    //    return false;
    //}

    public bool HandleMessage(MessageInfo message)
    {
        // switchにすると構造体受け取りでバグる
        if(message.messageType == MessageType.Marigan)
        {
            // byte[]→構造体
            SyncDeckInfo syncDeckInfo = new SyncDeckInfo();
            message.GetExtraInfo<SyncDeckInfo>(ref syncDeckInfo);

            players[message.fromPlayerID].Marigan(syncDeckInfo);
            return true;
        }
        if (message.messageType == MessageType.NoMarigan)
        {
            players[message.fromPlayerID].NoMarigan();
            return true;
        }
        if(message.messageType == MessageType.SetStrikerOK)
        {
            players[message.fromPlayerID].JunbiKanryoON();
            // ボタン非表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.DisableSetStrikerButton();
            return true;
        }
        if (message.messageType == MessageType.SetStrikerPass)
        {
            players[message.fromPlayerID].JunbiKanryoON();
            players[message.fromPlayerID].PushedNextButtonON();
            // ボタン非表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.DisableSetStrikerButton();
            return true;
        }

        // もう一回NEXT押してどうぞ
        if (message.messageType == MessageType.AgainNextButton)
        {
            players[message.fromPlayerID].JunbiKanryoOFF();
            players[message.fromPlayerID].PushedNextButtonOFF();
            return true;
        }

        if (message.messageType == MessageType.SetStrikerOK)
        {
            players[message.fromPlayerID].JunbiKanryoON();
            return true;
        }

        if (message.messageType == MessageType.SetStriker)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SelectCardIndexInfo setCardInfo = new SelectCardIndexInfo();
            message.GetExtraInfo<SelectCardIndexInfo>(ref setCardInfo);
            //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(setCardInfo));
            //Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(setCardInfo));
            //setCardInfo = (SetCardInfo)Marshal.PtrToStructure(ptr, setCardInfo.GetType());
            //Marshal.FreeHGlobal(ptr);

            Debug.Log("受信: プレイヤー" + message.fromPlayerID + "が手札" + setCardInfo.index + "のカードをセットしました");

            players[message.fromPlayerID].SetCard(setCardInfo);
            // ボタン表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.EnableSetStrikerButton();
            return true;
        }
        if (message.messageType == MessageType.SetSupport)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SelectCardIndexInfo setCardInfo = new SelectCardIndexInfo();
            message.GetExtraInfo<SelectCardIndexInfo>(ref setCardInfo);

            Debug.Log("受信: プレイヤー" + message.fromPlayerID + "が手札" + setCardInfo.index + "のサポートカードを発動");

            players[message.fromPlayerID].SetSupport(setCardInfo);

            // ★シーンメインのステートをサポート待ちのステートに変更
            Debug.Assert(sceneMain, "PlayerManagerがscemeMainアタッチされてない");
            sceneMain.ChangeState(SceneMainState.SupportExcusionState.GetInstance());

            // ★相手プレイヤーがすでにOKボタンを押していたら、もう一度表示させる。
            var youPlayer = GetCPUPlayerByID(message.fromPlayerID);
            if (youPlayer.isPushedJunbiKanryo)
            {
                // もう一回NEXT押してどうぞ
                youPlayer.JunbiKanryoOFF();
                // ボタン表示
                if (youPlayer.isMyPlayer)
                    uiManager.EnableSetStrikerButton();
            }

            return true;
        }
        if (message.messageType == MessageType.SetIntercept)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SelectCardIndexInfo setCardInfo = new SelectCardIndexInfo();
            message.GetExtraInfo<SelectCardIndexInfo>(ref setCardInfo);

            Debug.Log("受信: プレイヤー" + message.fromPlayerID + "が手札" + setCardInfo.index + "のインターセプトカードを発動");

            players[message.fromPlayerID].SetIntercept(setCardInfo);
            // ステート終了
            //players[message.fromPlayerID].isStateEnd = true;
            players[message.fromPlayerID].isPushedJunbiKanryo = true;

            //+---------------------------------------------------
            // ★★送られてきた方じゃないプレイヤーのステートが
            // 操作しているやつならもう一回操作できるように
            int iID = (message.fromPlayerID == 0) ? 1 : 0;
            if (iID == GetMyPlayerID())
            {
                AgainSetIntercept(iID);
            }
            //players[iID].stateMachine.currentState




            // [1212] インターセプトカードを伏せた後も押さす用に
            // ボタン非表示
            //if (players[message.fromPlayerID].isMyPlayer)
            //    uiManager.DisableSetStrikerButton();
            return true;
        }
        if(message.messageType == MessageType.BackToHand)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            BackToHandInfo backToHandInfo = new BackToHandInfo();
            message.GetExtraInfo<BackToHandInfo>(ref backToHandInfo);
            //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(backToHandInfo));
            //Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(backToHandInfo));
            //backToHandInfo = (BackToHandInfo)Marshal.PtrToStructure(ptr, backToHandInfo.GetType());
            //Marshal.FreeHGlobal(ptr);

            players[message.fromPlayerID].BackToHand(backToHandInfo);

            // ボタン非表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.DisableSetStrikerButton();

            return true;
        }
        //if(message.messageType == MessageType.SyncName)
        //{
        //    if (message.exInfo == null)
        //        return false;

        //    // 自分の名前なら同期する必要なし
        //    //if (players[message.fromPlayerID].isMyPlayer) return true;

        //    SyncNameInfo syncNameInfo = new SyncNameInfo();
        //    //syncNameInfo.cName = new char[64];
        //    message.GetExtraInfo<SyncNameInfo>(ref syncNameInfo);

        //    // プレイヤーの名前設定
        //    players[message.fromPlayerID].playerName = syncNameInfo.playerName;//new string(syncNameInfo.cName);
        //}
        if(message.messageType == MessageType.SyncDeck)
        { 
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SyncDeckInfo syncDeckInfo = new SyncDeckInfo();
            message.GetExtraInfo<SyncDeckInfo>(ref syncDeckInfo);
            //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(syncDeckInfo));
            //Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(syncDeckInfo));
            //syncDeckInfo = (SyncDeckInfo)Marshal.PtrToStructure(ptr, syncDeckInfo.GetType());
            //Marshal.FreeHGlobal(ptr);

            //Debug.Log("受信: プレイヤー" + message.fromPlayerID + "の手札情報:" +
            //    syncDeckInfo.yamahuda[0] + "," +
            //    syncDeckInfo.yamahuda[1] + "," +
            //    syncDeckInfo.yamahuda[2] + "," +
            //    syncDeckInfo.yamahuda[3] + "," +
            //    syncDeckInfo.yamahuda[4] + "," +
            //    syncDeckInfo.yamahuda[5] + "," +
            //    syncDeckInfo.yamahuda[6] + "," +
            //    syncDeckInfo.yamahuda[7] + "," +
            //    syncDeckInfo.yamahuda[8]);

            var isMyPlayer = players[message.fromPlayerID].isMyPlayer;

            // 発信元は同期する必要なし
            if (!isMyPlayer)
            {
                // 手札山札同期
                players[message.fromPlayerID].SyncDeck(syncDeckInfo);
            }

            // UIテキストの変更
            uiManager.UpdateDeckUI(players[message.fromPlayerID].deckManager, isMyPlayer);

            // 同期フラグ
            players[message.fromPlayerID].isSyncDeck = true;

            return true;
        }

        return false;
    }


    // 戻す
    public void AgainSetIntercept(int playerID)
    {
        // プレイヤーステート変更(インターセプトセットステート)
        //players[playerID].stateMachine.ChangeState(PlayerState.SetIntercept.GetInstance());
        GetMyPlayer().stateMachine.ChangeState(PlayerState.SetIntercept.GetInstance());

        //pMain.uiManager.AppearStrikerOK(false);  // falseにするとパスが出る
        uiManager.AppearInterceptOK();// [1211] NextButton

        //players[playerID].cardObjectManager.CheakActiveUseCard();
        GetMyPlayer().cardObjectManager.CheakActiveUseCard();

        // 相手側に自分のIDはまだおしてないことを伝える
        MessageManager.Dispatch(playerID, MessageType.AgainNextButton, null);
        //players[playerID].isPushedNextButton = false; // ボタンを押していなかったことに
    }

    public int StrikerBattle()
    {
        var card0 = players[0].GetFieldStrikerCard();
        var card1 = players[1].GetFieldStrikerCard();
        var winnerPlayerID = -1;

        // パス判定
        if(card0 == null)
        {
            if (card1 == null) return -1;

            winnerPlayerID = 1;
        }
        else if(card1 == null)
        {
            winnerPlayerID = 0;
        }

        // ジョーカー判定
        else if (card0.cardData.cardType == CardType.Joker && card1.cardData.power == 10)
        {
            // 相打ち処理
            //if (card1.cardData.cardType == CardType.Joker) return -1;

            //if (card1.cardData.power == 10)
                winnerPlayerID = 0;
            
            //else
            //    winnerPlayerID = 1;
            
        }
        else if (card1.cardData.cardType == CardType.Joker && card0.cardData.power == 10)
        {
            //if (card0.cardData.power == 10)
                winnerPlayerID = 1;

            //else
            //    winnerPlayerID = 0;
            
        }
        else
        {
            var power0 = players[0].jissainoPower;
            var power1 = players[1].jissainoPower;

            Debug.Log(power0 + " vs " + power1);

            // 相打ち処理
            if (power0 == power1) return -1;

            if (power0 > power1)
            {
                winnerPlayerID = 0;
                // カード負けるモーション
                //card1.Lose();
            }
            else if (power1 > power0)
            {
                winnerPlayerID = 1;
                // カード負けるモーション
               // card0.Lose();
            }
        }

        return winnerPlayerID;
    }

    public void Restart()
    {
        foreach (Player player in players)
            if(player)player.Restart();
    }
}
