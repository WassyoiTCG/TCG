using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NumberEffectBase : MonoBehaviour
{
    protected enum State
    {
        Start,
        Arrival,
        End
    }

    protected Transform cashTransform;
    protected Transform cameraTransform;
    protected State state = State.Start;
    protected int alpha = 0, frame = 0;

    protected virtual void Awake()
    {
        cashTransform = transform;
        cameraTransform = Camera.main.transform;
    }

    public virtual void Action(Vector3 position)
    {
        cashTransform.localPosition = position;
        state = State.Start;
        alpha = frame = 0;
    }
    protected abstract void FixedUpdate();

    protected void BillBoard()
    {
        // ビルボード処理
        Vector3 p = cameraTransform.position;
        p.y = cashTransform.position.y;
        cashTransform.LookAt(p);

        // 逆になってたので無理やり修正
        cashTransform.Rotate(0, 180, 0);
    }
}
