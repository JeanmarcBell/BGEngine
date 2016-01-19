using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class DrawHelper
    {
        #region DrawTexture
        /// <summary>
        /// The base DrawTexture function.
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destRectangle">The destination rectangle, in pixel space, where the texture is drawn</param>
        /// <param name="sourceRectangle">The source within the texture, values must be between [0-1]</param>
        /// <param name="color">The color to be applied</param>
        /// <param name="rotation">The rotation, in degrees to be applied</param>
        /// <param name="origin">The origin of the rotation, this value is in pixel space referenced from the top left of destRectangle</param>
        public static void DrawTexture(Texture2D texture, RectangleF destRectangle, RectangleF sourceRectangle, Color4 color, float rotation, Vector2 origin)
        {
            RectangleF atlasRect = texture.TextureCoordRect;
            RectangleF sourceInAtlasCoords = new RectangleF(atlasRect.Left + atlasRect.Width * sourceRectangle.Left, atlasRect.Top + atlasRect.Height * sourceRectangle.Top,
                atlasRect.Width * sourceRectangle.Width, atlasRect.Height * sourceRectangle.Height);

            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.Translate(destRectangle.Left + origin.X, destRectangle.Top + origin.Y, 0);
            GL.Rotate(rotation, 0.0f, 0.0f, 1.0f);
            GL.Translate(-origin.X, -origin.Y, 0);
            GL.BindTexture(TextureTarget.Texture2D, texture.ID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            float width = destRectangle.Width;
            float height = destRectangle.Height;
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(sourceInAtlasCoords.Left, sourceInAtlasCoords.Top);
            GL.Vertex2(0.0f, 0.0f);

            GL.TexCoord2(sourceInAtlasCoords.Right, sourceInAtlasCoords.Top);
            GL.Vertex2(width, 0.0f);

            GL.TexCoord2(sourceInAtlasCoords.Right, sourceInAtlasCoords.Bottom);
            GL.Vertex2(width, height);

            GL.TexCoord2(sourceInAtlasCoords.Left, sourceInAtlasCoords.Bottom);
            GL.Vertex2(0.0f, height);
            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
        }

        public static void DrawTexture(Texture2D texture, Vector2 position)
        {
            DrawTexture(texture, new RectangleF(position.X, position.Y, texture.Width, texture.Height), new RectangleF(0, 0, 1, 1), Color.White, 0f, Vector2.Zero);
        }

        public static void DrawTexture(Texture2D texture, Vector2 position, Color4 color)
        {
            DrawTexture(texture, new RectangleF(position.X, position.Y, texture.Width, texture.Height), new RectangleF(0, 0, 1, 1), color, 0f, Vector2.Zero);
        }

        public static void DrawTexture(Texture2D texture, Vector2 position, Color4 color, float rotation, Vector2 origin)
        {
            DrawTexture(texture, new RectangleF(position.X, position.Y, texture.Width, texture.Height), new RectangleF(0, 0, 1, 1), color, rotation, origin);
        }

        public static void DrawTexture(Texture2D texture, Vector2 position, Color4 color, float rotation, Vector2 origin, Vector2 scale)
        {
            DrawTexture(texture, 
                new RectangleF(position.X, position.Y, texture.Width*scale.X, texture.Height*scale.Y),
                new RectangleF(0, 0, 1, 1),  
                color, rotation, origin);
        }

        public static void DrawTexture(Texture2D texture, RectangleF destRectangle, RectangleF sourceRectangle, Color4 color)
        {
            DrawTexture(texture, destRectangle, sourceRectangle, color, 0f, Vector2.Zero);
        }

        public static void DrawTexture(Texture2D texture, RectangleF destRectangle, Color4 color)
        {
            DrawTexture(texture, destRectangle, new RectangleF(0, 0, 1, 1), color, 0f, Vector2.Zero);
        }
        #endregion

        public static void DrawString(UIFont font, string text, Vector2 location, Color4 color)
        {
            GL.Color3(0f, 0f, 0f);
            float offset = 0;
            foreach (char c in text)
            {
                UIFont.Glyph glyph = null;
                if (font.Glyphs.TryGetValue(c, out glyph))
                {
                    DrawTexture(font.FontTexture, new RectangleF(location.X + offset, location.Y, glyph.PixelWidth, glyph.PixelHeight), glyph.TextureCoords, color);
                    offset += glyph.PixelWidth;
                }
            }
        }

        public static void DrawLine(float x1, float y1, float x2, float y2, float thickness, Color4 color)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(thickness);

            float offset = 0.5f;

            GL.Color4(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(x1 + offset, y1 + offset);
            GL.Vertex2(x2 + offset, y2 + offset);
            GL.End();
        }

        public static void DrawLineStrip(IList<Vector2> points, Color4 color, float thickness, bool closePath = false)
        {
            if (points.Count < 2)
                throw new ArgumentException("Error. DrawLineStrip expects at least 2 points!");

            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(thickness);

            GL.Color4(color);
            GL.Begin(PrimitiveType.LineStrip);
            for (int n = 0; n < points.Count; ++n)
            {
                GL.Vertex2(points[n]);
            }
            if (points.Count > 2 && closePath)
                GL.Vertex2(points[0]);
            GL.End();
        }

        public static void DrawCircle(Vector2 center, float radius, Color4 color, float thickness, int numSegments = 36)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(thickness);

            GL.Color4(color);
            GL.Begin(PrimitiveType.LineStrip);

            double angleInc = Math.PI * 2.0 / numSegments;
            double angle = 0.0;

            for (int n=0 ; n<numSegments ; ++n)
            {
                Vector2 vec = new Vector2(center.X + (float)Math.Cos(angle) * radius, center.Y + (float)Math.Sin(angle) * radius);
                GL.Vertex2(vec);
                angle += angleInc;
            }
            GL.Vertex2(center.X + radius, center.Y);
            GL.End();
        }

        public static void DrawFilledDonut(Vector2 center, float innerRadius, float outerRadius, Color4 color, int numSegments = 36)
        {

            Vector2[] innerPoints = new Vector2[numSegments];
            Vector2[] outerPoints = new Vector2[numSegments];

            double angleInc = Math.PI * 2.0 / numSegments;
            double angle = 0.0;

            for (int n=0 ; n<numSegments ; ++n)
            {
                innerPoints[n] = new Vector2(center.X + (float)Math.Cos(angle) * innerRadius, center.Y + (float)Math.Sin(angle) * innerRadius);
                outerPoints[n] = new Vector2(center.X + (float)Math.Cos(angle) * outerRadius, center.Y + (float)Math.Sin(angle) * outerRadius);
                angle += angleInc;
            }

            GL.Color4(color);
            GL.Begin(PrimitiveType.Triangles);

            for (int n = 0; n < numSegments; ++n)
            {
                int nextIndex = (n + 1) % numSegments;
                GL.Vertex2(outerPoints[n]);
                GL.Vertex2(outerPoints[nextIndex]);
                GL.Vertex2(innerPoints[n]);

                GL.Vertex2(innerPoints[n]);
                GL.Vertex2(innerPoints[nextIndex]);
                GL.Vertex2(outerPoints[nextIndex]);
            }

            GL.End();
        }

        public static void DrawDonutSection(Vector2 center, float innerRadius, float outerRadius, float startAngleRad, float sweepRad, Color4 fill, Color4 outline, float lineThickness, int numSegments = 24)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(lineThickness);

            Vector2[] innerPoints = new Vector2[numSegments];
            Vector2[] outerPoints = new Vector2[numSegments];

            double angleInc = sweepRad / (numSegments - 1);
            double angle = startAngleRad;

            for (int n = 0; n < numSegments; ++n)
            {
                innerPoints[n] = new Vector2(center.X + (float)Math.Cos(angle) * innerRadius, center.Y + (float)Math.Sin(angle) * innerRadius);
                outerPoints[n] = new Vector2(center.X + (float)Math.Cos(angle) * outerRadius, center.Y + (float)Math.Sin(angle) * outerRadius);
                angle += angleInc;
            }

            GL.Color4(fill);
            GL.Begin(PrimitiveType.Triangles);

            for (int n = 0; n < numSegments - 1; ++n)
            {
                GL.Vertex2(outerPoints[n]);
                GL.Vertex2(outerPoints[n + 1]);
                GL.Vertex2(innerPoints[n]);

                GL.Vertex2(innerPoints[n]);
                GL.Vertex2(innerPoints[n + 1]);
                GL.Vertex2(outerPoints[n + 1]);
            }
            GL.End();

            GL.Color4(outline);
            GL.Begin(PrimitiveType.LineStrip);
            for (int n = 0; n < numSegments; ++n)
            {
                GL.Vertex2(innerPoints[n]);
            }
            for (int n = numSegments - 1; n >= 0; --n)
            {
                GL.Vertex2(outerPoints[n]);
            }
            GL.Vertex2(innerPoints[0]);
            GL.End();
        }

        public static void DrawRectangle(RectangleF rectangle, Color4 color, float thickness)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(thickness);

            float offset = 0.5f;

            GL.Color4(color);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex2(rectangle.Left + offset, rectangle.Top + offset);
            GL.Vertex2(rectangle.Right + offset, rectangle.Top + offset);
            GL.Vertex2(rectangle.Right + offset, rectangle.Bottom + offset);
            GL.Vertex2(rectangle.Left + offset, rectangle.Bottom + offset);
            GL.Vertex2(rectangle.Left + offset, rectangle.Top + offset);
            GL.End();
        }

        public static void FillRectangle(RectangleF rectangle, Color4 color)
        {
            GL.Disable(EnableCap.Texture2D);
            float offset = 0.5f;
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(rectangle.Left + offset, rectangle.Top + offset);
            GL.Vertex2(rectangle.Right + offset, rectangle.Top + offset);
            GL.Vertex2(rectangle.Right + offset, rectangle.Bottom + offset);
            GL.Vertex2(rectangle.Left + offset, rectangle.Bottom + offset);
            GL.End();
        }

        public static void DrawShaderQuad(RectangleF destRectangle)
        {
            Vector2 origin = Vector2.Zero;
            RectangleF texCoord = new RectangleF(0, 0, 1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.Translate(destRectangle.Left + origin.X, destRectangle.Top + origin.Y, 0);
            GL.Rotate(0, 0.0f, 0.0f, 1.0f);
            GL.Translate(-origin.X, -origin.Y, 0);

            float width = destRectangle.Width;
            float height = destRectangle.Height;
            GL.Color4(Color4.White);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoord.Left, texCoord.Top);
            GL.Vertex2(0.0f, 0.0f);

            GL.TexCoord2(texCoord.Right, texCoord.Top);
            GL.Vertex2(width, 0.0f);

            GL.TexCoord2(texCoord.Right, texCoord.Bottom);
            GL.Vertex2(width, height);

            GL.TexCoord2(texCoord.Left, texCoord.Bottom);
            GL.Vertex2(0.0f, height);
            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
        }

        public static void DrawTexture(int textureID, RectangleF destRectangle)
        {
            Vector2 origin = Vector2.Zero;
            RectangleF texCoord = new RectangleF(0,0,1,1);

            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.Translate(destRectangle.Left + origin.X, destRectangle.Top + origin.Y, 0);
            GL.Rotate(0, 0.0f, 0.0f, 1.0f);
            GL.Translate(-origin.X, -origin.Y, 0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            float width = destRectangle.Width;
            float height = destRectangle.Height;
            GL.Color4(Color4.White);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoord.Left, texCoord.Top);
            GL.Vertex2(0.0f, 0.0f);

            GL.TexCoord2(texCoord.Right, texCoord.Top);
            GL.Vertex2(width, 0.0f);

            GL.TexCoord2(texCoord.Right, texCoord.Bottom);
            GL.Vertex2(width, height);

            GL.TexCoord2(texCoord.Left, texCoord.Bottom);
            GL.Vertex2(0.0f, height);
            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
