using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class oulMath
{
    // ビルボード計算
    static public void Billboard(Transform transform)
    {
        Vector3 p = Camera.main.transform.position;
        //p.y = transform.position.y;
        transform.LookAt(p);
    }

    // ベジエ計算
    static public Vector3 Bezier(Vector3[] p, float t)
    {
        int numPoint = p.Length;
        float b = t;
        float a = 1 - b;

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
            float mult = b;
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

    static public float Bezierf(float[] f, float t)
    {
        int numPoint = f.Length;
        float b = t;
        float a = 1 - b;

        /*				//		参考資料		//
        //ベジェ曲線↓　まず　　最初と中間　　　　次に　　　　中間と最後
        pos->x = a*a*a* p1.x + 3 * a*a*b*p2.x + 3 * a*b*b*p2.x + b*b*b*p3.x;
        pos->y = a*a*a* p1.y + 3 * a*a*b*p2.y + 3 * a*b*b*p2.y + b*b*b*p3.y;
        pos->z = a*a*a* p1.z + 3 * a*a*b*p2.z + 3 * a*b*b*p2.z + b*b*b*p3.z;
        */

        // 2点間の直線の場合、ベジエ計算をするとおかしくなるので、割合による直線の計算にする
        if (numPoint == 2)
        {
            return f[0] * a + f[1] * b;
        }

        // 始点
        var ret = f[0] * Mathf.Pow(a, numPoint);

        // 中間
        for (int i = 1; i < numPoint - 1; i++)    // -1なのは終点を省くから
        {
            float mult = b;
            for (int j = 1; j < numPoint - 1; j++)
            {
                mult *= (j >= i) ? a : b;
            }
            ret += f[i] * (numPoint * mult);
        }

        // 終点
        ret += f[numPoint - 1] * Mathf.Pow(b, numPoint);

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

    // 線とボックスの交点、ヒット判定
    static public bool LineVsBox(BoxCollider box, Vector3 linePosition, Vector3 lineDirection, out Vector3 intersectionPoint)
    {
        //Vector3 e = sphere.center - linePosition;
        //float a = Vector3.Dot(e, lineDirection);
        //float t0 = a - Mathf.Sqrt(sphere.radius * sphere.radius - e.magnitude + a * a);
        //intersectionPoint = linePosition + lineDirection * t0;

        intersectionPoint = Vector3.zero;

        Vector3 width = box.size * box.transform.localScale.x;
        Vector3 boxMax = width / 2;
        Vector3 boxMin = -boxMax;

        //Vector3 p = box.transform.position + box.center;
        //p.x = p.x - linePosition.x;
        //p.y = p.y - linePosition.y;
        //p.z = p.z - linePosition.z;
        
        // 線をボックスの空間に
        Matrix4x4 invMat = Matrix4x4.Inverse(box.transform.localToWorldMatrix);
        linePosition = invMat * linePosition;
        invMat.SetRow(3, new Vector4(0, 0, 0, 1));
        lineDirection = invMat * lineDirection;

        float[] p = new float[3];
        p[0] = linePosition.x;
        p[1] = linePosition.y;
        p[2] = linePosition.z;
        float[] d = new float[3];
        d[0] = lineDirection.x;
        d[1] = lineDirection.y;
        d[2] = lineDirection.z;
        float[] max = new float[3];
        max[0] = width.x / 2;
        max[1] = width.y / 2;
        max[2] = width.z / 2;
        float[] min = new float[3];
        min[0] = -max[0];
        min[1] = -max[1];
        min[2] = -max[2];

        float t = float.MinValue, t_max = float.MaxValue;

        for(int i=0;i<3;i++)
        {
            if (Mathf.Abs(d[i]) >= Mathf.Epsilon)
                continue;
            if (p[i] < min[i] || p[i] > max[i])
                return false;   // 交差していない

            // スラブとの距離を算出
            // t1が近スラブ、t2が遠スラブとの距離
            float odd = 1.0f / d[i];
            float t1 = (min[i] - p[i]) * odd;
            float t2 = (max[i] - p[i]) * odd;
            if(t1 > t2)
            {
                float tmp = t1;
                t1 = t2;
                t2 = tmp;
            }
            if (t1 > t)
                t = t1;
            if (t2 < t_max)
                t_max = t2;

            // スラブ交差チェック
            if (t >= t_max)
                return false;
        }

        // ここまで来たら交差している

        // 交点
        intersectionPoint = linePosition + t * lineDirection;

        return true;
    }

    static public Vector3 LerpVector(Vector3 a, Vector3 b, float t)
    {
        return a * (1 - t) + b * t;
    }

    static public Vector3 ScreenToWorldPlate(Vector3 screenPos, Vector3 plateNormal, float shift)
    {
        // スクリーン上からのプロジェクションのNearとFarを求める
        //Vector3 NearPosition = Math::ScreenToWorld(screenPos, 0.0f);
        //Vector3 FarPosition = Math::ScreenToWorld(screenPos, 1.0f);
        Vector3 nearPosition = Camera.main.ScreenToWorldPoint(screenPos);

        // Unityの力で単位ベクトルを作る
        Vector3 direction = Camera.main.ScreenPointToRay(screenPos).direction;

        /*	線と平面による交点判定
        AXの長さ: XBの長さ = PAとNの内積 : PBとNの内積
        ※内積はマイナス値になる場合があるので、絶対値を使ってください。

        交点X = A + ベクトルAB * (PAとNの内積 / (PAとNの内積 + PBとNの内積))
        */

        const float dist = 65535;    // とりあえずでかい値(交点をとるため、あまり小さいと平面に届かない)

        Vector3 PA = nearPosition - (plateNormal * shift);
        Vector3 PB = (nearPosition + direction * dist) - (plateNormal * shift);
        float XB = Mathf.Abs(Vector3.Dot(PA, plateNormal));

        float pa_n = Mathf.Abs(Vector3.Dot(PA, plateNormal));
        float pb_n = Mathf.Abs(Vector3.Dot(PB, plateNormal));

        return (nearPosition + ((nearPosition + direction * dist) - nearPosition) * (pa_n / (pa_n + pb_n)));
    }
}
