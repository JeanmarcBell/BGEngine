using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class Texture2D
    {
        public int ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private string _name;
        public string Name { 
            get { return _name; }
            set { _name = value.ToLower(); } 
        }
        
        public Vector2[] TexCoords { get; private set; }

        public RectangleF TextureCoordRect
        {
            get
            {
                return new RectangleF(TexCoords[0].X, TexCoords[0].Y, TexCoords[1].X - TexCoords[0].X, TexCoords[2].Y - TexCoords[0].Y);
            }
        }
        public Texture2D(RectangleF unitRectangle, int id)
        {
            SetRegion(unitRectangle);
            this.ID = id;
            Width = 0;
            Height = 0;
            Name = "";
        }

        private void SetRegion(RectangleF unitRectangle)
        {
            TexCoords = new Vector2[4];
            TexCoords[0] = new Vector2(unitRectangle.Left, unitRectangle.Top);
            TexCoords[1] = new Vector2(unitRectangle.Right, unitRectangle.Top);
            TexCoords[2] = new Vector2(unitRectangle.Right, unitRectangle.Bottom);
            TexCoords[3] = new Vector2(unitRectangle.Left, unitRectangle.Bottom);
        }
    }
}
