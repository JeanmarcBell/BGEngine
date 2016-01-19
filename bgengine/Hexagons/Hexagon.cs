using BGEngine;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BGEngine.Hexagons
{
    public class Hexagon
    {
        public readonly static int HexagonSize = 32;
        public static float HexagonWidth { get { return (int)(HexagonHeight * Math.Sqrt(3) / 2); } }
        public static float HexagonHeight { get { return HexagonSize * 2; } }
        public static float VerticalSpacing { get { return HexagonHeight * 3 / 4; } }
        public static float HorizontalSpacing { get { return HexagonWidth; } }

        internal int[] BorderDirection;
        private int BorderWidth = 0;
        internal int[] colorData;
        private int borderVboID = -1;
        private int borderIboID = -1;
        private int borderIndexLength = 0;
        private bool borderChanged = false;

        public Hexagon(Color4 baseColor)
        {
            colorData = Enumerable.Repeat(baseColor.ToAbgr(), 12).ToArray();
            BorderDirection = Enumerable.Repeat(0, 6).ToArray();

            if (baseVboID == -1)
            {
                GenerateBaseIBO();
                GenerateBaseVBO();
            }
        }

        #region BaseLine
        private static int baseVboID = -1;
        private static int baseIboID = -1;

        private void GenerateBaseIBO()
        {
            ushort[] indexData = new ushort[12];

            for (ushort i = 0; i < 6; i++)
            {
                indexData[i * 2] = (ushort)(i * 2);
                indexData[(i * 2) + 1] = (ushort)((i * 2) + 1);
            }

            baseIboID = VBOHelper.BindIndexBuffer(indexData);
        }

        private void GenerateBaseVBO()
        {
            Vertex[] vertexData = new Vertex[12];
            Int32 color = new Color4(0.2f, 0.2f, 0.2f, 0.3f).ToAbgr();

            for (ushort i = 0; i < 6; i++)
            {
                //TODO: use the pair here as a line and shift the border perpendicular to this line in the direction of center, but not towards center itself
                // This should be more accurate then the current method
                vertexData[i * 2] = new Vertex()
                {
                    Position = polar(HexagonSize, i),
                    Color = color
                };
                vertexData[(i * 2) + 1] = new Vertex()
                {
                    Position = polar(HexagonSize, (i + 1) % 6),
                    Color = color
                };
            }

            baseVboID = VBOHelper.BindVertexBuffer(vertexData, baseVboID);
        }
        #endregion

        #region Borders
        private void GenerateBorder()
        {
            ushort[] indexData = new ushort[12];
            Vertex[] vertexData = new Vertex[12];

            for (ushort i = 0, j = 0; i < 6; i++)
            {
                if (BorderDirection[i] != 0)
                {
                    vertexData[i * 2] = new Vertex()
                    {
                        Position = polar(HexagonSize, i),
                        Color = colorData[i * 2]
                    };
                    vertexData[(i * 2) + 1] = new Vertex()
                    {
                        Position = polar(HexagonSize, (i + 1) % 6),
                        Color = colorData[(i * 2) + 1]
                    };

                    indexData[j * 2] = (ushort)(i * 2);
                    indexData[(j * 2) + 1] = (ushort)((i * 2) + 1);
                    borderIndexLength += 2;
                    j++;
                }
            }

            borderIboID = VBOHelper.BindIndexBuffer(indexData, borderIboID);
            borderVboID = VBOHelper.BindVertexBuffer(vertexData, borderVboID);
            borderChanged = false;
        }

        public void ClearBorder(int direction)
        {
            BorderDirection[direction % 6] = 0;
            borderChanged = true;
        }

        public void ClearBorderAll()
        {
            BorderDirection.Fill(0);
        }

        public void ModifyBorder(Color4 color, int lineWidth, int direction)
        {
            int c = color.ToAbgr();
            colorData[direction * 2] = c;
            colorData[(direction * 2) + 1] = c;
            BorderDirection[direction] = 1;
            BorderWidth = lineWidth;
            borderChanged = true;
        }

        public void ModifyBorderAll(Color4 color, int lineWidth)
        {
            colorData = Enumerable.Repeat(color.ToAbgr(), 12).ToArray();
            BorderDirection = Enumerable.Repeat(1, 6).ToArray();
            BorderWidth = lineWidth;
            borderChanged = true;
        }
        #endregion

        public void Draw(Vector2 center)
        {
            DrawBase(center);
            DrawBorder(center);
        }

        private void DrawBase(Vector2 center)
        {
            GL.PushMatrix();
            GL.LineWidth(3);
            GL.Translate(center.X, center.Y, 0);

            VBOHelper.Bind(baseVboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, baseIboID);
            GL.DrawElements(PrimitiveType.Lines, 12, DrawElementsType.UnsignedShort, IntPtr.Zero);
            VBOHelper.Unbind();
            GL.PopMatrix();
        }

        private void DrawBorder(Vector2 center)
        {
            if (borderChanged)
                GenerateBorder();

            if (borderIndexLength > 0)
            {
                GL.PushMatrix();
                GL.LineWidth(BorderWidth);
                GL.Translate(center.X, center.Y, 0);

                VBOHelper.Bind(borderVboID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, borderIboID);
                GL.DrawElements(PrimitiveType.Lines, borderIndexLength, DrawElementsType.UnsignedShort, IntPtr.Zero);
                VBOHelper.Unbind();
                GL.PopMatrix();
            }
        }

        private Vector2 polar(int hexagonSize, int corner)
        {
            double angle = Math.PI / 3 * (corner + 0.5);
            float x = (float)(hexagonSize * Math.Cos(angle));
            float y = (float)(hexagonSize * Math.Sin(angle));
            Vector2 vec = new Vector2(x, y);
            return vec;
        }
    }
}