using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    int shakeFrame;

    [SerializeField]
    float shakePower;

    Vector3 vOrgPos;// 元のスケール

    int frame;

    // Use this for initialization
    void Start()
    {
        frame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (frame > 0)
        {
            frame--;

            Vector2 shakedPos = /*transform.position*/ Vector2.zero;
            float power = shakePower * (frame / (float)shakeFrame);
            //Debug.Log(power);
            float x = ((Random.value - 0.5f) * 2) * power, y = ((Random.value - 0.5f) * 2) * power;
            shakedPos.x += x;
            shakedPos.y += y;
            //Debug.Log("x:" + x + "y:" + y);
            //Debug.Log("sx:" + shakedPos.x + "y:" + shakedPos.y);
            transform.position = new Vector3(transform.localPosition.x + shakedPos.x, transform.localPosition.y + shakedPos.y, transform.position.z);
            
            // 最後に来るフレームで元の位置に戻す
            if (frame == 0)
            {
                transform.position = new Vector3(vOrgPos.x, vOrgPos.y, vOrgPos.z);
            }
         }

    }

    public void Set()
    {
        frame = shakeFrame;

        vOrgPos = transform.localPosition;
    }
}
