using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Canvas canvas;

    //public Image cardFrameImage;
    //public GameObject fighterPowerFrame;
    public SpriteRenderer fighterPowerFrame;
    //public Text powerText;
    public Number powerNumber;
    public Text cardNameText;
    public Text syuzokuText;
    //public Image cardImage;
    public MeshRenderer cardFrameRenderer;
    public MeshRenderer cardSleeveRenderer;
    public MeshRenderer cardImageRenderer;

    public bool isSet;  // セットされているフラグ
    public bool isHold; // 掴んでるフラグ

    public Vector3 handPosition;    // 手札の座標保存用

    public Transform cashTransform;

    public CardData cardData { get; private set; }

    private void Awake()
    {
        isSet = false;
        isHold = false;
        //canvas = GetComponent<Canvas>();
        cashTransform = transform;
    }

    public void SetOrder(int no)
    {
        fighterPowerFrame.sortingOrder = no + 1;
        powerNumber.SetOrder(no + 2);
        cardFrameRenderer.sortingOrder = no;
        cardSleeveRenderer.sortingOrder = no;
        cardImageRenderer.sortingOrder = no;
        canvas.sortingOrder = no;
    }

    public void SetCardData(CardData data)
    {
        cardData = data;

        // カード名
        cardNameText.text = data.cardName;
        // カード画像
        //cardSprite.sprite = data.image;
        cardImageRenderer.materials[0].SetTexture("_MainTex", data.image); 

        switch (data.cardType)
        {
            case CardType.Fighter:
            case CardType.AbilityFighter:
                {
                    //fighterPowerFrame.SetActive(true);

                    var fighter = data.GetFighterCard();
                    // パワー
                    powerNumber.SetNumber(data.power);
                    // 種族
                    syuzokuText.text = CardDataBase.SyuzokuString[(int)fighter.syuzokus[0]];
                    for (int i = 1; i < fighter.syuzokus.Length; i++)
                        syuzokuText.text += " / " + CardDataBase.SyuzokuString[(int)fighter.syuzokus[i]];
                }
                break;

            case CardType.Joker:
                //fighterPowerFrame.SetActive(false);
                powerNumber.SetNumber(0);
                break;

            case CardType.Event:
                //fighterPowerFrame.SetActive(false);
                powerNumber.SetNumber(data.power);
                break;
        }
    }

    public void SetUraomote(bool omote)
    {
        if (omote)
        {
            fighterPowerFrame.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);
        }
        else
        {
            fighterPowerFrame.gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
        }
    }

    // カードを出す
    //public void SendCard()
    //{
    //    // カードの位置をセット位置にする
    //    cashTransform.localPosition = strikerSetPosition;

    //    isSet = true;
    //}
}
