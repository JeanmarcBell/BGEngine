using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Textures
{
    public sealed class StaticSprite : Sprite
    {
        public Texture2D Texture { get; private set; }

        public StaticSprite(Texture2D texture)
        {
            this.Texture = texture;
        }

        protected override Vector2 TextureOffset 
        { 
            get 
            {
                return new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
        }

        public override void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position, OpenTK.Graphics.Color4 color, float rotation, float scale, Vector2 origin)
        {
            Vector2 scaleVec = new Vector2(scale);
            spriteBatch.DrawTexture(Texture, position - TextureOffset, color, rotation, scaleVec, origin);
        }
    }
}
