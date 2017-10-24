using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class oulNetwork : NetworkManager
{
    static public oulNetwork s_Singleton;

    //public InputField ipInput;
    //public NetworkClient client { get; private set; }

    //public bool isServer;

    void Start()
    {
        s_Singleton = this;
        //DontDestroyOnLoad(gameObject);
    }

    //public void OnEnable()
    //{
    //    ipInput.onEndEdit.RemoveAllListeners();
    //    ipInput.onEndEdit.AddListener(onEndEditIP);
    //}

    //public void StartHost()
    //{
    //    // サーバがMsg.Textを受信したときに行う関数を登録する
    //    NetworkServer.RegisterHandler(Msg.MyMessage, networkMessage =>
    //    {
    //        var mes = networkMessage.ReadMessage<MyNetworkMessage>();
    //        Debug.Log("サーバ受信:" + mes.myMessageInfo.messageType.ToString());
    //        // Msg.Textを送ってきたクライアントに返信する
    //        networkMessage.conn.Send(Msg.MyMessage, mes);
    //    });

    //    // サーバー開始
    //    NetworkServer.Listen("127.0.0.1", 7000);
    //    isServer = true;
    //    Debug.Log("サーバー開始キテルグマ");

    //    ConnectClient("127.0.0.1");
    //}

    //public void StartClient()
    //{
    //    isServer = false;

    //    ConnectClient(ipInput.text);
    //}

    //public void ConnectClient(string ip)
    //{
    //    // クライアント開始
    //    client = new NetworkClient();
    //    Debug.Log("クライアント開始キテルグマ");

    //    // クライアントがサーバに接続完了したときに行う関数を登録する
    //    client.RegisterHandler(MsgType.Connect, _ =>
    //    {
    //        Debug.Log("サーバー接続キテルグマ");
    //    });


    //    // クライアントがMsg.Textを受信したときに行う関数を登録する
    //    client.RegisterHandler(Msg.MyMessage, networkMessage =>
    //    {
    //        var mes = networkMessage.ReadMessage<MyNetworkMessage>();
    //        Debug.Log("クライアント受信:" + mes.myMessageInfo.messageType.ToString());
    //        MessageManager.Receive(mes.myMessageInfo);
    //    });

    //    // サーバーに接続
    //    client.Connect(ipInput.text, 7000);
    //}

    public void SendMessage(MessageInfo message)
    {
        var mes = new MyNetworkMessage();
        mes.myMessageInfo.fromPlayerID = message.fromPlayerID;
        mes.myMessageInfo.messageType = message.messageType;
        if (message.exInfo != null)
        {
            mes.myMessageInfo.exInfo = new byte[256];
            int size = message.exInfo.Length;
            Buffer.BlockCopy(message.exInfo, 0, mes.myMessageInfo.exInfo, 0, size);
        }
        // サーバにMsg.Textを送る
        client.Send(Msg.MyMessage, mes);
    }

    public override void OnStartHost()
    {
        base.OnStartHost();

        Debug.Log("ホストキテルグマ");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // サーバがMsg.Textを受信したときに行う関数を登録する
        NetworkServer.RegisterHandler(Msg.MyMessage, networkMessage =>
        {
            var mes = networkMessage.ReadMessage<MyNetworkMessage>();
            Debug.Log("サーバー: " + mes.myMessageInfo.fromPlayerID + "番のプレイヤーからのメッセージを受信" + mes.myMessageInfo.messageType.ToString());
            // Msg.Textを送ってきたクライアントに返信する
            //networkMessage.conn.Send(Msg.MyMessage, mes);
            // クライアント全員に送信する
            NetworkServer.SendToAll(Msg.MyMessage, mes);
        });

        Debug.Log("サーバーキテルグマ");
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);

        // クライアントがMsg.Textを受信したときに行う関数を登録する
        client.RegisterHandler(Msg.MyMessage, networkMessage =>
        {
            var mes = networkMessage.ReadMessage<MyNetworkMessage>();
            Debug.Log("クライアント: " + mes.myMessageInfo.fromPlayerID + "番のプレイヤーからのメッセージを受信" + mes.myMessageInfo.messageType.ToString());
            MessageManager.Receive(mes.myMessageInfo);
        });

        Debug.Log("クライアント開始キテルグマ");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        Debug.Log("クライアント接続キテルグマ");
    }

    //public void OnClickJoin()
    //{
    //    //lobbyManager.ChangeTo(lobbyPanel);
    //
    //    //lobbyManager.networkAddress = ipInput.text;
    //    //lobbyManager.StartClient();
    //
    //    //lobbyManager.backDelegate = lobbyManager.StopClientClbk;
    //    //lobbyManager.DisplayIsConnecting();
    //
    //    //lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
    //}

    //void onEndEditIP(string text)
    //{
    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        StartClient();
    //    }
    //}
}


public enum MessageType
{
    NetPlay,        // ネット対戦プレイ選択
    OfflinePlay,    // オフライン対戦プレイ選択
    SyncDeck,       // 山札・手札・墓地・追放情報の同期
    SyncPoint,      // ポイント情報の同期
    SetCard,        // カードをセット
    BackToHand,     // セットしたカードを手札に戻す動作
    SetStrikerOK,   // ストライカーセット完了
    Marigan,        // マリガン
    NoMarigan,      // マリガンなし
}

//[StructLayout(LayoutKind.Sequential)]
public struct Info1
{
    public int a;
    public int b;
    public Vector3 pos;
}

// デッキ情報同期する用構造体(int型はカードのID)
public struct SyncDeckInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
    public int[] hand;         // 手札
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public int[] yamahuda;     // 山札
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public int[] bochi;        // 墓地
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public int[] tuihou;       // 追放
}

public struct PointInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] points;
}

public struct BackToHandInfo
{
	public int orgHandNo;	// もともと何番目のカードだったか
}

public struct SetCardInfo
{
    public int handNo;  // 手札の何番目のカードがセットされたか
}


public struct MessageInfo
{
    public MessageType messageType;                 // メッセージのID
    public int fromPlayerID;                        // 誰が送ってきたか
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public byte[] exInfo;                           // いつものinfo
    // exInfoのセット
    public void SetExtraInfo(object structure)
    {
        int size = Marshal.SizeOf(structure);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        exInfo = new byte[size];
        Marshal.StructureToPtr(structure, ptr, false);
        Marshal.Copy(ptr, exInfo, 0, size);

        /*
            但しこれではバッファリングのためにメモリを倍食っています。それが嫌ならGCHandleでbyte配列のポインタを取得し、直接そこに書き込むこともできます。
            int size = Marshal.SizeOf(obj);
            byte[] bytes = new byte[size];
            GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, gch.AddrOfPinnedObject(), false);
            gch.Free();
         */

        //exInfo = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
        //Marshal.StructureToPtr(structure, exInfo, false);
    }
}

class MyNetworkMessage : MessageBase
{
    public MessageInfo myMessageInfo;
}

class Msg
{
    public const short MyMessage = MsgType.Highest + 1;
}


public static class MessageManager
{
    static public SceneMain sceneMain;    // ゲーム管理さん
    static public bool isNetwork;

    public static void Start(SceneMain main)
    {
        sceneMain = main;
        isNetwork = false;
    }

    // ネットからの情報を受信
    public static void Receive(MessageInfo message)
    {
        // ゲーム管理さんに知らせる
        sceneMain.HandleMessage(message);
    }

    // ネットに送信する
    public static void Send(MessageInfo message)
    {
        // ネットワークかそうでないかで分岐
        if (isNetwork)
        {
            // サーバにMsg.Textを送る
            oulNetwork.s_Singleton.SendMessage(message);
        }

        else
        {
            Receive(message);
        }
    }

    // 前の真夏っぽい引数でネットに送信
    public static void Dispatch(int fromPlayerID, MessageType messageType, object extraInfo)
    {
        // 
        MessageInfo message = new MessageInfo();
        message.messageType = messageType;
        message.fromPlayerID = fromPlayerID;
        if (extraInfo != null)
            message.SetExtraInfo(extraInfo);

        Send(message);
    }
}