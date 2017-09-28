using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1;

    public Vector3 move;
    Transform cashTransform;

	// Use this for initialization
	void Start ()
    {
        cashTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        var axis = Vector2.zero;

        // 入力
        if (Input.GetKey(KeyCode.UpArrow))   axis.y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) axis.y -= 1;
        if (Input.GetKey(KeyCode.LeftArrow)) axis.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow))axis.x += 1;

        // 軸正規化
        if(axis != Vector2.zero) axis.Normalize();

        // 移動量作成
        move.x = axis.x * moveSpeed;
        move.z = axis.y * moveSpeed;
    }

    void FixedUpdate()
    {
        // 座標更新処理
        cashTransform.LookAt(cashTransform.localPosition + move);
        cashTransform.localPosition += move;
        move *= 0.5f;
    }
}
