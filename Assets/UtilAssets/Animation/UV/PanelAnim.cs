using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    // Use this for initialization
    public void Awake()
    {
        // 最初は描画しないようにする
        gameObject.SetActive(false);
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

}
