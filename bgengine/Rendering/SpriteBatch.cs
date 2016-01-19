using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class SpriteBatch
    {
        public bool IsStarted { get; private set; }

        private int maxSize;
        private RenderBatchObject[] renderBuffer;
        private Vertex[] vertexData;
        private int iboID = -1;
        private int vboID = -1;

        private int bufferPosition = 0;
        private bool IsBatching = false;

        public SpriteBatch(int maxSize = 2000)
        {
            this.maxSize = maxSize;
            int maxIndexSize = maxSize * 6;
            int offset = maxIndexSize % 6 != 0 ? (6 - maxIndexSize % 6) : 0;
            int arrSize = maxIndexSize + offset;
            ushort[] indicesData = new ushort[arrSize];
            for (int i = 0, a = 0; i < maxIndexSize; i += 6, a += 4)
            {
                indicesData[i] = (ushort)a;
                indicesData[i + 1] = (ushort)(a + 1);
                indicesData[i + 2] = (ushort)(a + 2);
                indicesData[i + 3] = (ushort)(a + 2);
                indicesData[i + 4] = (ushort)(a + 3);
                indicesData[i + 5] = (ushort)(a);
            }

            ushort val = indicesData[arrSize - 1];
            iboID = VBOHelper.BindIndexBuffer(indicesData);

            PreGenRenderBuffer();
            PreGenVertexBuffer();
        }

        private void PreGenVertexBuffer()
        {
            vertexData = new Vertex[4 * maxSize];
        }

        private void PreGenRenderBuffer()
        {
            renderBuffer = new RenderBatchObject[maxSize];
            for (int i = 0; i < maxSize; i++)
            {
                renderBuffer[i] = new RenderBatchObject();
            }
        }

        /// <summary>
        /// Clears all stored draw commands
        /// </summary>
        public void BeginBatch()
        {
            if (IsBatching)
                throw new Exception("Must call End before calling Begin!");

            bufferPosition = 0;
            IsBatching = true;
        }

        public void EndBatch()
        {
            if (!IsBatching)
                throw new Exception("Must call Begin before calling End!");

            if (bufferPosition > 0)
            {
                CreateVertexData();

                // Draw
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, renderBuffer[0].Texture.ID);
                VBOHelper.Bind(vboID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
                GL.DrawElements(PrimitiveType.Triangles, bufferPosition * 6, DrawElementsType.UnsignedShort, IntPtr.Zero);

                // Clear buffers
                GL.BindTexture(TextureTarget.Texture2D, 0);
                VBOHelper.Unbind();
                GL.Disable(EnableCap.Texture2D);
            }

            IsBatching = false;
        }

        private void CreateVertexData()
        {
            for (int i = 0; i < bufferPosition; i++)
            {
                var rbo = renderBuffer[i];
                int index = i * 4;

                vertexData[index].Position = rbo.TopLeft;
                vertexData[index + 1].Position = rbo.TopRight;
                vertexData[index + 2].Position = rbo.BottomRight;
                vertexData[index + 3].Position = rbo.BottomLeft;

                vertexData[index].TexCoord = rbo.Texture.TexCoords[0];
                vertexData[index + 1].TexCoord = rbo.Texture.TexCoords[1];
                vertexData[index + 2].TexCoord = rbo.Texture.TexCoords[2];
                vertexData[index + 3].TexCoord = rbo.Texture.TexCoords[3];

                vertexData[index].Color = rbo.Color;
                vertexData[index + 1].Color = rbo.Color;
                vertexData[index + 2].Color = rbo.Color;
                vertexData[index + 3].Color = rbo.Color;

                index += 4;
            }

            vboID = VBOHelper.BindVertexBuffer(vertexData, vboID);
        }

        #region Draw Requests
        private void AddDrawCommand(Texture2D texture, RectangleF destRectangle, Color4 color, float rotation)
        {
            if (bufferPosition >= renderBuffer.Length)
            {
                //TODO: Flush current buffer
                bufferPosition = 0;
            }
            RenderBatchObject batchObject = renderBuffer[bufferPosition];

            batchObject.SetTexture(texture, destRectangle);
            batchObject.Color = color.ToAbgr();
            batchObject.Rotation = rotation;
            bufferPosition++;
        }

        public void DrawTexture(Texture2D texture, RectangleF destRectangle, Color4 color, float rotation)
        {
            AddDrawCommand(texture, destRectangle, color, rotation);
        }

        public void DrawTexture(Texture2D texture, Vector2 position, Color4 color, float rotation, Vector2 scale, Vector2 origin)
        {
            float x = position.X + (texture.Width * origin.X) * (1 - scale.X);
            float y = position.Y + (texture.Height * origin.Y) * (1 - scale.Y);
            float width = texture.Width * scale.X;
            float height = texture.Height * scale.Y;

            DrawTexture(
                texture,
                new RectangleF(x, y, width, height),
                color,
                rotation);
        }

        public void DrawTexture(Texture2D texture, Vector2 position, Color4 color, float rotation, Vector2 scale)
        {
            DrawTexture(
                texture,
                position,
                color,
                rotation, 
                scale, 
                Vector2.Zero);
        }

        public void DrawTexture(Texture2D texture, Vector2 position, Color4 color)
        {
            DrawTexture(
                texture,
                new RectangleF(position.X, position.Y, texture.Width, texture.Height),
                color,
                0.0f);
        }

        public void DrawTexture(Texture2D texture, Vector2 position)
        {
            DrawTexture(
                texture,
                new RectangleF(position.X, position.Y, texture.Width, texture.Height),
                Color4.White,
                0.0f);
        }
        #endregion

        private class RenderBatchObject
        {
            public Texture2D Texture { get; private set; }
            public Int32 Color { get; set; }
            private RectangleF DestinationRectangle;
            public Vector2 TopLeft;
            public Vector2 TopRight;
            public Vector2 BottomRight;
            public Vector2 BottomLeft;

            private float _rotation;
            public float Rotation
            {
                get { return _rotation; }
                set
                {
                    _rotation = value;
                    CalculateRotation();
                }
            }

            public RenderBatchObject()
            {
                Color = Color4.White.ToAbgr();
                this.Texture = null;
            }

            public void SetTexture(Texture2D texture, RectangleF destinationRectangle)
            {
                this.Texture = texture;
                this.DestinationRectangle = destinationRectangle;
                CalculateRotation();
            }

            private void CalculateRotation()
            {
                if (Rotation == 0.0f)
                {
                    TopLeft.X = DestinationRectangle.Left;
                    TopLeft.Y = DestinationRectangle.Top;
                    TopRight.X = DestinationRectangle.Right;
                    TopRight.Y = DestinationRectangle.Top;
                    BottomLeft.X = DestinationRectangle.Left;
                    BottomLeft.Y = DestinationRectangle.Bottom;
                    BottomRight.X = DestinationRectangle.Right;
                    BottomRight.Y = DestinationRectangle.Bottom;
                    return;
                }

                Vector2 center = DestinationRectangle.Center();

                TopLeft = RotateAroundPoint(DestinationRectangle.Left, DestinationRectangle.Top, center, Rotation);
                TopRight = RotateAroundPoint(DestinationRectangle.Right, DestinationRectangle.Top, center, Rotation);
                BottomRight = RotateAroundPoint(DestinationRectangle.Right, DestinationRectangle.Bottom, center, Rotation);
                BottomLeft = RotateAroundPoint(DestinationRectangle.Left, DestinationRectangle.Bottom, center, Rotation);
            }

            private Vector2 RotateAroundPoint(float x, float y, Vector2 origin, double rotation)
            {
                float sin = (float)Math.Sin(rotation);
                float cos = (float)Math.Cos(rotation);

                float translatedX = x - origin.X;
                float translatedY = y - origin.Y;

                float rotatedX = translatedX * cos - translatedY * sin;
                float rotatedY = translatedX * sin + translatedY * cos;

                return new Vector2(rotatedX + origin.X, rotatedY + origin.Y);
            }
        }
    }
}
