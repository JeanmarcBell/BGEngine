using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class QuaternionExtensions
    {
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
       {
           float rollOver2 = roll * 0.5f;
           float sinRollOver2 = (float)Math.Sin((double)rollOver2);
           float cosRollOver2 = (float)Math.Cos((double)rollOver2);
           float pitchOver2 = pitch * 0.5f;
           float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
           float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
           float yawOver2 = yaw * 0.5f;
           float sinYawOver2 = (float)Math.Sin((double)yawOver2);
           float cosYawOver2 = (float)Math.Cos((double)yawOver2);

           Quaternion result = new Quaternion();
           result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 - sinYawOver2 * sinPitchOver2 * sinRollOver2;
           result.X = sinYawOver2 * sinPitchOver2 * cosRollOver2 + cosYawOver2 * cosPitchOver2 * sinRollOver2;
           result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
           result.Z = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;

           return result;
       }
    }
}
