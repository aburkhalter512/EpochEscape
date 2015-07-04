﻿using UnityEngine;

namespace Utilities
{
    public class Vec2Int
    {
        public int x;
        public int y;

        public Vec2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vec2Int(Vec2Int v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public static Vec2Int operator +(Vec2Int v1, Vec2Int v2)
        {
            return new Vec2Int(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vec2Int operator -(Vec2Int v1, Vec2Int v2)
        {
            return new Vec2Int(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vec2Int operator *(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x * s, v1.y * s);
        }
        public static Vec2Int operator /(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x / s, v1.y / s);
        }
        public static Vec2Int operator %(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x % s, v1.y % s);
        }

        public static void floorTo(Vec2Int v, int multiple)
        {
            v.x = v.x / multiple * multiple;
            v.y = v.y / multiple * multiple;
        }

        public virtual Vec2Int clone()
        {
            return new Vec2Int(this);
        }
    }

    public class Math
    {
        public static Vector2 copy(Vector2 v) { return new Vector2(v.x, v.y); }
        public static Vector2 toVector2(Vec2Int p) { return new Vector2(p.x, p.y); }

        public static Vector3 toVector3(Vector2 v) { return new Vector3(v.x, v.y, 0.0f); }
        public static Vector3 toVector3(Vector2 v, float z) { return new Vector3(v.x, v.y, z); }
        public static Vector3 toVector3(Vec2Int v) { return new Vector3(v.x, v.y, 0); }

        public static Vector3 copy(Vector3 v) { return new Vector3(v.x, v.y, v.z); }

        public const float DEFAULT_TOLERANCE = 0.05f;
        public static bool IsApproximately(float a, float b, float tolerance = DEFAULT_TOLERANCE)
        {
            return Mathf.Abs(a - b) < tolerance;
        }
        public static bool IsApproximately(Vector3 a, Vector3 b, float tolerance = DEFAULT_TOLERANCE)
        {
            return IsApproximately(a.x, b.x) && IsApproximately(a.y, b.y) && IsApproximately(a.z, b.z);
        }
        public static int roundToMultiple(float num, int multiple)
        {
            return Mathf.RoundToInt(num / multiple) * multiple;
        }

        public static float WrapAngle(float angle)
        {
            angle %= 360;
            return angle < 0.0f ? angle + 360 : angle;
        }
    }
}
