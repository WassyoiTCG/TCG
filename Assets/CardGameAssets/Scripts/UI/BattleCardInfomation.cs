using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardInfomation : MonoBehaviour
{
    public uGUICard card;
    //public GameObject cardInfoUI;
    public Image frame;
    public Text abilityText;

    public void Action(CardData data)
    {
        gameObject.SetActive(true);
        card.SetCardData(data);
        //card.SetOrder(10);

        // 効果なしなら表示しない
        if (data.cardType != CardType.Fighter)
        {
            frame.gameObject.SetActive(true);
            abilityText.text = data.abilityText;
        }

    }

    public void Stop()
    {
        gameObject.SetActive(false);
        //cardInfoUI.SetActive(false);
        frame.gameObject.SetActive(false);
    }
}
