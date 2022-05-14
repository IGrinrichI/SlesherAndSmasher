using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlane
{
    public readonly Vector3 m, n;

    public MyPlane(Vector3 n, Vector3 m)
    {
        //A(x-x0)+B(y-y0)+C(z-z0)=0
        //M(x0,y0,z0), N(A,B,C)
        this.m = m;
        this.n = n;
    }

    public bool GetSide(Vector3 dot)
    {
        return n.x * (dot.x - m.x) + n.y * (dot.y - m.y) + n.z * (dot.z - m.z) > 0f ? true : false;
    }

    public Vector3 GetDot(Vector3 point, Vector3 direction)
    {
        float t = -(this.n.x*(point.x-this.m.x) + this.n.y * (point.y - this.m.y) + this.n.z * (point.z - this.m.z)) / (this.n.x*direction.x + this.n.y * direction.y + this.n.z * direction.z);
        return point + direction * t;
    }

    public Vector3 GetDotBetween(Vector3 a, Vector3 b)
    {
        return GetDot(a, b-a);
    }
}
