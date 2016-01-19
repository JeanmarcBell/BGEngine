using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Textures
{
    public sealed class AnimatedSprite : Sprite
    {
        private int frameIndex = 0;
        private double elapsed = 0.0d;
        private Animation2D animation;
        public bool IsRunning { get; set; }

        public AnimatedSprite(Animation2D animation)
        {
            this.animation = animation;
            IsRunning = true;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Reset()
        {
            IsRunning = false;
            elapsed = 0;
            frameIndex = 0;
        }

        protected override Vector2 TextureOffset
        {
            get { return new Vector2(animation[frameIndex].Texture.Width / 2, 
                animation[frameIndex].Texture.Height / 2); }
        }

        private void CheckFrameLifetime()
        {
            double elapsedMs = elapsed * 1000.0;
            double durationMs = animation[frameIndex].DurationMS;

            if (elapsedMs >= durationMs)
            {
                elapsed = (elapsedMs - durationMs) / 1000.0;
                frameIndex = (frameIndex + 1) % animation.Count;
            }
        }

        public override void Draw(double elapsed, SpriteBatch spriteBatch, Vector2 position, Color4 color, float rotation, float scale, Vector2 origin)
        {
            if (animation.Count > 0 && IsRunning)
            {
                this.elapsed += elapsed;

                CheckFrameLifetime();
                HandleDraw(spriteBatch, position, color, rotation, scale, origin);
            }
        }

        private void HandleDraw(SpriteBatch spriteBatch, Vector2 position, Color4 color, float rotation, float scale, Vector2 origin)
        {
            Animation2DFrame currentFrame = animation[frameIndex];

            Vector2 totalScale = new Vector2(currentFrame.Scale * scale);
            float totalRotation = rotation + currentFrame.Rotation;
            spriteBatch.DrawTexture(currentFrame.Texture, position - TextureOffset, color, totalRotation, totalScale, origin);
        }

        public static Sprite Make(string p)
        {
            Animation2D anim = ResourceManager.Animations["ocean"];
            return new AnimatedSprite(anim);
        }
    }
}
