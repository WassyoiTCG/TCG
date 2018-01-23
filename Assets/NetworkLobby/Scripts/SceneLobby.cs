using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLobby : BaseNetworkScene
{
    // ステート
    public BaseEntityStateMachine<SceneLobby> stateMachine;

    // ネットワーク
    public oulNetwork networkManager;

    // Use this for initialization
    void Start()
    {
        // 追加1126 システムの初期化をする(WinMainのInitApp)
        oulSystem.Initialize();

        // ネットワークメッセージ初期化
        MessageManager.Start(SelectData.isNetworkBattle);
        MessageManager.SetNetworkScene(this);

        // ネットワークオブジェクト取得
        networkManager = GameObject.Find("NetworkManager").GetComponent<oulNetwork>();

        // ホストかクライアントかを起動
        if(SelectData.networkType == NETWORK_TYPE.HOST)
        {
            networkManager.StartHost();
        }
        if(SelectData.networkType == NETWORK_TYPE.CLIENT)
        {
            networkManager.StartClient2(PlayerDataManager.GetPlayerData().ip);
        }

        // ステート初期化
        // ステートマシンの初期化や切り替えは最後に行う
        stateMachine = new BaseEntityStateMachine<SceneLobby>(this);

        stateMachine.globalState = SceneLobbyState.Global.GetInstance();
        stateMachine.ChangeState(SceneLobbyState.WaitConnection.GetInstance());
    }
	
	// Update is called once per frame
	void Update ()
    {
        stateMachine.Update();
	}

    public override void HandleMessage(MessageInfo message)
    {
        stateMachine.HandleMessage(message);
    }
}
