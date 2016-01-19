using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class RectanglePack
    {
        public Size BoundingSize { get; set; }
        public List<KeyValuePair<string, Rectangle>> PackedRectangles;
    }

    public static class RectanglePacker
    {
        public static RectanglePack PackRectangles(List<KeyValuePair<string, Size>> textures, int pixelSpacing, int MaxWidth = 2048, int MaxHeight = 2048)
        {
            IOrderedEnumerable<KeyValuePair<string, Size>> orderedTextures = textures.OrderByDescending(p => p.Value.Height);
            
            int horizontalOffset = 0;
            int verticalOffset = 0;
            int maxInRow = 0;
            int numRows = 0;

            RectanglePack pack = new RectanglePack();
            pack.PackedRectangles = new List<KeyValuePair<string, Rectangle>>();

            foreach (KeyValuePair<string, Size> kvp in orderedTextures)
            {
                if (horizontalOffset + kvp.Value.Width + pixelSpacing > MaxWidth)
                {
                    horizontalOffset = 0;
                    verticalOffset += maxInRow + pixelSpacing;
                    maxInRow = 0;
                    numRows++;
                }

                KeyValuePair<string, Rectangle> packedRectangle = new KeyValuePair<string, Rectangle>(kvp.Key,
                    new Rectangle(horizontalOffset, verticalOffset, kvp.Value.Width, kvp.Value.Height));
                pack.PackedRectangles.Add(packedRectangle);
                horizontalOffset += kvp.Value.Width + pixelSpacing;
                if (kvp.Value.Height > maxInRow)
                {
                    maxInRow = kvp.Value.Height;
                }
            }

            int width = numRows > 0 ? MaxWidth : horizontalOffset;
            int height = verticalOffset + maxInRow + pixelSpacing;
            pack.BoundingSize = new Size(width, height);
            return pack;
        }
    }
}
