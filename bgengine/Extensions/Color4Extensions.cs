using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class Color4Extensions
    {
        public static Color4 Lerp(this Color4 value1, Color4 value2, float amount)
        {
            Color4 c = new Color4(
                (1-amount) * value1.R + amount*value2.R,
                (1-amount) * value1.G + amount*value2.G,
                (1-amount) * value1.B + amount*value2.B,
                (1-amount) * value1.A + amount*value2.A
                );
            return c;
        }

        public static int ToAbgr(this Color4 color)
        {
            int A = (int)(color.A * 255);
            int B = (int)(color.B * 255);
            int G = (int)(color.G * 255);
            int R = (int)(color.R * 255);

            return A << 24 | B << 16 | G << 8 | R;
        }
    }
}
