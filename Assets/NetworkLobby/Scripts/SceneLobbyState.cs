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
            SceneManager.LoadScene("Main");
        }

        public override void Execute(SceneLobby e)
        {

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