using System;

namespace RVO
{
    public struct RVOMath
    {
        public static float absSq(Vector2 v)
        {
            return v * v;
        }
        public static Vector2 normalize(Vector2 v)
        {
            return v / abs(v);
        }

        internal const float RVO_EPSILON = 0.00001f;

        internal static float sqrt(float a)
        {
            return (float)Math.Sqrt(a);
        }
        internal static float fabs(float a)
        {
            return Math.Abs(a);
        }
        internal static float distSqPointLineSegment(Vector2 a, Vector2 b, Vector2 c)
        {
            float r = ((c - a) * (b - a)) / absSq(b - a);

            if (r < 0.0f)
            {
                return absSq(c - a);
            }
            else if (r > 1.0f)
            {
                return absSq(c - b);
            }
            else
            {
                return absSq(c - (a + r * (b - a)));
            }
        }
        internal static float sqr(float p)
        {
            return p * p;
        }
        internal static float det(Vector2 v1, Vector2 v2)
        {
            return v1.x_ * v2.y_ - v1.y_ * v2.x_;
        }
        internal static float abs(Vector2 v)
        {
            return (float)Math.Sqrt(absSq(v));
        }
        internal static float leftOf(Vector2 a, Vector2 b, Vector2 c)
        {
            return det(a - c, b - a);
        }
    }
}