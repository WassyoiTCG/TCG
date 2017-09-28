using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class oulMath
{
    // ビルボード計算
    static public void Billboard(ref Transform transform)
    {
        Vector3 p = Camera.main.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }

    // ベジエ計算
    static public Vector3 Bezier(Vector3[] p, float t)
    {
        var numPoint = p.Length;
        var b = t;
        var a = 1 - b;

        /*				//		参考資料		//
        //ベジェ曲線↓　まず　　最初と中間　　　　次に　　　　中間と最後
        pos->x = a*a*a* p1.x + 3 * a*a*b*p2.x + 3 * a*b*b*p2.x + b*b*b*p3.x;
        pos->y = a*a*a* p1.y + 3 * a*a*b*p2.y + 3 * a*b*b*p2.y + b*b*b*p3.y;
        pos->z = a*a*a* p1.z + 3 * a*a*b*p2.z + 3 * a*b*b*p2.z + b*b*b*p3.z;
        */

        // 2点間の直線の場合、ベジエ計算をするとおかしくなるので、割合による直線の計算にする
        if (numPoint == 2)
        {
            return p[0] * a + p[1] * b;
        }

        // 始点
        var ret = p[0] * Mathf.Pow(a, numPoint);

        // 中間
        for (int i = 1; i < numPoint - 1; i++)    // -1なのは終点を省くから
        {
            var mult = b;
            for (int j = 1; j < numPoint - 1; j++)
            {
                mult *= (j >= i) ? a : b;
            }
            ret += p[i] * (numPoint * mult);
        }

        // 終点
        ret += p[numPoint - 1] * Mathf.Pow(b, numPoint);

        return ret;
    }

    // 線と球の交点、ヒット判定
    static public bool LineVsSphere(SphereCollider sphere, Vector3 linePosition, Vector3 lineDirection, out Vector3 intersectionPoint)
    {
        //Vector3 e = sphere.center - linePosition;
        //float a = Vector3.Dot(e, lineDirection);
        //float t0 = a - Mathf.Sqrt(sphere.radius * sphere.radius - e.magnitude + a * a);
        //intersectionPoint = linePosition + lineDirection * t0;
        //
        //return false;

        // l.x, l.y, l.z : レイの始点
        // v.x, v.y, v.z : レイの方向ベクトル
        // p.x, p.y, p.z : 球の中心点の座標
        // r : 球の半径
        // q1x, q1y, q1z: 衝突開始点（戻り値）
        // q2x, q2y, q2z: 衝突終了点（戻り値

        intersectionPoint = Vector3.zero;

        Vector3 p = sphere.transform.position + sphere.center;
        p.x = p.x - linePosition.x;
        p.y = p.y - linePosition.y;
        p.z = p.z - linePosition.z;

        float r = sphere.radius * sphere.transform.localScale.x;

        float A = lineDirection.sqrMagnitude;
        float B = lineDirection.x * p.x + lineDirection.y * p.y + lineDirection.z * p.z;
        float C = p.sqrMagnitude - r * r;

        if (A == 0.0f)
            return false; // レイの長さが0

        float s = B * B - A * C;
        if (s < 0.0f)
            return false; // 衝突していない

        s = Mathf.Sqrt(s);
        float a1 = (B - s) / A;
        float a2 = (B + s) / A;

        if (a1 < 0.0f || a2 < 0.0f)
            return false; // レイの反対で衝突

        // 最初の交点
        intersectionPoint = linePosition + a1 * lineDirection;

        // 突き抜けた交点
        //intersectionPoint = linePosition + a2 * lineDirection;

        return true;
    }

    // 線と球の交点、ヒット判定
    static public bool LineVsCircle(SphereCollider sphere, Vector3 linePosition, Vector3 lineDirection, out Vector3 intersectionPoint)
    {
        //Vector3 e = sphere.center - linePosition;
        //float a = Vector3.Dot(e, lineDirection);
        //float t0 = a - Mathf.Sqrt(sphere.radius * sphere.radius - e.magnitude + a * a);
        //intersectionPoint = linePosition + lineDirection * t0;

        intersectionPoint = Vector3.zero;

        Vector3 p = sphere.transform.position + sphere.center;
        p.x = p.x - linePosition.x;
        p.y = p.y - linePosition.y;
        p.z = p.z - linePosition.z;

        float r = sphere.radius * sphere.transform.localScale.x;

        float A = lineDirection.sqrMagnitude;
        float B = lineDirection.x * p.x + lineDirection.y * p.y + lineDirection.z * p.z;
        float C = p.sqrMagnitude - r * r;

        if (A == 0.0f)
            return false; // レイの長さが0

        float s = B * B - A * C;
        if (s < 0.0f)
            return false; // 衝突していない

        s = Mathf.Sqrt(s);
        float a1 = (B - s) / A;
        float a2 = (B + s) / A;

        if (a1 < 0.0f || a2 < 0.0f)
            return false; // レイの反対で衝突

        // 交点
        intersectionPoint = linePosition + p.magnitude * lineDirection;

        return true;
    }

    static public Vector3 LerpVector(Vector3 a, Vector3 b, float t)
    {
        return a * (1 - t) + b * t;
    }
}
