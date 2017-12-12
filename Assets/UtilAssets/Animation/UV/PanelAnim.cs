using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    private const int iNameHashTag = 0;// 決め打ちで0ハッシュタグ0番目のアニメーションを発動

    // Use this for initialization
    public void Awake()
    {
        // 最初は描画しないようにする
        // gameObject.SetActive(false);

        // [1208]最初のActive呼ぶ時毎回呼ばれるのでなし
    }

    protected virtual void Update()
    {
        // アニメーション再生が最後までいったら削除
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void Action(Vector3 vPos, Vector3 vAngle)
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = vPos;
        gameObject.transform.localEulerAngles = vAngle;
        oulMath.Billboard(gameObject.transform);
        gameObject.GetComponent<Animator>().Play(iNameHashTag, 0, 0.0f);

    }
    public void Action2D(Vector3 vPos)// アングルもつけてよし
    {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = vPos;
        //gameObject.transform.localEulerAngles = vAngle;
        //oulMath.Billboard(gameObject.transform);
        gameObject.GetComponent<Animator>().Play(iNameHashTag, 0, 0.0f);

    }

}
