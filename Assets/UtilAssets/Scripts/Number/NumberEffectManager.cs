using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberEffectManager : MonoBehaviour
{
    public enum NumberEffectType
    {
        Jump,
    }

    public enum ColorType
    {
        White,
        Red,
        Green,
        Blue,
        Cyan,
        YellowGreen,
        Orange
    }

    // Use this for initialization
    void Start()
    {
        // プール配列確保
        var objectPoller = GetComponent<ObjectPoller>();
        objectPoller.poolDatas = new ObjectPoolData[System.Enum.GetValues(typeof(NumberEffectType)).Length];

        GameObject[] roots = new GameObject[System.Enum.GetValues(typeof(NumberEffectType)).Length];
        for (int i = 0; i < System.Enum.GetValues(typeof(NumberEffectType)).Length; i++)
        {
            // 数字を動かす用のルート(親)を作成
            roots[i] = new GameObject();
            roots[i].name = ((NumberEffectType)i).ToString();
            roots[i].transform.SetParent(transform, false);

            // 動きを設定
            switch ((NumberEffectType)i)
            {
                case NumberEffectType.Jump:
                    roots[i].AddComponent<NumberJump>();
                    break;

            }

            // 動かす親にくっつける数字オブジェクトを生成
            var numberObject =
                transform.parent.Find("NumberCreater")
                //GameObject.Find("Util/NumberCreater")
                .GetComponent<NumberCreater>().CreateNumber(/*scale*/1, /*width*/1, new Vector2(0, 0), NumberCreater.FontType.NumberFont0);
            numberObject.transform.SetParent(roots[i].transform, false);

            // プールの元オブジェクト設定(これをもとに設定数ぶん生成する)
            objectPoller.poolDatas[i].originalObject = roots[i];
        }

        // 何個まで出すか
        objectPoller.poolDatas[(int)NumberEffectType.Jump].poolObjects = new GameObject[16];

        // 元オブジェクトをすべて設定したので、Instantiateする
        objectPoller.Pool();

        // (TODO)Instantiateしたオブジェクトのスクリプトの変数が複製元と違う事態が発生したので急きょゴリくん
        for(int i=0;i< System.Enum.GetValues(typeof(NumberEffectType)).Length; i++)
        {
            var scale = 0.5f;
            var width = 0.5f;
            switch ((NumberEffectType)i)
            {
                case NumberEffectType.Jump:
                    break;

                //case NumberEffectType.JumpBig:
                //    scale = 2;
                //    width *= scale;
                //    break;

                //case NumberEffectType.Time:
                //    scale = 1.5f;
                //    width *= (scale * 1.5f);
                //    break;
            }
            for (int j = 0; j < objectPoller.poolDatas[i].poolObjects.Length; j++)
            {
                objectPoller.poolDatas[i].poolObjects[j].transform.GetChild(0).GetComponent<Number>().scale = scale;
                objectPoller.poolDatas[i].poolObjects[j].transform.GetChild(0).GetComponent<Number>().width = width;

                roots[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddNumber(Vector3 position, int number, ColorType colorType, NumberEffectType numberEffectType)
    {
        var root = GetComponent<ObjectPoller>().GetPoolObject((int)numberEffectType);
        if (!root) return;
        var numberObject = root.transform.GetChild(0).gameObject;
        if (!numberObject) return;

        var color = new Color();
        switch (colorType)
        {
            case ColorType.White:
                color = new Color(1, 1, 1);// 色
                break;
            case ColorType.Red:
                color = new Color(1, 0, 0);
                break;
            case ColorType.Green:
                color = new Color(0, 1, 0);
                break;
            case ColorType.Blue:
                color = new Color(0, 0, 1);
                break;
            case ColorType.Cyan:
                color = new Color(0, 1, 1);
                break;
            case ColorType.YellowGreen:
                color = new Color(0.67f, 1, 0.47f);
                break;
            case ColorType.Orange:
                color = new Color(1, 0.5f, 0);
                break;
        }


        // 時間回復なら、頭に時間マークをつける
        //if (numberEffectType == NumberEffectType.Time) numberObject.GetComponent<Number>().SetHeadMark(Number.HeadMark.Time);

        //numberObject.GetComponent<Number>().GetComponent<Number>().SetPosision(position);
        numberObject.GetComponent<Number>().SetColor(color);
        numberObject.GetComponent<Number>().SetNumber(number);

        // 稼働させる
        root.SetActive(true);
        root.GetComponent<NumberEffectBase>().Action(position);
    }
}
