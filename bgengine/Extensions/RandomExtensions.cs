using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class RandomExtensions
    {
        public static Vector2 NextVector2(this Random rand)
        {
            var x = ((rand.NextDouble() * 2) - 1) * Int32.MaxValue;
            var y = ((rand.NextDouble() * 2) - 1) * Int32.MaxValue;

            return new Vector2((float)x, (float)y);
        }

        public static Vector2 NextVector2(this Random rand, float min, float max)
        {
            var diff = max - min;
            var x = (rand.NextDouble() * diff) + min;
            var y = (rand.NextDouble() * diff) + min;

            return new Vector2((float)x, (float)y);
        }
    }
}
