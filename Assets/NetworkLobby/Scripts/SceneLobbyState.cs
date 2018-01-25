using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLobbyState
{

    //+-------------------------------------------
    //  グローバルステート
    //+-------------------------------------------
    public class Global : BaseEntityState<SceneLobby>
    {
        static Global m_pInstance;
        public static Global GetInstance() { if (m_pInstance == null) { m_pInstance = new Global(); } return m_pInstance; }

        public override void Enter(SceneLobby e)
        {

        }

        public override void Execute(SceneLobby e)
        {

        }

        public override void Exit(SceneLobby e)
        {

        }

        public override bool OnMessage(SceneLobby e, MessageInfo message)
        {
            switch(message.messageType)
            {
                case MessageType.SyncName:
                    {
                        // 自分から送られたならスルー
                        if ((NETWORK_TYPE)message.fromPlayerID == e.networkType) return true;

                        // データ解凍
                        SyncNameInfo info = new SyncNameInfo();
                        message.GetExtraInfo<SyncNameInfo>(ref info);

                        //Debug.Log("相手の名前が送られてきた - " + info.playerName);

                        char[] charArray = new char[64];
                        for (int i = 0; i < 64; i++)
                        {
                            charArray[i] = Convert.ToChar(info.iName[i]);
                        }

                        if ((NETWORK_TYPE)message.fromPlayerID == NETWORK_TYPE.HOST)
                        {
                            e.hostWindow.SetPlayerName(new string(charArray));
                            e.hostWindow.SetPlayerActive(true);
                        }
                        if ((NETWORK_TYPE)message.fromPlayerID == NETWORK_TYPE.CLIENT)
                        {
                            e.clientWindow.SetPlayerName(new string(charArray));
                            e.clientWindow.SetPlayerActive(true);
                        }
                    }
                    return true;
                case MessageType.ClickJunbiOK:
                    if((NETWORK_TYPE)message.fromPlayerID == NETWORK_TYPE.HOST)
                    {
                        e.hostWindow.SetJunbiOK(true);
                    }
                    if((NETWORK_TYPE)message.fromPlayerID == NETWORK_TYPE.CLIENT)
                    {
                        e.clientWindow.SetJunbiOK(true);
                    }
                    return true;
            }

            return false;
        }

    }

    //+-------------------------------------------
    //  相手の接続待ち
    //+-------------------------------------------
    public class WaitConnection : BaseEntityState<SceneLobby>
    {
        static WaitConnection m_pInstance;
        public static WaitConnection GetInstance() { if (m_pInstance == null) { m_pInstance = new WaitConnection(); } return m_pInstance; }

        public override void Enter(SceneLobby e)
        {

        }

        public override void Execute(SceneLobby e)
        {
            if (SelectData.networkType == NETWORK_TYPE.HOST)
            {
                if (e.networkManager.GetNumServerConnections() >= 2)
                {
                    // 誰かと繋がった
                    e.stateMachine.ChangeState(OKClick.GetInstance());
                }
            }
            if (SelectData.networkType == NETWORK_TYPE.CLIENT)
            {
                // サーバーと繋がったら
                if (e.networkManager.GetNumClientConnections() >= 1)
                {
                    // 誰かと繋がった
                    e.stateMachine.ChangeState(OKClick.GetInstance());
                }
            }
        }

        public override void Exit(SceneLobby e)
        {

        }

        public override bool OnMessage(SceneLobby e, MessageInfo message)
        {
            return false;
        }

    }

    public class OKClick : BaseEntityState<SceneLobby>
    {
        static OKClick m_pInstance;
        public static OKClick GetInstance() { if (m_pInstance == null) { m_pInstance = new OKClick(); } return m_pInstance; }

        public override void Enter(SceneLobby e)
        {
            // シーンメインにいく
            //SceneManager.LoadScene("Main");

            // 自分の名前を相手に送る
            SyncNameInfo info = new SyncNameInfo();
            info.iName = new int[64];
            char[] cName = PlayerDataManager.GetPlayerData().playerName.ToCharArray();
            for (int i = 0; i < cName.Length; i++)
            {
                // char型なのに12450という値が出た("ア")。c#のchar型は自分の知っているchar型ではない？
                //if(cName[i] > byte.MaxValue)
                //{
                //    Debug.Log((int)cName[i] + "," + cName[i]);
                //}
                //info.byName[i] = Convert.ToByte(cName[i]);
                info.iName[i] = Convert.ToInt32(cName[i]);
                //if (cName[i] == '\0')break;
            }
            //info.playerName = PlayerDataManager.GetPlayerData().playerName;
            //info.iconID = PlayerDataManager.GetPlayerData().iconNo;
            MessageManager.Dispatch((int)e.networkType, MessageType.SyncName, info);

            //Debug.Log("相手に自分の名前を送ります！ - " + info.playerName);

            // 準備OKのボタンを表示する
            e.junbiOKButton.SetActive(true);
        }

        public override void Execute(SceneLobby e)
        {
            // 両方準備OKを押したらシーンメインにいく
            if(e.hostWindow.isPushedJunbiOK() && e.clientWindow.isPushedJunbiOK())
            {
                SceneManager.LoadScene("Main");
            }
        }

        public override void Exit(SceneLobby e)
        {

        }

        public override bool OnMessage(SceneLobby e, MessageInfo message)
        {
            return false;
        }

    }
}