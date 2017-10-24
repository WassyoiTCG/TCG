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

    public Text myHandCountText, myYamahudaCountText, myBochiCountText, myTsuihouCountText;
    public Text cpuHandCountText, cpuYamahudaCountText, cpuBochiCountText, cpuTsuihouCountText;

    public bool isPlayesStandbyOK()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player) allOK = false;
        return allOK;
    }

    // 自分が操作しているプレイヤーのIDを取得
    public int GetMyPlayerID()
    {
        foreach (Player player in players)
            if (player.isMyPlayer) return player.playerID;

        return -1;
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
            if (!player.isFirstDrawEnd) allOK = false;
        return allOK;
    }

    public bool isSetStrikerEnd()
    {
        bool allOK = true;
        foreach (Player player in players)
            if (!player.isSetStrikerEnd) allOK = false;
        return allOK;
    }

    // マリガン


    // カード設置
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
            players[message.fromPlayerID].SetStrikerOK();
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
            if (players[message.fromPlayerID].isMyPlayer && players[message.fromPlayerID].isSetStriker())
                uiManager.AppearStrikerOK();
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

            // 発信元は同期する必要なし
            if (!players[message.fromPlayerID].isMyPlayer)
            {
                // 手札山札同期
                players[message.fromPlayerID].SyncDeck(syncDeckInfo);

                // UIテキストの変更
                cpuHandCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumHand().ToString();
                cpuYamahudaCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumYamahuda().ToString();
                cpuBochiCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumBochi().ToString();
                cpuTsuihouCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumTuihou().ToString();
            }

            else
            {
                // UIテキストの変更
                myHandCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumHand().ToString();
                myYamahudaCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumYamahuda().ToString();
                myBochiCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumBochi().ToString();
                myTsuihouCountText.text = "x" + players[message.fromPlayerID].deckManager.GetNumTuihou().ToString();
            }

            return true;
        }

        return false;
    }

    public bool StrikerBattle(int score)
    {
        var card0 = players[0].GetFieldStrikerCard();
        var card1 = players[1].GetFieldStrikerCard();
        var winnerPlayerID = -1;

        // ジョーカー判定
        if (card0.cardType == CardType.Joker)
        {
            // 相打ち処理
            if (card1.cardType == CardType.Joker) return false;

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
        {   // 相打ち処理
            if (card0.power == card1.power) return false;

            if (card0.power > card1.power)
                winnerPlayerID = 0;
            
            else if(card1.power > card0.power)
                winnerPlayerID = 1;
        }

        uiManager.AddScore(players[winnerPlayerID].isMyPlayer, score);

        return true;
    }
}
