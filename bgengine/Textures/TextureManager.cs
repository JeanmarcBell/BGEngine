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
