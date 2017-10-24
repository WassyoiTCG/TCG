using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class oulInput
{
    //===============================================
    //	定数
    //===============================================

    public enum TouchState
    {
        None = 99,           // タッチなし
        Began = 0,           // 画面に指がタッチしました
        Moved = 1,           // 画面上で指が動きました
        Stationary = 2,      // 指が画面にタッチしているが動いてはいません
        Ended = 3,           // 画面から指が離れました。これは、タッチの最終段階です。
        Canceled = 4,        // システムがタッチの追跡をキャンセルしました
    }

    //===============================================
    //	メンバ変数
    //===============================================
    static Vector3 prevPos = Vector3.zero;
    static float holdTime = 0;
    static GameObject touchCollision2DObject; // タップしたときにヒットしたオブジェクト
    static GameObject touchCollision3DObject; // タップしたときにヒットしたオブジェクト

    static bool isAndroid = Application.platform == RuntimePlatform.Android;
    static bool isIOS = Application.platform == RuntimePlatform.IPhonePlayer;
    static bool isEditor = !isAndroid && !isIOS;

    static public void Update()
    {
        switch (GetTouchState())
        {
            case TouchState.None:
                touchCollision2DObject = null;
                touchCollision3DObject = null;
                break;

            case TouchState.Ended:
            case TouchState.Canceled:
                break;

            case TouchState.Began:
                holdTime = 0;
                touchCollision2DObject = Collision2D();
                touchCollision3DObject = Collision3D();
                break;

            case TouchState.Stationary:
            case TouchState.Moved:
                holdTime += Time.deltaTime;
                break;
        }
    }

    static public TouchState GetTouchState(int iTouchID = 0)
    {
        if (isEditor)
        {
            if (Input.GetMouseButtonDown(0)) { return TouchState.Began; }
            if (Input.GetMouseButton(0)) { return TouchState.Moved; }
            if (Input.GetMouseButtonUp(0)) { return TouchState.Ended; }
        }
        else
        {
            if (Input.touchCount > 0) return (TouchState)((int)Input.GetTouch(iTouchID).phase);
        }
        return TouchState.None;
    }

    static public Vector3 GetPosition(int touchID = 0, bool isWorldCoordinate = true)
    {
        var ret = new Vector3(114, 514, 810);   // Vector3.zeroだとなんか怖い
        if (isEditor)
        {
            ret = Input.mousePosition;
        }
        else
        {
            if (Input.touchCount > 0) ret = Input.GetTouch(touchID).position;
        }

        if (isWorldCoordinate)
        {
            //if (!Camera.main) UnityEditor.EditorUtility.DisplayDialog("Error", "Camera Nai!!!!!!", "OK");
            /*else */ret = Camera.main.ScreenToWorldPoint(ret);
        }
        return ret;
    }

    static public Vector3 GetDeltaPosition(int iTouchID = 0)
    {
        if (isEditor)
        {
            var phase = GetTouchState();
            if (phase != TouchState.None)
            {
                var current = Input.mousePosition;
                var delta = current - prevPos;
                prevPos = current;
                return delta;
            }
        }
        else
        {
            if (Input.touchCount > 0) return Input.GetTouch(iTouchID).deltaPosition;
        }
        return Vector3.zero;
    }

    static public float GetHoldTime() { return holdTime; }

    static public GameObject Collision2D()
    {
        Collider2D col = Physics2D.OverlapPoint(GetPosition());
        return col ? col.gameObject : null;
    }
    static public GameObject Collision3D()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(GetPosition(0, false));
        Collider col = Physics.Raycast(ray, out hit, 500.0f) ? hit.collider : null;
        return col ? col.gameObject : null;
    }

    static public GameObject GetTapObject2D()
    {
        // 離されているかつ、タッチした瞬間のオブジェクトと離した瞬間のオブジェクトが同じだったらそのオブジェクトを返す
        return (GetTouchState() == TouchState.Ended && touchCollision2DObject == Collision2D()) ? touchCollision2DObject : null;
    }
    static public GameObject GetTapObject3D()
    {
        // 離されているかつ、タッチした瞬間のオブジェクトと離した瞬間のオブジェクトが同じだったらそのオブジェクトを返す
        return (GetTouchState() == TouchState.Ended && touchCollision3DObject == Collision3D()) ? touchCollision3DObject : null;
    }
    //static public bool isTapObject(GameObject target)
    //{
    //    // 離されているかつ、タッチした瞬間のオブジェクトと離した瞬間のオブジェクトと引数のオブジェクトが同じ
    //    return (GetTapObject() == target);
    //}
}