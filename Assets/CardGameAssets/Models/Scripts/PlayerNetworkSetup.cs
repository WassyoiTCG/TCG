using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;	//ネットワーク関連で必要なライブラリー

public class PlayerNetworkSetup : NetworkBehaviour
{
    // Use this for initialization
    void Start()
    {
        //自分が操作するオブジェクトに限定する
        if (isLocalPlayer)
        {
            GetComponent<Player>().enabled = true;

            ////FirstPersonControllerをアクティブ化
            //GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            ////FirstPersonCharacterの各コンポーネントをアクティブ化
            //FPSCharacterCam.enabled = true;
            //audioListener.enabled = true;
        }
    }
}
