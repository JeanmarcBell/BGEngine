using BGEngine;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Colors
{
    public static class ColorUtil
    {
        public static Color4 HSVToColor(float hue, float saturation, float value)
        {
            if (hue == 0 && saturation == 0)
                return new Color4(value, value, value, 1.0f);

            float c = saturation * value;
            float x = c * (1 - Math.Abs(hue % 2 - 1));
            float m = value - c;

            if (hue < 1) 
                return new Color4(c + m, x + m, m, 1.0f);
            else if (hue < 2)
                return new Color4(x + m, c + m, m, 1.0f);
            else if (hue < 3) 
                return new Color4(m, c + m, x + m, 1.0f);
            else if (hue < 4) 
                return new Color4(m, x + m, c + m, 1.0f);
            else if (hue < 5) 
                return new Color4(x + m, m, c + m, 1.0f);
            else 
                return new Color4(c + m, m, x + m, 1.0f);
        }
    }
}
