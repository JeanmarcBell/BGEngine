using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// The angle of a vector in radians
        /// </summary>
        public static float ToAngle(this Vector2 vector)
        {
            //atan2 returns -PI to PI. should this return 0 to 2PI instead?
            return (float)(Math.Atan2(vector.Y, vector.X));
        }

        /// <summary>
        /// Create a vector from an angle in Radians
        /// </summary>
        public static Vector2 FromAngle(float angle)
        {
            //var degrees = (float)(angle / RADIANS_TO_DEGREE);
            float x = NormalizeFloat((float)Math.Cos(angle));
            float y = NormalizeFloat((float)Math.Sin(angle));
            
            return new Vector2(x, y);
        }

        private static float NormalizeFloat(float value)
        {
            if (Math.Abs(value) < 0.00000000001f)
            {
                value = 0.0f;
            }

            return value;
        }

        /// <summary>
        /// The distance between two vectors squared
        /// </summary>
        /// <param name="baseVector"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float DistanceSquared(this Vector2 baseVector, Vector2 vector)
        {
            float deltaX = baseVector.X - vector.X;
            float deltaY = baseVector.Y - vector.Y;
            var diffVec = baseVector - vector;

            return (diffVec * diffVec).Length;
        }

        /// <summary>
        /// The length of the vector squared
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float LengthSquared(this Vector2 vector)
        {
            return (vector.X * vector.X) + (vector.Y * vector.Y);
        }

        public static void InvertX(this Vector2 vector)
        {
            vector.X = -vector.X;
        }

        public static void InvertY(this Vector2 vector)
        {
            vector.X = -vector.X;
        }

        /// <summary>
        /// Rotates the vector around 0,0
        /// </summary>
        /// <param name="rotation">The angle to rotate by in degrees</param>
        public static Vector2 Rotate(this Vector2 vector, double rotation)
        {
            float sin = (float)Math.Sin(rotation);
            float cos = (float)Math.Cos(rotation);

            float x = vector.X * cos - vector.Y * sin;
            float y = vector.X * sin + vector.Y * cos;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Rotates the vector around a point
        /// </summary>
        /// <param name="origin">The point to rotate around</param>
        /// <param name="rotation">The angle to rotate by in degrees</param>
        public static Vector2 RotateAroundPoint(this Vector2 vector, Vector2 origin, double rotation)
        {
            float sin = (float)Math.Sin(rotation);
            float cos = (float)Math.Cos(rotation);

            float translatedX = vector.X - origin.X;
            float translatedY = vector.Y - origin.Y;

            float rotatedX = translatedX * cos - translatedY * sin;
            float rotatedY = translatedX * sin + translatedY * cos;

            return new Vector2(rotatedX + origin.X, rotatedY + origin.Y);
        }

        /// <summary>
        /// Clamps the values of the vector between -max and +max
        /// </summary>
        public static Vector2 Clamp(this Vector2 vector, float maxX, float maxY)
        {
            float newX = Math.Max(-maxX, Math.Min(vector.X, maxX));
            float newY = Math.Max(-maxY, Math.Min(vector.Y, maxY));

            return new Vector2(newX, newY);
        }

        public static Vector2 ScaleTo(this Vector2 vector, float scalar) {
            if (vector.Length == 0)
            {
                return vector;
            }

            float ratio = scalar / vector.Length;
            float x = vector.X * ratio;
            float y = vector.Y * ratio;

            return new Vector2(x, y);
        }
    }
}
