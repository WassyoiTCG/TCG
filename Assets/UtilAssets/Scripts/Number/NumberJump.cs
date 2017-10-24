using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberJump : NumberEffectBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Action(Vector3 position)
    {
        base.Action(position);
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        // ビルボード処理
        base.BillBoard();

        // ステートで動きを分岐
        switch (state)
        {
            case State.Start:

                // y座標を↑にずらす
                transform.localPosition += new Vector3(0, 0.02f, 0);

                // アルファを上げる
                alpha = Mathf.Min(alpha + 50, 255);
                //transform.GetChild(0).GetComponent<Number>().SetAlpha(alpha / 255.0f);

                if (++frame >= 12)
                {
                    //　アルファがマックスになったら終了
                    state = State.Arrival;

                    // フレームを初期化
                    frame = 0;
                }

                break;
            case State.Arrival:

                transform.position += new Vector3(0, 0.005f, 0);

                // フレームで時間稼ぎ
                if (++frame > 12)
                    state = State.End;

                break;
            case State.End:
                //transform.localPosition += new Vector3(0, -0.005f, 0);
                // アルファを上げる
                alpha = Mathf.Max(alpha - 40, 0);
                //transform.GetChild(0).GetComponent<Number>().SetAlpha(alpha / 255.0f);

                // なんか終わるフラグを立てる
                if (alpha == 0)
                {
                    //Destroy(gameObject);
                    gameObject.SetActive(false);
                }
                break;
        }

    }
}
