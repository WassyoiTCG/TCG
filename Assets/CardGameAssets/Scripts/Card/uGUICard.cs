using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

public class uGUICard : MonoBehaviour
{
    public Image mainTexture;
    public Text cardName;
    public GameObject powerFrame;
    public Number powerNumber;
    public Text syuzokuText;
    public GameObject panel;
    public GameObject DeckSetInfo;
    public GameObject EventFullInfo;
    public GameObject AbilityFullInfo;

    public CardData cardData;// (TODO)カードデータの情報いれるよ

    //+------------------------
    //  new変数
    //+------------------------
    private bool isMyDeck = false;  // このカードはデッキなのかそうでないのか？(デッキ編集用) 
    public bool GetMyDeck() { return isMyDeck; }
    public void SetMyDeck(bool flag) { isMyDeck = flag; }

    private bool isNotGrasp = false;    //  掴めない状態
    public bool GetNotGrasp() { return isNotGrasp; }
    // public void SetNotGrasp(bool flag) { isMyDeck = flag; }
    public void NotGrasp_On()
    {
        isNotGrasp = true;

        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        DeckSetInfo.SetActive(true);
    }
    public void NotGrasp_Off()
    {
        isNotGrasp = false;

        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        DeckSetInfo.SetActive(false);
        EventFullInfo.SetActive(false);
        
        
        AbilityFullInfo.SetActive(false);
    }

    public void EventFullInfo_On()
    {
        //if (isNotGrasp == false)
        {
            isNotGrasp = true;

            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            EventFullInfo.SetActive(true);
        }

    }

    public void EventFullInfo_Off()
    {
        isNotGrasp = false;

        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        EventFullInfo.SetActive(false);
    }

    public void AbilityFullInfo_On()
    {
        //if (isNotGrasp == false)
        {
            isNotGrasp = true;
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            AbilityFullInfo.SetActive(true);
        }
    }

    public void AbilityFullInfo_Off()
    {
        isNotGrasp = false;
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        AbilityFullInfo.SetActive(false);
    }

    // Use this for initialization
    protected virtual void Awake()
    {
        if(DeckSetInfo != null)DeckSetInfo.SetActive(false);
        if(EventFullInfo != null)EventFullInfo.SetActive(false);
        if (AbilityFullInfo != null) AbilityFullInfo.SetActive(false);

    }

    public void SetCardData(CardData data)
    {
        // アクティブ 
        gameObject.SetActive(true);

        cardData = data;
        // 欠番カード
        //if (data.id == (int)IDType.NONE)
        //{
        //    cardName.text = data.cardName;
        //    return;
        //}

        cardName.text = data.cardName;

        mainTexture.sprite = data.image;/*Sprite.Create((Texture2D)data.image, new Rect(0, 0, data.image.width, data.image.height), new Vector2(0.5f, 0.5f));*/

        // パワーのフレームの有り無し
        if (true)
        {
            powerFrame.SetActive(true);
            powerNumber.SetNumber(data.power);
        }
        else powerFrame.SetActive(false);

        // ストライカーのカードだったら種族を
        if (data.isStrikerCard())
        {
            var striker = data.GetFighterCard();
            //if(striker == null)
            //{
            //    return;
            //}
            syuzokuText.text = CardDataBase.SyuzokuString[(int)striker.syuzokus[0]];
            for (int i = 1; i < striker.syuzokus.Length; i++)
                syuzokuText.text += " / " + CardDataBase.SyuzokuString[(int)striker.syuzokus[i]];
        }
        else syuzokuText.text = "";

        // □えうれしあなら消す 
        if (cardData.id == (int)IDType.NONE)
        {
            gameObject.SetActive(false);
        }


    }

    // 無しのカードを入れる
    public void MissingCard()
    {
        CardData data = CardDataBase.GetCardData((int)IDType.NONE);// 欠番 
        // (11/14) -1を入れるとなしカードが欲しい　この関数いらん
        SetCardData(data);

        // □えうれしあを消す 
        gameObject.SetActive(false);

    }

    // 色全データ更新
    public void AlphaSetUpdate()
    {
        //mainTexture.GetComponent<Image>().color = gameObject.GetComponent<Image>().color;
        gameObject.GetComponent<CanvasGroup>().alpha = gameObject.GetComponent<Image>().color.a;
    }

}
