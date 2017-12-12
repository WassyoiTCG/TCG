
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
    public const int numJoker = 1;
    public int jorkerCard;
    public const int numEvent = 4;
    public int[] eventCards;
    public const int deckMax = numStriker + numJoker + numEvent;
    public const int MaxAbilityStrikerNom = 4;
    //public int[] cards = new int[numStriker + numJoker + numEvent];

    public PlayerDeckData()
    {
        deckName = "";
        isEmpty = true;
        strikerCards = Enumerable.Repeat((int)IDType.NONE, numStriker).ToArray();
        eventCards = Enumerable.Repeat((int)IDType.NONE, numEvent).ToArray();
    }

    public int[] GetArray()
    {
        int[] cards = new int[numStriker + numJoker + numEvent];
        for (int i = 0; i < numStriker; i++)
        {
            cards[i] = strikerCards[i];
         }
        cards[numStriker] = jorkerCard;
        for (int i = 0; i < numEvent; i++)
        {
            cards[numStriker + numJoker + i] = eventCards[i];
        }
        return cards;
    }

    public void SetArray(int[] array)
    {
        for (int i = 0; i < numStriker; i++)
        {
            strikerCards[i] = array[i];
        }
        jorkerCard = array[numStriker];
        for (int i = 0; i < numEvent; i++)
        {
            eventCards[i] = array[numStriker + numJoker + i];
        }
    }

    // ストライカーとジョーカーが全部セットされているかどうかチェック
    public bool isSetAllStriker()
    {
        foreach (int strikerCardID in strikerCards)
            if (strikerCardID == (int)IDType.NONE) return false;

        if (jorkerCard == (int)IDType.NONE) return false;

        return true;
    }
}


public class PlayerData
{
    public const int numDeckData = 6;
    PlayerDeckData[] deckDatas;
    public PlayerDeckData GetDeckData(int slotNo) { return deckDatas[slotNo]; }

    public uint coin;
    public uint playCount;
    public uint playTime;

    public PlayerData()
    {
        deckDatas = new PlayerDeckData[numDeckData];
        for (int i = 0; i < numDeckData; i++)
        {
            deckDatas[i] = new PlayerDeckData();
        }
        coin = 0;
        playCount = 0;
        playTime = 0;
    }
}

public static class PlayerDataManager
{
    static PlayerData playerData = new PlayerData();

    public static PlayerData GetPlayerData() { return playerData; }

    static void LoadDeck()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "deck.txt";
        var loader = new TextLoader();
        loader.LoadText(path);

        for (int i = 0; i < PlayerData.numDeckData; i++)
        {
            var deckData = playerData.GetDeckData(i);

            // デッキ名読み込み
            //deckData = new PlayerDeckData();
            deckData.deckName = loader.ReadDoubleQuotation();
            deckData.isEmpty = (deckData.deckName == "");
            if (deckData.isEmpty)
            {
                continue;
            }

            // ストライカー読み込み
            for (int j = 0; j < PlayerDeckData.numStriker; j++)
            {
                deckData.strikerCards[j] = loader.ReadInt();
            }
            // ジョーカー読み込み
            deckData.jorkerCard = loader.ReadInt();
            // イベント読み込み
            for (int j = 0; j < PlayerDeckData.numEvent; j++)
            {
                deckData.eventCards[j] = loader.ReadInt();
            }
        }
    }

    static void LoadPlayerData()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "save.txt";
        var loader = new TextLoader();
        loader.LoadText(path);


        // コイン
        playerData.coin = (uint)loader.ReadInt();

        // プレイ回数
        playerData.playCount = (uint)loader.ReadInt();

        // コイン
        playerData.playTime = (uint)loader.ReadInt();
    }

    public static void Load()
    {
        LoadDeck();
        LoadPlayerData();
    }

    public static void DeckSave(int slotNo, int[] ID15)
    {
        Debug.Assert(ID15.Length == 15);
        playerData.GetDeckData(slotNo).SetArray(ID15);
        AllDeckSave();
    }

    public static void AllDeckSave()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "deck.txt";

        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.ASCII))
        {
            for (int i = 0; i < PlayerData.numDeckData; i++)
            {
                var deckData = playerData.GetDeckData(i);

                // デッキ名描きだし
                writer.Write("\"" + deckData.deckName + "\"" + "\r\n");
                if (deckData.isEmpty) continue;

                // ストライカー描きだし
                for (int j = 0; j < PlayerDeckData.numStriker; j++)
                {
                    writer.Write(deckData.strikerCards[j].ToString() + " ");
                }
                // ジョーカー描きだし
                writer.Write("\r\n" + deckData.jorkerCard.ToString() + "\r\n");
                // イベント描きだし
                for (int j = 0; j < PlayerDeckData.numEvent; j++)
                {
                    writer.Write(deckData.eventCards[j].ToString() + ((j == PlayerDeckData.numEvent - 1) ? "\r\n" : " "));
                }
            }
        }

    }

    public static void PlayerDataSave()
    {
        var directory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/SaveData";
        var path = directory + "/" + "save.txt";

        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.ASCII))
        {
            // コイン
            writer.Write("\r\n" + playerData.coin.ToString());

            // プレイ回数
            writer.Write("\r\n" + playerData.playCount.ToString());

            // プレイ回数
            writer.Write("\r\n" + playerData.playTime.ToString());
        }
    }

    public static void AllSave()
    {
        AllDeckSave();
        PlayerDataSave();
    }
}