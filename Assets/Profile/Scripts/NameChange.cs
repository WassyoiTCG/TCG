using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChange : MonoBehaviour
{
    public InputField nameInputField;

    public void Action(string playerName)
    {
        // 変更前の名前を入力欄に表示
        nameInputField.text = playerName;
        // 自身を表示する
        gameObject.SetActive(true);
    }

    public void Stop()
    {
        string input = nameInputField.text;
        // 何か入力されていたら、プレイヤー名変更
        if (input != "")
        {
            PlayerDataManager.GetPlayerData().playerName = input;
        }

        // 自身を非表示にする
        gameObject.SetActive(false);
    }
}
