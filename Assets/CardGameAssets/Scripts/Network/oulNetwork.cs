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

    //public NetworkClient client { get; private set; }

    //List<ClientMessageOKInfo> clientIDList = new List<ClientMessageOKInfo>();
    Queue<MyNetworkMessage> messageQueue = new Queue<MyNetworkMessage>();
    MyNetworkMessage currentMessage;
    float waitTimer = 0;
    readonly float delay = 0.5f;

    //float waitTimer;
    int sendMessageNumber;
    //int receiveMessageNumber;

    MessageInfo[] messageBuffer = new MessageInfo[2048];

    List<NetworkConnection> clientConnections = new List<NetworkConnection>();
    List<NetworkConnection> serverConnections = new List<NetworkConnection>();

    //class ClientMessageOKInfo
    //{
    //    public int connectionId;   // 接続してるクライアントのID
    //    public bool sendOK;        // 正しくメッセージが送れたか
    //}


    void Start()
    {
        s_Singleton = this;
        Restart();
        //DontDestroyOnLoad(gameObject);
    }

    public void Stop()
    {
        if(MessageManager.isServer)
        {
            // ホスト終了
            StopHost();
        }
        else
        {
            // クライアント終了
            StopClient();
        }
    }

    public void Restart()
    {
        currentMessage = null;
        waitTimer = 0;

        sendMessageNumber = 0;
        //receiveMessageNumber = 0;
        for (int i = 0; i < messageBuffer.Length; i++)
        {
            messageBuffer[i] = new MessageInfo();
            messageBuffer[i].messageType = MessageType.NoMessage;
        }
    }

    //bool isAllSendOK()
    //{
    //    foreach(ClientMessageOKInfo c in clientIDList)
    //    {
    //        if (!c.sendOK) return false;
    //    }
    //    return true;
    //}

    //[Server]
    void Update()
    {
        if (!MessageManager.isServer) return;

        if(currentMessage != null)
        {
            // ゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリゴリ
            // でぃれいじかん、つくろう
            switch (currentMessage.myMessageInfo.messageType)
            {
                case MessageType.SelectCemetery:
                case MessageType.SelectHand:
                case MessageType.SelectNumber:
                case MessageType.SelectYamahuda:
                    if ((waitTimer += Time.deltaTime) > delay)
                    {
                        waitTimer = 0;
                    }
                    else return;
                    break;
            }


            // クライアントに送信
            //var res = NetworkServer.SendToAll(Msg.MyMessage, currentMessage);
            bool result = false;

            // this list holds all connections (local and remote)
            foreach(NetworkConnection conn in NetworkServer.connections)
            {
                if (conn != null)
                    result &= conn.Send(Msg.MyMessage, currentMessage);
            }

            // クライアント全員に送信する
            Debug.Log("クライアント全員に" + currentMessage.myMessageInfo.number + "番のメッセージ送信[" + currentMessage.myMessageInfo.messageType.ToString() +
                "]" + result.ToString());

            currentMessage = null;
        }
        else if(messageQueue.Count > 0)
        {
            currentMessage = messageQueue.Dequeue();

            //// 全員に送れたら
            //if (isAllSendOK())
            //{
            //    // フラグリセット
            //    foreach (ClientMessageOKInfo c in clientIDList)
            //    {
            //        c.sendOK = false;
            //    }
            //    // 先頭メッセージ削除
            //    messageQueue.RemoveAt(0);
            //    return;
            //}

            //if ((waitTimer += Time.deltaTime) > 0.5f)
            //{
            //    foreach (ClientMessageOKInfo c in clientIDList)
            //    {
            //        if (!c.sendOK) NetworkServer.SendToClient(c.connectionId, Msg.MyMessage, messageQueue[0]);
            //    }
            //}


        }
    }

    //public void OnEnable()
    //{
    //    ipInput.onEndEdit.RemoveAllListeners();
    //    ipInput.onEndEdit.AddListener(onEndEditIP);
    //}

    public override NetworkClient StartHost()
    {
        Debug.Log("ホストを開始します。");

        return base.StartHost();
        //// サーバー開始
        //NetworkServer.Listen("127.0.0.1", 7000);
        //
        //ConnectClient("127.0.0.1");
    }

    public NetworkClient StartClient2(string ip)
    {
        MessageManager.isServer = false;
        // 入力されたipアドレスを保存
        networkAddress = ip;
        // Unityちゃんにマルナゲータ
        return StartClient();
        //ConnectClient(ipInput.text);

        Debug.Log("クライアントを開始します。IP-" + ip);
    }

    //public void ConnectClient(string ip)
    //{
    //    // クライアント開始
    //    client = new NetworkClient();
    //    //Debug.Log("クライアント開始キテルグマ");

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
        //client.Send(Msg.MyMessage, mes);
        // サーバーに届くまでMsg.Textを送り続ける
        for (int i = 0; ; i++)
        {
            if (client.Send(Msg.MyMessage, mes))
                break;
            // 114514回送っても返事が来ないので死んでどうぞ
            if (i > 114514)
            {
                Application.Quit();
            }
        }
    }

    public override void OnStartHost()
    {
        base.OnStartHost();

        Debug.Log("ホストキテルグマ");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 
        MessageManager.isServer = true;

        // サーバがMsg.Textを受信したときに行う関数を登録する
        NetworkServer.RegisterHandler(Msg.MyMessage, networkMessage =>
        {
            var mes = networkMessage.ReadMessage<MyNetworkMessage>();
            Debug.Log("サーバー: " + networkMessage.conn.connectionId + "番のプレイヤーからのメッセージを受信" + mes.myMessageInfo.messageType.ToString());

            // 
            //if (mes.myMessageInfo.messageType == MessageType.ReceiveOK)
            //{
            //    // byte[]→構造体
            //    ReceiveOKInfo info = new ReceiveOKInfo();
            //    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            //    Marshal.Copy(mes.myMessageInfo.exInfo, 0, ptr, Marshal.SizeOf(info));
            //    info = (ReceiveOKInfo)Marshal.PtrToStructure(ptr, info.GetType());
            //    Marshal.FreeHGlobal(ptr);

            //    for (int i = 0; i < clientIDList.Count; i++)
            //    {
            //        if (clientIDList[i].connectionId == networkMessage.conn.connectionId)
            //        {
            //            clientIDList[i].sendOK = true;
            //            break;
            //        }
            //    }
            //    return;
            //}
            if(mes.myMessageInfo.messageType == MessageType.ReMessage)
            {
                // byte[]→構造体
                ReMessageInfo reMessageInfo = new ReMessageInfo();
                mes.myMessageInfo.GetExtraInfo<ReMessageInfo>(ref reMessageInfo);
                //IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(reMessageInfo));
                //Marshal.Copy(mes.myMessageInfo.exInfo, 0, ptr, Marshal.SizeOf(reMessageInfo));
                //reMessageInfo = (ReMessageInfo)Marshal.PtrToStructure(ptr, reMessageInfo.GetType());
                //Marshal.FreeHGlobal(ptr);

                // 返信
                MyNetworkMessage re = new MyNetworkMessage();
                re.myMessageInfo = messageBuffer[reMessageInfo.messageNumber];
                Debug.Assert(re.myMessageInfo.messageType != MessageType.NoMessage, "ReMessageでエラー");
                networkMessage.conn.Send(Msg.MyMessage, re);
            }

            Debug.Log("メッセージ保存: " + sendMessageNumber);

            // 番号
            messageBuffer[sendMessageNumber] = mes.myMessageInfo;
            mes.myMessageInfo.number = sendMessageNumber++;


            // Msg.Textを送ってきたクライアントに返信する
            // 受け取ったよメッセージを送る
            //MyNetworkMessage re = new MyNetworkMessage();
            //re.myMessageInfo.messageType = MessageType.ReceiveOK;
            //ReceiveOKInfo exInfo = new ReceiveOKInfo();
            //exInfo.iMessageType = (uint)mes.myMessageInfo.messageType;
            //re.myMessageInfo.SetExtraInfo(exInfo);
            //networkMessage.conn.Send(Msg.MyMessage, re);

            // クライアント全員に送信する
            //NetworkServer.SendToAll(Msg.MyMessage, mes);

            // おくられてないとき用にメッセージ保存
            messageQueue.Enqueue(mes);
            //waitTimer = 114514;
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

            Debug.Log("クライアント: " + "サーバーからメッセージを受信" + mes.myMessageInfo.messageType.ToString());

            //if (receiveMessageNumber != mes.myMessageInfo.number)
            //{
            //    Debug.LogError("メッセージずれてるクマ。前回のメッセージが受け取れてない可能性があるクマ");
            //    //oulFile.OutPutLog(Application.dataPath + "/log.txt", "メッセージずれてるクマ。前回のメッセージが受け取れてない可能性があるクマ\n");

            //    for (int i = receiveMessageNumber; i < mes.myMessageInfo.number; i++)
            //    {
            //        ReMessageInfo info = new ReMessageInfo();
            //        info.messageNumber = i;
            //        MessageInfo re = new MessageInfo();
            //        re.messageType = MessageType.ReMessage;
            //        re.SetExtraInfo(info);
            //        SendMessage(re);
            //    }
            //}
            //else receiveMessageNumber++;

            // Msg.Textを送ってきたサーバーに返信する
            //if (mes.myMessageInfo.messageType != MessageType.ReceiveOK)
            //{
            //    MyNetworkMessage re = new MyNetworkMessage();
            //    re.myMessageInfo.messageType = MessageType.ReceiveOK;
            //    ReceiveOKInfo exInfo = new ReceiveOKInfo();
            //    exInfo.iMessageType = (uint)mes.myMessageInfo.messageType;
            //    re.myMessageInfo.SetExtraInfo(exInfo);
            //    networkMessage.conn.Send(Msg.MyMessage, re);
            //}
            MessageManager.ReceiveNetwork(mes.myMessageInfo);
        });

        Debug.Log("クライアント開始キテルグマ");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        clientConnections.Add(conn);

        Debug.Log("OnClientConnect");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        serverConnections.Add(conn);

        // クライアント登録
        //ClientMessageOKInfo info = new ClientMessageOKInfo();
        //info.connectionId = conn.connectionId;
        //info.sendOK = false;
        //clientIDList.Add(info);

        Debug.Log("OnServerConnect: クライアント" + conn.connectionId + "と接続シテルグマ");
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        clientConnections.Clear();
        serverConnections.Clear();

        Debug.Log("OnStopHost");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        serverConnections.Clear();

        Debug.Log("OnStopClient");
    }

    public void Spawn()
    {
        ClientScene.AddPlayer(0);
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

    public int GetNumClientConnections() { return clientConnections.Count; }
    public int GetNumServerConnections() { return serverConnections.Count; }
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
    public static BaseNetworkScene networkScene;    // ゲーム管理さん
    public static bool isNetwork;
    public static bool isServer = false;

    //static readonly float kankaku = 0.1f;   // おくる間隔
    //static float timer;
    static bool messageSyoriSitemoii;

    static int receiveMessageNumber;


    public static void SetMessageSyoriSitemoii(bool value) { messageSyoriSitemoii = value; }

    static List<MessageInfo> messageBox = new List<MessageInfo>();

    public static void Start(bool network)
    {
        isNetwork = network;
        Restart();
    }

    public static void SetNetworkScene(BaseNetworkScene scene) { networkScene = scene; }

    public static void Restart()
    {
        messageSyoriSitemoii = true;
        receiveMessageNumber = 0;
        messageBox.Clear();
    }

    public static void ReceiveOffline(MessageInfo message)
    {
        // 受診
        networkScene.HandleMessage(message);
    }

    // ネットからの情報を受信
    public static void ReceiveNetwork(MessageInfo message)
    {
        //Debug.Log("受信:" + message.messageType.ToString());

        //if (message.messageType == MessageType.ReceiveOK)
        //{
        //    // byte[]→構造体
        //    ReceiveOKInfo receiveOKInfo = new ReceiveOKInfo();
        //    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(receiveOKInfo));
        //    Marshal.Copy(message.exInfo, 0, ptr, Marshal.SizeOf(receiveOKInfo));
        //    receiveOKInfo = (ReceiveOKInfo)Marshal.PtrToStructure(ptr, receiveOKInfo.GetType());
        //    Marshal.FreeHGlobal(ptr);

        //    for (int i = 0; i < messageQueue.Count; i++)
        //    {
        //        if(messageQueue[i].messageType != MessageType.ReceiveOK &&
        //            messageQueue[i].messageType == (MessageType)receiveOKInfo.iMessageType)
        //        {
        //            messageQueue.Remove(messageQueue[i]);
        //            break;
        //        }
        //    }
        //    return;
        //}

        // メッセージボックスにつっこむ
        messageBox.Add(message);

        // 受け取ったよメッセージを送る
        //MessageInfo info = new MessageInfo();
        //info.messageType = MessageType.ReceiveOK;
        //ReceiveOKInfo exInfo = new ReceiveOKInfo();
        //exInfo.iMessageType = (uint)message.messageType;
        //info.SetExtraInfo(exInfo);
        //Send(info);
        //if (messageQueue.Count > 0)
        //{
        //    messageQueue.Insert(0, info);
        //}
        //else messageQueue.Add(info);

    }

    public static void Update()
    {
        // 処理待ち中は無視
        if (!messageSyoriSitemoii) return;
        // メッセージ何もなかったら
        if (messageBox.Count == 0) return;
        // 一定間隔(Remessage送りすぎないように)
        //if ((timer += Time.deltaTime) < kankaku)
        //{
        //    // 通信中UI表示
        //    sceneMain.uiManager.AppearConnectingUI();
        //    return;
        //}
        // 通信中UI非表示
        //sceneMain.uiManager.DisAppearConnectingUI();

        //timer = 0;

        bool ok = false;
        // ずれがないように順番通りに処理するようにする
        for (int i = 0; i < messageBox.Count; i++)
        {
            if(messageBox[i].number == receiveMessageNumber)
            {
                var message = messageBox[i];
                // 処理したのでリストから消去
                messageBox.Remove(message);
                // 次のメッセージ待機
                receiveMessageNumber++;
                // 受け取ったことにする
                networkScene.HandleMessage(message);
                // 見つかったフラグ
                ok = true;
                break;
            }
        }

        if (!ok)
        {
            Debug.LogError("メッセージずれてるクマ。前回のメッセージが受け取れてない可能性があるクマ");
            //oulFile.OutPutLog(Application.dataPath + "/log.txt", "メッセージずれてるクマ。前回のメッセージが受け取れてない可能性があるクマ\n");

            // もっかい送ってもらう
            ReMessageInfo info = new ReMessageInfo();
            info.messageNumber = receiveMessageNumber;
            MessageInfo re = new MessageInfo();
            re.messageType = MessageType.ReMessage;
            re.SetExtraInfo(info);
            oulNetwork.s_Singleton.SendMessage(re);

            //for (int i = receiveMessageNumber; i < mes.myMessageInfo.number; i++)
            //{
            //    ReMessageInfo info = new ReMessageInfo();
            //    info.messageNumber = i;
            //    MessageInfo re = new MessageInfo();
            //    re.messageType = MessageType.ReMessage;
            //    re.SetExtraInfo(info);
            //    SendMessage(re);
            //}
        }

        //if (messageBox[0].messageType == MessageType.ReceiveOK)
        //{
        //    Send(messageBox[0]);
        //    return;
        //}

        //if ((timer += Time.deltaTime) > kankaku)
        //{
        //    timer = 0;

        //    Send(messageBox[0]);
        //}
    }

    // ネットに送信する
    public static void Send(MessageInfo message)
    {
        Debug.Log("送信:" + message.messageType.ToString());

        // ネットワークかそうでないかで分岐
        if (isNetwork)
        {
            // サーバにMsg.Textを送る
            // ※自分がサーバーなら送らず内部で処理する
            //if(!isServer)
                oulNetwork.s_Singleton.SendMessage(message);
            //ReceiveNetwork(message);
        }

        else
        {
            ReceiveOffline(message);
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

        //if (isNetwork)
        //    messageQueue.Add(message);
    }

    public static void DispatchOffline(int fromPlayerID, MessageType messageType, object extraInfo)
    {
        // 
        MessageInfo message = new MessageInfo();
        message.messageType = messageType;
        message.fromPlayerID = fromPlayerID;
        if (extraInfo != null)
            message.SetExtraInfo(extraInfo);

        //Send(message);

        ReceiveOffline(message);

        //if (isNetwork)
        //    messageQueue.Add(message);
    }
}