using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CemeteryInfomationUI : MonoBehaviour
{
    // プレハブ
    public uGUICard prefab;

    public Image cemeteryInfoFrame;             // 墓地追放情報の枠
    public GameObject cemeteryButton;           // 墓地ボタン
    public GameObject expulsionButton;          // 追放ボタン
    public Sprite spriteCemeteryFrame;          // 墓地の枠
    public Sprite spriteExpulsionFrame;         // 追放の枠
    public Sprite spriteCemeteryON;             // 墓地ON
    public Sprite spriteCemeteryOFF;            // 墓地OFF
    public Sprite spriteExpulsionON;            // 追放ON
    public Sprite spriteExpulsionOFF;           // 追放OFF

    uGUICard[] uguiCards = new uGUICard[15];
    List<CardData> cemeteryCards;
    CardData[] expulsionCards;

    const float offsetX = -380;
    const float offsetY = 40;
    const float kankakuX = 147;
    const float kankakuY = 100;
    const float scale = 0.3f;

    const float cardWidth = 488 * scale;
    const float cardHeight = 640 * scale;

    void Awake()
    {
        // キャッシュ
        var cache = transform;

        for (int i = 0; i < uguiCards.Length; i++)
        {
            uguiCards[i] = Instantiate(prefab, cache);
            uguiCards[i].transform.localPosition = new Vector3(offsetX + (i % 6) * kankakuX, offsetY - (i / 6) * kankakuY);
            uguiCards[i].transform.localScale = new Vector3(scale, scale, 1);
            uguiCards[i].gameObject.SetActive(false);
        }
    }

    public void Restart()
    {
        foreach(uGUICard card in uguiCards)
        {
            if(card)card.gameObject.SetActive(false);
        }
    }

    public void Appear(List<CardData> cemetery, CardData[] expulsion)
    {
        gameObject.SetActive(true);
        cemeteryCards = cemetery;
        expulsionCards = expulsion;
        // 墓地表示
        ShowCemeteryInfo();
    }
    public void ShowCemeteryInfo()
    {
        // 墓地データセット
        SetDatas(cemeteryCards);

        cemeteryInfoFrame.sprite = spriteCemeteryFrame;

        cemeteryButton.GetComponent<Image>().sprite = spriteCemeteryON;
        // 描画順変更
        expulsionButton.transform.SetSiblingIndex(1);
        expulsionButton.GetComponent<Image>().sprite = spriteExpulsionOFF;
    }
    public void ShowExplusionInfo()
    {
        // 追放データセット
        SetDatas(expulsionCards);

        cemeteryInfoFrame.sprite = spriteExpulsionFrame;

        cemeteryButton.GetComponent<Image>().sprite = spriteCemeteryOFF;
        // 描画順変更
        cemeteryButton.transform.SetSiblingIndex(1);
        expulsionButton.GetComponent<Image>().sprite = spriteExpulsionON;
    }

    public void DisAppear()
    {
        gameObject.SetActive(false);
    }

    public void SetDatas(CardData[] datas)
    {
        // パワー順にソートする
        List<CardData> list = new List<CardData>();
        //foreach (CardData card in datas) list.Add(card);
        list.AddRange(datas);
        //list.Sort(((a, b) => a.power - b.power));

        //for (int i = 0; i < list.Count - 1; i++)
        //{
        //    for (int j = i + 1; j < list.Count; j++)
        //    {
        //        if (!list[j].isStrikerCard()) continue;
        //        if(list[i].power > list[j].power)
        //        {
        //            list.Sort();
        //        }
        //    }

        //}

        for (int i = 0; i < 15; i++)
        {
            if (i >= list.Count)
            {
                uguiCards[i].gameObject.SetActive(false);
            }
            else
            {
                uguiCards[i].SetCardData(list[i]);
                uguiCards[i].gameObject.SetActive(true);
            }
        }
    }

    public void SetDatas(List<CardData> datas)
    {
        // パワー順にソートする
        //datas.Sort(((a, b) => a.power - b.power));

        for (int i = 0; i < 15; i++)
        {
            if (i >= datas.Count)
            {
                uguiCards[i].gameObject.SetActive(false);
            }
            else
            {
                uguiCards[i].SetCardData(datas[i]);
                uguiCards[i].gameObject.SetActive(true);
            }
        }
    }

    public uGUICard CollisionCards(Vector3 mousePosition)
    {
        //mousePosition.y = -mousePosition.y;

        float cardX, cardY;

        foreach(uGUICard card in uguiCards)
        {
            if(card.gameObject.activeSelf)
            {
                cardX = card.transform.position.x;
                cardY = card.transform.position.y;

                // x判定
                if (mousePosition.x < cardX - cardWidth / 2 || mousePosition.x > cardX + cardWidth / 2) continue;

                Debug.Log(mousePosition + "," + cardY + "," + cardHeight / 2);

                // y判定
                if (mousePosition.y < cardY - cardHeight / 2 || mousePosition.y > cardY + cardHeight / 2) continue;

                // ここまで来たら当たってるので、そのカードを返す



                return card;
            }
        }

        // ヒットしていない
        return null;
    }
}
