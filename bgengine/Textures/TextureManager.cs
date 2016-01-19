using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace BGEngine
{
    public class TextureManager
    {
        public Dictionary<string, TextureAtlas> AtlasTable { get; private set; }

        public TextureManager()
        {
            AtlasTable = new Dictionary<string, TextureAtlas>();
        }

        /*public void LoadTexture(string filePath)
        {
            Console.WriteLine("Loading {0}", filePath);
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(filePath);

            FileInfo fileInfo = new FileInfo(filePath);
            string texName = fileInfo.Name.Replace(fileInfo.Extension, "");
            if (TextureTable.ContainsKey(texName))
                throw new ArgumentException("TextureTable already contains a texture with the name \"" + texName + "\"");

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            Bitmap bmp = new Bitmap(filePath);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            bmp.UnlockBits(bmpData);
            Texture2D texInfo = new Texture2D();
            texInfo.ID = id;
            texInfo.Width = bmpData.Width;
            texInfo.Height = bmpData.Height;
            texInfo.Name = texName;

            TextureTable.Add(texInfo.Name, texInfo);
        }*/

        /// <summary>
        /// Load all image files (.jpg or .png) from a directory. Not recursive.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the directory doesn't exist or atlas already loaded.</exception>
        public void LoadAtlasFromFolder(string folderPath, string atlasName, int maxWidth = 2048, int maxHeight = 2048)
        {
            if (AtlasTable.ContainsKey(atlasName))
                throw new ArgumentException(String.Format("Atlas named {0} already loaded.", atlasName), "atlasName");

            if (!Directory.Exists(folderPath))
                throw new ArgumentException("Could not find resource folder \"" + folderPath + "\"");

            TextureAtlas atlas = new TextureAtlas(atlasName);
            foreach (var file in Directory.EnumerateFiles(folderPath))
            {
                string ext = Path.GetExtension(file);
                if (ext == ".jpg" || ext == ".png")
                {
                    string textureName = Path.GetFileNameWithoutExtension(file);
                    atlas.AddTexture((Bitmap)Bitmap.FromFile(file), textureName);
                }
            }
            atlas.PackTextures(maxWidth, maxHeight);
            AtlasTable.Add(atlasName.ToLower(), atlas);
        }

        public void LoadTileMap(string imagePath, string atlasName, int tileWidth, int tileHeight)
        {
            using (Bitmap image = (Bitmap)Image.FromFile(imagePath))
            {
                if (image != null)
                {
                    AtlasTable.Add(atlasName.ToLower(), new TextureAtlas(atlasName, image, tileWidth, tileHeight));
                }
            }
        }

        public Texture2D this[string atlasName, string textureName]
        {
            get 
            {
                atlasName = atlasName.ToLower();
                textureName = textureName.ToLower();

                if (!AtlasTable.ContainsKey(atlasName))
                    throw new ArgumentException("Atlas does not exist.", "atlasName");


                var texture = AtlasTable[atlasName][textureName];
                return texture; 
            }
        }
    }
}
