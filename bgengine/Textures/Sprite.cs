using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Textures
{
    public abstract class Sprite
    {
        public void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(elapsed, spriteBatch, position, Color4.White, 0.0f, 1.0f);
        }

        public void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position, Vector2 origin)
        {
            Draw(elapsed, spriteBatch, position, Color4.White, 0.0f, 1.0f, origin);
        }

        public void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position, Color4 color, float rotation, float scale)
        {
            Draw(elapsed, spriteBatch, position, color, rotation, scale, Vector2.Zero);
        }

        public abstract void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position, Color4 color, float rotation, float scale, Vector2 origin);
        protected abstract Vector2 TextureOffset { get; }
    }
}
