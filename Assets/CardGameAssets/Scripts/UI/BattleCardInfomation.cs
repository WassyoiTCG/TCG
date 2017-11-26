using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardInfomation : MonoBehaviour
{
    public uGUICard card;
    public GameObject cardInfoUI;
    public Text abilityText;

    public void Action(CardData data)
    {
        gameObject.SetActive(true);
        card.SetCardData(data);
        //card.SetOrder(10);
        cardInfoUI.SetActive(true);
        abilityText.text = data.abilityText;
    }

    public void Stop()
    {
        gameObject.SetActive(false);
        cardInfoUI.SetActive(false);
    }
}
