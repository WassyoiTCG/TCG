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

    public bool isServer = false;
    List<ClientMessageOKInfo> clientIDList = new List<ClientMessageOKInfo>();
    List<MyNetworkMessage> messageQueue = new List<MyNetworkMessage>();

    float waitTimer;
    int sendMessageNumber;
    //int receiveMessageNumber;

    MessageInfo[] messageBuffer = new MessageInfo[2048];

    class ClientMessageOKInfo
    {
        public int connectionId;   // 接続してるクライアントのID
        public bool sendOK;        // 正しくメッセージが送れたか
    }


    void Start()
    {
        s_Singleton = this;
        Restart();
        //DontDestroyOnLoad(gameObject);
    }

    public void Restart()
    {
        sendMessageNumber = 0;
        //receiveMessageNumber = 0;
        for (int i = 0; i < messageBuffer.Length; i++)
        {
            messageBuffer[i] = new MessageInfo();
            messageBuffer[i].messageType = MessageType.NoMessage;
        }
    }

    bool isAllSendOK()
    {
        foreach(ClientMessageOKInfo c in clientIDList)
        {
            if (!c.sendOK) return false;
        }
        return true;
    }

    void Update()
    {
        if (!isServer) return;

        if(messageQueue.Count > 0)
        {
            // 全員に送れたら
            if (isAllSendOK())
            {
                // フラグリセット
                foreach (ClientMessageOKInfo c in clientIDList)
                {
                    c.sendOK = false;
                }
                // 先頭メッセージ削除
                messageQueue.RemoveAt(0);
                return;
            }

            if ((waitTimer += Time.deltaTime) > 0.5f)
            {
                foreach (ClientMessageOKInfo c in clientIDList)
                {
                    if (!c.sendOK) NetworkServer.SendToClient(c.connectionId, Msg.MyMessage, messageQueue[0]);
                }
            }


        }
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
        isServer = true;

        // サーバがMsg.Textを受信したときに行う関数を登録する
        NetworkServer.RegisterHandler(Msg.MyMessage, networkMessage =>
        {
            var mes = networkMessage.ReadMessage<MyNetworkMessage>();
            Debug.Log("サーバー: " + networkMessage.conn.connectionId + "番のプレイヤーからのメッセージを受信" + mes.myMessageInfo.messageType.ToString());

            // 
            if (mes.myMessageInfo.messageType == MessageType.ReceiveOK)
            {
                // byte[]→構造体
                ReceiveOKInfo info = new ReceiveOKInfo();
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                Marshal.Copy(mes.myMessageInfo.exInfo, 0, ptr, Marshal.SizeOf(info));
                info = (ReceiveOKInfo)Marshal.PtrToStructure(ptr, info.GetType());
                Marshal.FreeHGlobal(ptr);

                for (int i = 0; i < clientIDList.Count; i++)
                {
                    if (clientIDList[i].connectionId == networkMessage.conn.connectionId)
                    {
                        clientIDList[i].sendOK = true;
                        break;
                    }
                }
                return;
            }
            else if(mes.myMessageInfo.messageType == MessageType.ReMessage)
            {
                // byte[]→構造体
                ReMessageInfo reMessageInfo = new ReMessageInfo();
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(reMessageInfo));
                Marshal.Copy(mes.myMessageInfo.exInfo, 0, ptr, Marshal.SizeOf(reMessageInfo));
                reMessageInfo = (ReMessageInfo)Marshal.PtrToStructure(ptr, reMessageInfo.GetType());
                Marshal.FreeHGlobal(ptr);

                // 返信
                MyNetworkMessage re = new MyNetworkMessage();
                re.myMessageInfo = messageBuffer[reMessageInfo.messageNumber];
                Debug.Assert(re.myMessageInfo.messageType != MessageType.NoMessage, "ReMessageでエラー");
                networkMessage.conn.Send(Msg.MyMessage, re);
            }

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
            NetworkServer.SendToAll(Msg.MyMessage, mes);

            // おくられてないとき用にメッセージ保存
            //messageQueue.Add(mes);
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

    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);
    //}

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        // クライアント登録
        ClientMessageOKInfo info = new ClientMessageOKInfo();
        info.connectionId = conn.connectionId;
        info.sendOK = false;
        clientIDList.Add(info);

        Debug.Log("サーバー: クライアント" + conn.connectionId + "と接続シテルグマ");
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

    static readonly float kankaku = 0.1f;   // おくる間隔
    static float timer;
    static bool messageSyoriSitemoii;

    static int receiveMessageNumber;

    public static void SetMessageSyoriSitemoii(bool value) { messageSyoriSitemoii = value; }

    static List<MessageInfo> messageBox = new List<MessageInfo>();

    public static void Start(SceneMain main, bool network)
    {
        sceneMain = main;
        isNetwork = network;
        Restart();
    }

    public static void Restart()
    {
        messageSyoriSitemoii = true;
        receiveMessageNumber = 0;
        messageBox.Clear();
    }

    public static void ReceiveOffline(MessageInfo message)
    {
        // 受診
        sceneMain.HandleMessage(message);
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
        if ((timer += Time.deltaTime) < kankaku)
        {
            // 通信中UI表示
            sceneMain.uiManager.AppearConnectingUI();
            return;
        }
        // 通信中UI非表示
        sceneMain.uiManager.DisAppearConnectingUI();

        timer = 0;

        bool ok = false;
        // ずれがないように順番通りに処理するようにする
        for (int i = 0; i < messageBox.Count; i++)
        {
            if(messageBox[i].number == receiveMessageNumber)
            {
                // 受け取ったことにする
                sceneMain.HandleMessage(messageBox[i]);
                // 処理したのでリストから消去
                messageBox.RemoveAt(i);
                // 次のメッセージ待機
                receiveMessageNumber++;
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
            oulNetwork.s_Singleton.SendMessage(message);
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
}