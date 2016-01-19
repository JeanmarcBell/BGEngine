using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public Vector2 Position; // 8 bytes
        public Vector2 TexCoord; // 8 bytes
        public Int32 Color;
    }

    public static class VBOHelper
    {
        private static readonly Int32 VertexSize = (4 * sizeof(float)) + sizeof(Int32);

        public static int BindVertexBuffer(Vertex[] vertexData, int vboID = -1)
        {
            if (vboID == -1)
            {
                GL.GenBuffers(1, out vboID);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * VertexSize), vertexData, BufferUsageHint.StreamDraw);            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboID;
        }

        public static int BindIndexBuffer(ushort[] indicesData, int iboID = -1)
        {
            if (iboID == -1)
            {
                GL.GenBuffers(1, out iboID);
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicesData.Length * sizeof(ushort)), indicesData, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            return iboID;
        }

        /*
        public static int BindColorBuffer(Int32[] colorData, int cboID = -1)
        {
            if (cboID == -1)
            {
                GL.GenBuffers(1, out cboID);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, cboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorData.Length * sizeof(Int32)), colorData, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return cboID;
        }
        */
        
        public static void Bind(int vboID)
        {
            // Position Buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.VertexPointer(2, VertexPointerType.Float, VertexSize, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.VertexArray);

            // Texture Buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, VertexSize, (IntPtr)(2 * sizeof(float)));
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            // Color Buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexSize, (IntPtr)(4 * sizeof(float)));
            GL.EnableClientState(ArrayCap.ColorArray);
        }

        internal static void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }
    }
}
