﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SceneLobby : BaseNetworkScene
{
    // ステート
    public BaseEntityStateMachine<SceneLobby> stateMachine;

    // ロビーのウィンドウ
    public LobbyPlayerWindow hostWindow;
    public LobbyPlayerWindow clientWindow;

    // その他UI
    public GameObject junbiOKButton;

    // ネットワーク
    public oulNetwork networkManager;
    public NetworkClient client;
    public NETWORK_TYPE networkType;

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

        networkType = SelectData.networkType;

        // ホストかクライアントかを起動
        if(networkType == NETWORK_TYPE.HOST)
        {
            // ホストウィンドウのプレイヤー名設定
            hostWindow.SetPlayerName(PlayerDataManager.GetPlayerData().playerName);
            // ホストウィンドウにプレイヤー表示
            hostWindow.SetPlayerActive(true);

            // ネットワーク開始
            if(!networkManager.isNetworkActive)client = networkManager.StartHost();
        }
        if(networkType == NETWORK_TYPE.CLIENT)
        {
            // クライアントウィンドウのプレイヤー名設定
            clientWindow.SetPlayerName(PlayerDataManager.GetPlayerData().playerName);
            // クライアントウィンドウにプレイヤー表示
            clientWindow.SetPlayerActive(true);

            // ネットワーク開始
            if(!networkManager.isNetworkActive)client = networkManager.StartClient2(PlayerDataManager.GetPlayerData().ip);
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

    public void ClickBackButton()
    {
        // ホストかクライアントかを閉じる
        if (SelectData.networkType == NETWORK_TYPE.HOST)
        {
            networkManager.StopHost();
        }
        if (SelectData.networkType == NETWORK_TYPE.CLIENT)
        {
            networkManager.StopClient();
        }

        SceneManager.LoadScene("Menu");
    }

    public void ClickOKButton()
    {
        // 押したので非表示
        junbiOKButton.SetActive(false);

        // メッセージ送信
        MessageManager.Dispatch((int)networkType, MessageType.ClickJunbiOK, null);
    }

    public override void HandleMessage(MessageInfo message)
    {
        stateMachine.HandleMessage(message);
    }
}