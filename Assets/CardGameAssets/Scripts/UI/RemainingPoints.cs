using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingPoints : MonoBehaviour {


    public const int POINTS_MAX = 10;
    //+-----------------------
    //  メンバ変数
    //+-----------------------
    public GameObject Frame;
    public Image[] Points = new Image[(int)POINTS_MAX];

    public GameObject PointCurrsor;
    public Text TrunText;

    // Use this for initialization
    void Awake () {
        for (int i = 0; i < POINTS_MAX; i++)
        {
            Points[i].transform.localPosition = new Vector2((i % 5) * 128, -(i / 5) * 128);
        }
   
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<BoyonAppeared>().SelfUpdate();
	}

    public void TouchBegan()
    {
        Frame.SetActive(true);
        for (int i = 0; i < POINTS_MAX; i++)
        {
            Points[i].gameObject.SetActive(true);
        }
        gameObject.GetComponent<BoyonAppeared>().Action();
    }

    public void TouchEnded()
    {
        gameObject.GetComponent<BoyonAppeared>().Stop();
        Frame.SetActive(false);
        for (int i = 0; i < POINTS_MAX; i++)
        {
            Points[i].gameObject.SetActive(false);
        }

    }

    // リセット
    public void Reset()
    {
        float l_fCol = 1.0f;
        Color newColor;
        newColor.r = l_fCol;
        newColor.g = l_fCol;
        newColor.b = l_fCol;
        newColor.a = 1.0f;

        // 全部色戻す
        for (int i = 0; i < POINTS_MAX; i++)
        {
            Points[i].color = newColor;
        }

    }

    // まだある(255)
    public void Remaining(int no) 
    {
        //  エラー処理
        if (no >= POINTS_MAX || no < 0)
        {
            Debug.LogWarning("その番号のポイントは存在しない！");
        }

        float l_fCol = 1.0f;
        var newColor = Points[no].color;
        newColor.r = l_fCol;
        newColor.g = l_fCol;
        newColor.b = l_fCol;

        Points[no].color = newColor;


    }

    // もうない(128)
    public void NotRemaining(int point)
    {
        // ゴレイヌ降臨
        int no = point;
        no /= 10;
        no--;

        //  エラー処理
        if (no >= POINTS_MAX || no < 0)
        {
            Debug.LogWarning("その番号のポイントは存在しない！");
        }

        float l_fCol = 0.5f;
        var newColor = Points[no].color;
        newColor.r = l_fCol;
        newColor.g = l_fCol;
        newColor.b = l_fCol;

        Points[no].color = newColor;


    }

    // 今のポイント
    public void SetPointCurrsor(int point)
    {
        // ゴレイヌ降臨
        int no = point;
        no /= 10;
        no--;

        PointCurrsor.transform.localPosition = new Vector2((no % 5) * 128, -(no / 5) * 128);


    }

    public void SetTrunText(int currentTrun)
    {
        // ターン数更新
        TrunText.text = currentTrun.ToString() + "ターン目";
    }

}
