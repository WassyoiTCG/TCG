using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkErrorWindow : MonoBehaviour
{
    public void Action() { gameObject.SetActive(true); }
    public void Stop() { gameObject.SetActive(false); }
    public void ClickOK()
    {
        // 自身を非表示に
        Stop();

        // シーンメニューに戻る
        SceneManager.LoadScene("Menu");
    }
}
