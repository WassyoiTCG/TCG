using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexOffsetMove : MonoBehaviour {

    public Material moveOffsetMaterial;

    public Vector2 moveSpeed;
    Vector2 move;

	// Use this for initialization
	void Start ()
    {
        move.x = move.y = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        move.x += moveSpeed.x;
        move.y += moveSpeed.y;
        if (move.x < 0) move.x += 1;
        else if (move.x > 1) move.x -= 1;
        if (move.y < 0) move.y += 1;
        else if (move.y > 1) move.y -= 1;

        // シェーダーにセット
        moveOffsetMaterial.SetTextureOffset("_MainTex", move);
	}
}
