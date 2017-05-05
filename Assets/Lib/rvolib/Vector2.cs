using System;
using System.Globalization;

namespace RVO
{
    public struct Vector2
    {
        internal float x_;
        internal float y_;

        public float x() { return x_; }
        public float y() { return y_; }

        public override string ToString()
        {
            return "(" + x_.ToString(new CultureInfo("").NumberFormat) + "," + y_.ToString(new CultureInfo("").NumberFormat) + ")";
        }

        public Vector2(float x, float y)
        {
            x_ = x;
            y_ = y;
        }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x_ + rhs.x_, lhs.y_ + rhs.y_);
        }
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x_ - rhs.x_, lhs.y_ - rhs.y_);
        }
        public static float operator *(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x_ * rhs.x_ + lhs.y_ * rhs.y_;
        }
        public static Vector2 operator *(float k, Vector2 u)
        {
            return u * k;
        }
        public static Vector2 operator *(Vector2 u, float k)
        {
            return new Vector2(u.x_ * k, u.y_ * k);
        }
        public static Vector2 operator /(Vector2 u, float k)
        {
            return new Vector2(u.x_ / k, u.y_ / k);
        }
        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.x_, -v.y_);
        }
    }
}
