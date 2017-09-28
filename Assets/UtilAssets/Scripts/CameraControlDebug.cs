using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlDebug : MonoBehaviour
{
    [Range(0.01f, 0.1f)]
    public float translateSpeed = 0.05f;// 移動速度

    [Range(1.0f, 5.7f)]
    public float rotateSpeed = 3.0f;   // 回転速度

    [Range(0.1f, 1)]
    public float lerp = 0.33f;          // 補間度合

    Transform cashTransform;

    Vector3 newPosition;
    float newYaw, newPitch;

	// Use this for initialization
	void Start()
    {
        cashTransform = transform;

        //Debug.Log("キテルグマ");
        newPosition = cashTransform.position;
        newYaw = cashTransform.localEulerAngles.y;
        newPitch = cashTransform.localEulerAngles.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // キー入力で制御

        // Translate
        var forward = new Vector3(Mathf.Sin(cashTransform.localEulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(cashTransform.localEulerAngles.y * Mathf.Deg2Rad));
        if (Input.GetKey(KeyCode.W)) newPosition += forward * translateSpeed;
        if (Input.GetKey(KeyCode.S)) newPosition -= forward * translateSpeed;
        if (Input.GetKey(KeyCode.A)) newPosition -= cashTransform.right * translateSpeed;
        if (Input.GetKey(KeyCode.D)) newPosition += cashTransform.right * translateSpeed;
        if (Input.GetKey(KeyCode.Q)) newPosition.y -= translateSpeed;
        if (Input.GetKey(KeyCode.E)) newPosition.y += translateSpeed;
        if (newPosition.y < 0) newPosition.y = 0;
        cashTransform.localPosition = newPosition * lerp + cashTransform.localPosition * (1 - lerp);

        // Rotate
        //if (Input.GetKey(KeyCode.UpArrow)) transform.Rotate(-rotateSpeed, 0, 0);
        //if (Input.GetKey(KeyCode.DownArrow)) transform.Rotate(rotateSpeed, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow))  newPitch -= rotateSpeed;
        if (Input.GetKey(KeyCode.DownArrow)) newPitch += rotateSpeed;
        if (Input.GetKey(KeyCode.LeftArrow)) newYaw -= rotateSpeed;
        if (Input.GetKey(KeyCode.RightArrow)) newYaw += rotateSpeed;
        newPitch = Mathf.Clamp(newPitch, -85, 85);
        var newAngle = cashTransform.localEulerAngles;
        newAngle.x = Mathf.LerpAngle(cashTransform.localEulerAngles.x, newPitch, lerp);
        newAngle.y = Mathf.LerpAngle(cashTransform.localEulerAngles.y, newYaw, lerp);
        cashTransform.localEulerAngles = newAngle;
    }
}
