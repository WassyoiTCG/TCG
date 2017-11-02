using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    public enum SortingType
    {
        Right,  // 右づめ
        Center, // 中央ぞろえ
        Left,   // 左詰め
    }
    public SortingType sortingType;

    ObjectPoller objectPoller;

    //public GameObject imageObject;
    public Sprite[] numimage = new Sprite[10];
    public int orderInLayer = 0;
    public Sprite dot;
    public Sprite[] headMarks = new Sprite[(int)HeadMark.Max];
    List<int> numbers = new List<int>();
    public float width = 1, scale = 1;
    public Color color = new Color(1, 1, 1, 1);
    public int startNumber = 0;
    public enum HeadMark
    {
        None = -1,
        Plus,
        Minus,
        Percent,
        Max,
    }
    HeadMark headMark = HeadMark.None;

    Transform cashTransform;
    bool isStart = false;
    bool isSpriteRenderer = true;
    readonly int dotFlag = -1;

    void Start()
    {
        if (isStart) return;
        isStart = true;

        objectPoller = GetComponent<ObjectPoller>();

        cashTransform = transform;

        // 初期ナンバー設定
        SetNumber(startNumber);
        // カラー設定
        SetColor(color);
    }

    public void SetNumber(int number, bool topZero = true)
    {
        if (!isStart) Start();

        //アクティブだった数字オブジェクトを非アクティブに
        for (int i = 0; i < cashTransform.childCount; i++)
        {
            var obj = cashTransform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                //Destroy(obj);
                obj.SetActive(false);
            }
        }

        Set(number, 0, topZero);
    }

    public void SetNumber(float number, int syousuten, bool topZero = true)
    {
        if (!isStart) Start();

        //アクティブだった数字オブジェクトを非アクティブに
        for (int i = 0; i < cashTransform.childCount; i++)
        {
            var obj = cashTransform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                //Destroy(obj);
                obj.SetActive(false);
            }
        }

        Set(number, 0, topZero, syousuten);
    }

    void Set(int value, float offsetX, bool topZero, int digit = 0)
    {
        // 数字リスト初期化
        numbers.Clear();

        // 桁指定ver
        if (digit > 0)
        {
            for (int i = 0; i < digit; i++)
            {
                numbers.Add(value != 0 ? value % 10 : 0);
                value /= 10;
            }
        }

        // 桁自動指定ver
        else
        {
            digit = value;
            do
            {
                value = digit % 10;
                digit = digit / 10;
                numbers.Add(value);
            } while (digit != 0);
        }

        // 数値の値を元に、数字を表示していく
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            if (!topZero)
            {
                if (numbers[i] == 0) continue;
                else topZero = true;
            }

            InstantiateNumber(i, false, numbers.Count);
        }

        // 数字の頭のマーク
        if(headMark != HeadMark.None)InstantiateNumber(-1, true, numbers.Count);
    }

    void Set(float value, float offsetX, bool topZero, int syousutenn, int digit = 0)
    {
        // 小数点を除く(114.514fなら1000掛けて114514になる)
        int intValue = (int)(value * Mathf.Pow(10, syousutenn));

        // 数字リスト初期化
        numbers.Clear();

        // 桁指定ver
        if (digit > 0)
        {
            digit += syousutenn;

            for (int i = 0; i < digit; i++)
            {
                // 小数点スキップ
                if (i == syousutenn)
                {
                    numbers.Add(dotFlag);
                    continue;
                }

                numbers.Add(intValue != 0 ? intValue % 10 : 0);
                value /= 10;
            }
        }

        // 桁自動指定ver
        else
        {
            digit = intValue;
            for (int i = 0; digit != 0; i++)
            {
                // 小数点スキップ
                if (i == syousutenn)
                {
                    numbers.Add(dotFlag);
                    continue;
                }
                intValue = digit % 10;
                digit = digit / 10;
                numbers.Add(intValue);
            }
        }

        // 数値の値を元に、数字を表示していく
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            if (!topZero)
            {
                if (numbers[i] == 0) continue;
                else topZero = true;
            }

            InstantiateNumber(i, false, numbers.Count);
        }

        // 数字の頭のマーク
        if (headMark != HeadMark.None) InstantiateNumber(-1, true, numbers.Count);
    }

    void InstantiateNumber(int i, bool isHead, int numberCount)
    {
        //複製
        //var scoreimage = (RectTransform)Instantiate(imageObject.gameObject).transform;
        //scoreimage.SetParent(transform, false);
        //scoreimage.localPosition = new Vector2(-width * i, 0);
        //scoreimage.sizeDelta = new Vector2(size, size);
        //scoreimage.GetComponent<Image>().sprite = numimage[numbers[i]];
        //var scoreimage = Instantiate(imageObject.gameObject).transform;
        var numberImage = objectPoller.GetPoolObject().transform;
        if (sortingType == SortingType.Right || numberCount == 1)
            numberImage.localPosition = new Vector2(-width * i, 0);
        else if (sortingType == SortingType.Left)
            numberImage.localPosition = new Vector2(width * i, 0);
        else if (sortingType == SortingType.Center)
            numberImage.localPosition = new Vector2(-width * i + (numberCount * width / 2) - (width / 2), 0);
        numberImage.localScale = new Vector2(scale, scale);
        var spriteRenderer = numberImage.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // .分岐
            if (numbers[i] == dotFlag) spriteRenderer.sprite = dot;
            // 普通の数字
            else spriteRenderer.sprite = isHead ? headMarks[(int)headMark] : numimage[numbers[i]];
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = orderInLayer;
            isSpriteRenderer = true;
            return;
        }
        var image = numberImage.GetComponent<Image>();
        if(image != null)
        {
            // .分岐
            if (numbers[i] == dotFlag) image.sprite = dot;
            // 普通の数字
            else image.sprite = isHead ? headMarks[(int)headMark] : numimage[numbers[i]];
            image.color = color;
            //image.sortingOrder = orderInLayer;
            isSpriteRenderer = false;
            // 画像サイズ
            var rectTransform = (RectTransform)numberImage;
            rectTransform.sizeDelta = new Vector2(image.preferredWidth, image.preferredHeight);
        }
    }

    public void SetTime(float time)
    {
        //いままで表示されてた数字オブジェクト削除
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                //Destroy(obj);
                obj.SetActive(false);
            }
        }

        //int minutes = (frameTime / 60) / 60, second = (frameTime / 60) % 60, frame = frameTime % 60;
        int minutes = (int)(time / 60), second = (int)time % 60, frame = (int)(time * 100) % 100;
        float offsetX = 0;
        Set(frame, offsetX, true, 2);
        offsetX -= width * 2.65f;
        Set(second, offsetX, true, 2);
        offsetX -= width * 2.65f;
        Set(minutes, offsetX, false, 2);
    }

    public void SetPosision(Vector3 position) { cashTransform.localPosition = position; }
    public void SetColor(Color color)
    {
        this.color = color;

        //いままで表示されてた数字オブジェクトの色も変更
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = cashTransform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                if (isSpriteRenderer)
                    obj.GetComponent<SpriteRenderer>().color = color;
                else obj.GetComponent<Image>().color = color;
            }
        }
    }
    public void SetAlpha(float alpha)
    {
        color.a = alpha;

        //いままで表示されてた数字オブジェクトの色も変更
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = cashTransform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                if (isSpriteRenderer)
                    obj.GetComponent<SpriteRenderer>().color = color;
                else obj.GetComponent<Image>().color = color;
            }
        }
    }
    public void SetHeadMark(HeadMark mark) { headMark = mark; }

    public void SetOrder(int no)
    {
        orderInLayer = no;

        //いままで表示されてた数字オブジェクトの色も変更
        for (int i = 0; i < transform.childCount; i++)
        {
            var obj = cashTransform.GetChild(i).gameObject;
            if (!obj) continue;
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                obj.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
            }
        }
    }

    //public Sprite[] numimage;
    //public List<int> number = new List<int>();
    ////スコアを表示するメソッド
    //public void SetScore(int score)
    //{
    //    //いままで表示されてたスコアオブジェクト削除
    //    for(int i=0;i<transform.childCount;i++)
    //    {
    //        var obj = transform.GetChild(i).gameObject;
    //        if (!obj) continue;
    //        if (0 <= obj.name.LastIndexOf("Clone"))
    //        {
    //            Destroy(obj);
    //        }
    //    }

    //    var digit = score;
    //    //要素数0には１桁目の値が格納
    //    number = new List<int>();
    //    do
    //    {
    //        score = digit % 10;
    //        digit = digit / 10;
    //        number.Add(score);
    //    }while (digit != 0);

    //    GameObject.Find("Score").GetComponent<Image>().sprite = numimage[number[0]];
    //    for (int i = 1; i < number.Count; i++)
    //    {
    //        //複製
    //        RectTransform scoreimage = (RectTransform)Instantiate(transform.FindChild("Score").gameObject).transform;
    //        scoreimage.SetParent(transform, false);
    //        scoreimage.localPosition = new Vector2(
    //            scoreimage.localPosition.x - 64 * i,
    //            scoreimage.localPosition.y);
    //        scoreimage.GetComponent<Image>().sprite = numimage[number[i]];
    //    }
    //}


    //[SerializeField]
    //private GameObject showSprite;  // スプライト表示用オブジェクト(プレハブ)

    //// 数字スプライト
    //[SerializeField]
    //private Sprite _0;
    //[SerializeField]
    //private Sprite _1;
    //[SerializeField]
    //private Sprite _2;
    //[SerializeField]
    //private Sprite _3;
    //[SerializeField]
    //private Sprite _4;
    //[SerializeField]
    //private Sprite _5;
    //[SerializeField]
    //private Sprite _6;
    //[SerializeField]
    //private Sprite _7;
    //[SerializeField]
    //private Sprite _8;
    //[SerializeField]
    //private Sprite _9;
    //[SerializeField]
    //private Sprite _Minus;

    //[SerializeField]
    //float width;    // 数字の表示間隔

    //private int showValue;  // 表示する値

    //private GameObject[] numSpriteGird;         // 表示用スプライトオブジェクトの配列
    //private Dictionary<char, Sprite> dicSprite; // スプライトディクショナリ

    //// スプライトディクショナリを初期化する
    //void Awake()
    //{
    //    dicSprite = new Dictionary<char, Sprite>() {
    //        { '0', _0 },
    //        { '1', _1 },
    //        { '2', _2 },
    //        { '3', _3 },
    //        { '4', _4 },
    //        { '5', _5 },
    //        { '6', _6 },
    //        { '7', _7 },
    //        { '8', _8 },
    //        { '9', _9 },
    //        { '-', _Minus },
    //    };
    //}

    //// 表示する値
    //public int Value
    //{
    //    get
    //    {
    //        return showValue;
    //    }
    //    set
    //    {
    //        showValue = value;

    //        // 表示文字列取得
    //        string strValue = value.ToString();

    //        // 現在表示中のオブジェクト削除
    //        if (numSpriteGird != null)
    //        {
    //            foreach (var numSprite in numSpriteGird)
    //            {
    //                Destroy(numSprite);
    //            }
    //        }

    //        // 表示桁数分だけオブジェクト作成
    //        numSpriteGird = new GameObject[strValue.Length];
    //        for (var i = 0; i < numSpriteGird.Length; ++i)
    //        {
    //            // オブジェクト作成
    //            numSpriteGird[i] = Instantiate(
    //                showSprite,
    //                transform.position + new Vector3((float)i * width, 0),
    //                Quaternion.identity) as GameObject;

    //            // 表示する数値指定
    //            numSpriteGird[i].GetComponent<SpriteRenderer>().sprite = dicSprite[strValue[i]];

    //            // 自身の子階層に移動
    //            numSpriteGird[i].transform.parent = transform;
    //        }
    //    }
    //}

}
