using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    int myPlayerID = -1;                    // 自分のプレイヤーのID

    //public oulNetwork networkManager;
    public UIManager uiManager;

    //public void OnClickNetPlay()
    //{
    //    // ネットフラグOFF
    //    MessageManager.isNetwork = false;
    //    MessageManager.Dispatch(myPlayerID, MessageType.NetPlay, null);

    //    // ネットフラグON
    //    MessageManager.isNetwork = true;
    //    // ボタン非表示
    //    uiManager.DisAppearNetOrOffline();
    //}

    //public void OnClickOfflinePlay()
    //{
    //    // ネットフラグOFF
    //    MessageManager.isNetwork = false;

    //    MessageManager.Dispatch(myPlayerID, MessageType.OfflinePlay, null);

    //    // ボタン非表示
    //    uiManager.DisAppearNetOrOffline();
    //}

    public void OnClickCemeteryButton()
    {                
        // SE
        oulAudio.PlaySE("decide2");
        
        uiManager.cemeteryInfoUIManager.ShowCemeteryInfo();
    }

    public void OnClickExpulsionButton()
    {
        // SE
        oulAudio.PlaySE("decide2");

        Debug.LogWarning("この関数は使用してはいけない");
        //uiManager.cemeteryInfoUIManager.ShowExplusionInfo();
    }

    //public void OnClickHostGame()
    //{
    //    networkManager.StartHost();

    //    // ボタン非表示
    //    uiManager.DisAppearLobby();
    //}

    //public void OnClickClientGame()
    //{
    //    networkManager.StartClient();

    //    // ボタン非表示
    //    uiManager.DisAppearLobby();
    //}

    // マリガンボタンを押したとき
    public void OnClickMarigan()
    {
        // SE
        oulAudio.PlaySE("decide2");

        var playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        if(myPlayerID == -1)
            myPlayerID = playerManager.GetMyPlayerID();

        // とてつもないゴリのような何か
        // マリガンを、新しくシャッフルしたデッキの情報を互いに同期させて7ドローという処理をしているので、
        // ここでもう一度山札をリセットして情報をつっこんでいる
        var player = playerManager.GetMyPlayer();
        player.deckManager.SetDeckData(player.deckData, false);
        var exInfo = player.GetSyncDeckInfo();

        MessageManager.Dispatch(myPlayerID, MessageType.Marigan, exInfo);

        // ボタン非表示
        uiManager.DisAppearFirstDraw();
    }

    // マリガンしないボタンを押したとき
    public void OnClickNoMarigan()
    {
        // SE
        oulAudio.PlaySE("decide2");

        if (myPlayerID == -1)
            myPlayerID = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetMyPlayerID();

        MessageManager.Dispatch(myPlayerID, MessageType.NoMarigan, null);

        // ボタン非表示
        uiManager.DisAppearFirstDraw();
    }

    // 準備完了ボタン押したとき
    public void OnClickSetStrikerOK()
    {
        // SE
        oulAudio.PlaySE("decide3");

        MessageManager.Dispatch(myPlayerID, MessageType.SetStrikerOK, null);
        Debug.Log("ストライカーセットボタンキテルグマ");

        // 別のところでもこのメッセージを送るので、非表示処理はここでしない

    }

    // パスボタン押したとき
    public void OnClickNext()
    {
        // SE
        Debug.Log("ネクストボタン");
        oulAudio.PlaySE("decide3");

        MessageManager.Dispatch(myPlayerID, MessageType.SetStrikerPass, null);

        // 別のところでもこのメッセージを送るので、非表示処理はここでしない

    }

    // 数字選択ボタン押したとき
    public void OnClickNumberButton(int number)
    {
        // SE
        oulAudio.PlaySE("decide2");

        SelectNumberInfo info = new SelectNumberInfo();
        info.selectNumber = number;

        // 数字選択メッセージ送信
        MessageManager.Dispatch(myPlayerID, MessageType.SelectNumber, info);

        // ボタン非表示
        //uiManager.DisAppearSelectNumberUI(number);
    }

    public void OnClickRestart()
    {
        // SE
        oulAudio.PlaySE("decide2");

        MessageManager.Dispatch(myPlayerID, MessageType.Restart, null);

        uiManager.DisAppearEndGameUI();
    }
    public void OnClickEndButton()
    {
        // SE
        oulAudio.PlaySE("decide2");

        // 相手にも送る(1/11両方送ってるので大丈夫)
        //int CPUID = 1 - myPlayerID;
        //MessageManager.Dispatch(CPUID, MessageType.EndGame, null);

        MessageManager.Dispatch(myPlayerID, MessageType.EndGame, null);

        uiManager.DisAppearEndGameUI();
    }
}
