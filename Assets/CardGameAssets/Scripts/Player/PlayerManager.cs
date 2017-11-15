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

    public Player GetPlayer(int id) { return players[id]; }

    // 自分が操作しているプレイヤーのIDを取得
    public int GetMyPlayerID()
    {
        foreach (Player player in players)
            if (player.isMyPlayer) return player.playerID;

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

    public void SetState(PlayerState state)
    {
        foreach (Player player in players)
            player.ChangeState(state);
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

    public void Draw()
    {
        foreach (Player player in players)
            player.Draw();
    }

    public bool isFirstDrawEnd()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isFirstDrawEnd || !player.isSynced) allOK = false;
        return allOK;
    }

    public bool isSetStrikerEnd()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isSetStrikerOK()) allOK = false;
        return allOK;
    }

    public void OpenStriker()
    {
        foreach (Player player in players)
            player.OpenStriker();
    }

    public bool HandleMessage(MessageInfo message)
    {
        // switchにすると構造体受け取りでバグる
        if(message.messageType == MessageType.Marigan)
        {
            players[message.fromPlayerID].Marigan();
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
            // ボタン非表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.DisableSetStrikerButton();
            return true;
        }
        if (message.messageType == MessageType.SetCard)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SetCardInfo setCardInfo = new SetCardInfo();
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(setCardInfo));
            Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(setCardInfo));
            setCardInfo = (SetCardInfo)Marshal.PtrToStructure(ptr, setCardInfo.GetType());
            Marshal.FreeHGlobal(ptr);

            Debug.Log("受信: プレイヤー" + message.fromPlayerID + "が手札" + setCardInfo.handNo + "のカードをセットしました");

            players[message.fromPlayerID].SetCard(setCardInfo);
            // ボタン表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.EnableSetStrikerButton();
            return true;
        }
        if(message.messageType == MessageType.BackToHand)
        {
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            BackToHandInfo backToHandInfo = new BackToHandInfo();
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(backToHandInfo));
            Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(backToHandInfo));
            backToHandInfo = (BackToHandInfo)Marshal.PtrToStructure(ptr, backToHandInfo.GetType());
            Marshal.FreeHGlobal(ptr);

            players[message.fromPlayerID].BackToHand(backToHandInfo);

            // ボタン非表示
            if (players[message.fromPlayerID].isMyPlayer)
                uiManager.DisableSetStrikerButton();

            return true;
        }
        if(message.messageType == MessageType.SyncDeck)
        { 
            if (message.exInfo == null)
                return false;

            // byte[]→構造体
            SyncDeckInfo syncDeckInfo = new SyncDeckInfo();
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(syncDeckInfo));
            Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(syncDeckInfo));
            syncDeckInfo = (SyncDeckInfo)Marshal.PtrToStructure(ptr, syncDeckInfo.GetType());
            Marshal.FreeHGlobal(ptr);

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
            players[message.fromPlayerID].isSynced = true;

            return true;
        }

        return false;
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

            winnerPlayerID = 0;
        }
        else if(card1 == null)
        {
            winnerPlayerID = 1;
        }

        // ジョーカー判定
        else if (card0.cardType == CardType.Joker)
        {
            // 相打ち処理
            if (card1.cardType == CardType.Joker) return -1;

            if (card1.power == 10)
                winnerPlayerID = 0;
            
            else
                winnerPlayerID = 1;
            
        }
        else if (card1.cardType == CardType.Joker)
        {
            if (card0.power == 10)
                winnerPlayerID = 1;

            else
                winnerPlayerID = 0;
            
        }
        else
        {
            var power0 = players[0].jissainoPower;
            var power1 = players[1].jissainoPower;

            // 相打ち処理
            if (power0 == power1) return -1;

            if (power0 > power1)
                winnerPlayerID = 0;
            
            else if(power1 > power0)
                winnerPlayerID = 1;
        }

        return winnerPlayerID;
    }

    public void Restart()
    {
        foreach (Player player in players)
            player.Restart();
    }
}
