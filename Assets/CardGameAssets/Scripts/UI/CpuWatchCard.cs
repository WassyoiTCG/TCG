using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuWatchCard : MonoBehaviour {

    //const int ABILITY_MAX = 4;

    // (複製元のプレハブ)
    public uGUICard prefab;
    //
    uGUICard[] CpuAbilityCards = new uGUICard[PlayerDeckData.MaxAbilityStrikerNom];

    // リストの理由は　4枚カード描画できるけど相手が３枚しか効果持ってなかった時
    List<CardData> AbilityCards;

    // 場所
    readonly Vector2 offset = new Vector2(-370, 0);
    readonly float width = 250;
    readonly float scale = 0.5f;

    // Use this for initialization
    void Awake () {

        // キャッシュ
        var cache = transform;

        // 初期化
        for (int i = 0; i < CpuAbilityCards.Length; i++)
        {
            CpuAbilityCards[i] = Instantiate(prefab, cache);
            CpuAbilityCards[i].transform.localPosition = new Vector3(offset.x + i * width, offset.y);
            CpuAbilityCards[i].transform.localScale = new Vector3(scale, scale, 1);
            CpuAbilityCards[i].gameObject.SetActive(false);
        }

    }

    public void Restart()
    {
        foreach (uGUICard card in CpuAbilityCards)
        {
            if (card) card.gameObject.SetActive(false);
        }
    }

    // 付ける 
    public void Appear(List<CardData> cpuAbilityCards)
    {
        gameObject.SetActive(true);
        AbilityCards = cpuAbilityCards;

        // 相手の効果表示
        // データセット
        SetDatas(AbilityCards);


    }

    // カードデータをセットしていくやで
    public void SetDatas(List<CardData> datas)
    {
        // パワー順にソートする
        datas.Sort(((a, b) => a.power - b.power));

        for (int i = 0; i < PlayerDeckData.MaxAbilityStrikerNom; i++)
        {
            if (i >= datas.Count)
            {
                CpuAbilityCards[i].gameObject.SetActive(false);
            }
            else
            {
                CpuAbilityCards[i].SetCardData(datas[i]);
                CpuAbilityCards[i].gameObject.SetActive(true);
            }
        }
    }

    // 閉じる
    public void DisAppear()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
