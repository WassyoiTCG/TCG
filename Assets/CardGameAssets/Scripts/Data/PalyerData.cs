
//+-------------------------------------------
//  ゲーム起動中だけずっと残っているデータ
//+-------------------------------------------

using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerDeckData
{
    public string deckName;
    public bool isEmpty;
    public const int numStriker = 10;
    public int[] strikerCards;
    public int jorkerCard;
    public const int numEvent = 4;
    public int[] eventCards;

    public PlayerDeckData()
    {
        deckName = "";
        isEmpty = true;
        strikerCards = Enumerable.Repeat(-1, numStriker).ToArray();
        eventCards = Enumerable.Repeat(-1, numEvent).ToArray();
    }
}


public class PlayerData
{
    public const int numDeckData = 6;
    public PlayerDeckData[] deckDatas = new PlayerDeckData[numDeckData];
    public uint coin;
    public uint playCount;
    public uint playTime;

    public PlayerData()
    {
        deckDatas = new PlayerDeckData[numDeckData];
        coin = 0;
        playCount = 0;
        playTime = 0;
    }
}

public static class PlayerDataManager
{
    static PlayerData playerData = new PlayerData();

    public static void Load()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "save.txt";
        var loader = new TextLoader();
        loader.LoadText(path);

        // 読み飛ばし用
        //var skip = "";

        for (int i = 0; i < PlayerData.numDeckData; i++)
        {
            // デッキ名読み込み
            playerData.deckDatas[i] = new PlayerDeckData();
            playerData.deckDatas[i].deckName = loader.ReadDoubleQuotation();
            playerData.deckDatas[i].isEmpty = (playerData.deckDatas[i].deckName == "");
            if(playerData.deckDatas[i].isEmpty)
            {
                continue;
            }

            // ストライカー読み込み
            for (int j = 0; j < PlayerDeckData.numStriker; j++)
            {
                playerData.deckDatas[i].strikerCards[j] = loader.ReadInt();
            }
            // ジョーカー読み込み
            playerData.deckDatas[i].jorkerCard = loader.ReadInt();
            // イベント読み込み
            for (int j = 0; j < PlayerDeckData.numEvent; j++)
            {
                playerData.deckDatas[i].eventCards[j] = loader.ReadInt();
            }
        }

        // コイン
        playerData.coin = (uint)loader.ReadInt();

        // プレイ回数
        playerData.playCount = (uint)loader.ReadInt();

        // コイン
        playerData.playTime = (uint)loader.ReadInt();
    }

    public static void Save()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "save.txt";

        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.ASCII))
        {
            for (int i = 0; i < PlayerData.numDeckData; i++)
            {
                // デッキ名描きだし
                writer.Write("\"" + playerData.deckDatas[i].deckName + "\"" + "\r\n");
                if (playerData.deckDatas[i].isEmpty) continue;

                // ストライカー描きだし
                for (int j = 0; j < PlayerDeckData.numStriker; j++)
                {
                    writer.Write(playerData.deckDatas[i].strikerCards[j].ToString() + " ");
                }
                // ジョーカー描きだし
                writer.Write("\r\n" + playerData.deckDatas[i].jorkerCard.ToString() + "\r\n");
                // イベント描きだし
                for (int j = 0; j < PlayerDeckData.numEvent; j++)
                {
                    writer.Write(playerData.deckDatas[i].eventCards[j].ToString() + " ");
                }
            }

            // コイン
            writer.Write("\r\n" + playerData.coin.ToString());

            // プレイ回数
            writer.Write("\r\n" + playerData.playCount.ToString());

            // コイン
            writer.Write("\r\n" + playerData.playTime.ToString());
        }
    }
}