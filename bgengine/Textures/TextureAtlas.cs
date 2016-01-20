using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class TextureAtlas
    {
        public string Name { get; private set; }
        public Dictionary<string, Texture2D> Textures { get; private set; }
        public int Count { get { return Textures.Count; } }
        public int TextureID { get; private set; }
        public Size AtlasSize { get; private set; }
        public bool IsPacked { get; private set; }

        private Dictionary<string, Bitmap> unpackedImages = new Dictionary<string, Bitmap>();

        public TextureAtlas(string name)
        {
            IsPacked = false;
            Name = name;
            Textures = new Dictionary<string, Texture2D>();
            TextureID = -1;
            AtlasSize = new Size(0, 0);
        }

        public TextureAtlas(string name, Bitmap tileMap, int tileWidth, int tileHeight)
        {
            AtlasSize = tileMap.Size;
            Textures = new Dictionary<string, Texture2D>();
            TextureID = GL.GenTexture();
            Name = name;

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData bmpData = tileMap.LockBits(new Rectangle(0, 0, tileMap.Width, tileMap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            tileMap.UnlockBits(bmpData);

            int index = 0;
            for (int y=0 ; y <= (tileMap.Height - tileHeight) ; y += tileHeight)
            {
                for (int x = 0; x <= (tileMap.Width - tileWidth); x += tileWidth)
                {
                    Rectangle pixelBounds = new Rectangle(x, y, tileWidth, tileHeight);
                    Texture2D texture = new Texture2D(MakeUnitRectangle(pixelBounds, tileMap.Size), TextureID);
                    texture.Name = index.ToString();
                    texture.Width = tileWidth;
                    texture.Height = tileHeight;
                    Textures.Add(texture.Name, texture);
                    index++;
                }
            }

            IsPacked = true;
        }

        public void AddTexture(Bitmap bitmap, string textureName)
        {
            if (IsPacked)
            {
                throw new InvalidOperationException("Cannot add textures to a TextureAtlas which is already packed.");
            }
            unpackedImages.Add(textureName, bitmap);            
        }

        private RectangleF MakeUnitRectangle(Rectangle rect, Size size)
        {
            return new RectangleF((float)rect.Left / (float)size.Width, (float)rect.Top / (float)size.Height,
                (float)rect.Width / (float)size.Width, (float)rect.Height / (float)size.Height);
        }

        public void PackTextures(int maxWidth = 2048, int maxHeight = 2048)
        {
            // Calculate packing
            List<KeyValuePair<string, Size>> sizeList = new List<KeyValuePair<string, Size>>();
            foreach (KeyValuePair<string, Bitmap> kvp in unpackedImages)
            {
                sizeList.Add(new KeyValuePair<string, Size>(kvp.Key, kvp.Value.Size));
            }
            RectanglePack packedRectangles = RectanglePacker.PackRectangles(sizeList, 2, maxWidth, maxHeight);

            // Build bitmaps
            Bitmap atlasBitmap = new Bitmap(packedRectangles.BoundingSize.Width, packedRectangles.BoundingSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(atlasBitmap);
            graphics.Clear(Color.Transparent);

            TextureID = GL.GenTexture();
            foreach (KeyValuePair<string, Rectangle> packedRectangle in packedRectangles.PackedRectangles)
            {
                Bitmap bitmap = null;
                if (unpackedImages.TryGetValue(packedRectangle.Key, out bitmap))
                {
                    graphics.DrawImage(bitmap, new Rectangle(packedRectangle.Value.Left, packedRectangle.Value.Top, bitmap.Width, bitmap.Height));

                    Texture2D texture = new Texture2D(MakeUnitRectangle(packedRectangle.Value, packedRectangles.BoundingSize), TextureID);
                    texture.Name = packedRectangle.Key.ToLower();
                    texture.Width = bitmap.Width;
                    texture.Height = bitmap.Height;
                    Textures.Add(texture.Name, texture);
                }            
            }
            graphics.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData bmpData = atlasBitmap.LockBits(new Rectangle(0, 0, atlasBitmap.Width, atlasBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            
            atlasBitmap.UnlockBits(bmpData);
            atlasBitmap.Save("test.png");
            atlasBitmap.Dispose();
        }

        public Texture2D this[string textureName]
        {
            get{
                Texture2D texture;
                this.Textures.TryGetValue(textureName, out texture);
                return texture;
            }
        }
    }
}
