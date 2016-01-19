using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine.Rendering
{
    public class RenderTarget
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color4 ClearColor { get; set; }

        private int FboHandle;
        private int ColorTexture;
        private static bool isFboBound = false;
        private static bool debug = false;

        public RenderTarget(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.ClearColor = Color4.Black;

            CreateColorBuffer();
            CreateFBO();
            CheckFboError();
        }

        private void CreateColorBuffer()
        {
            GL.GenTextures(1, out ColorTexture);
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            CheckGLError();
        }

        private void CreateFBO()
        {
            GL.Ext.GenFramebuffers(1, out FboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
            
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
            CheckGLError();
        }

        public void Bind()
        {
            if (isFboBound)
            {
                throw new Exception("A RenderTarget is already bound!");
            }

            isFboBound = true;
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
            //GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            //GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

            GL.ClearColor(ClearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SetViewport();
            CheckGLError();
        }

        private void SetViewport()
        {
            GL.PushAttrib(AttribMask.ViewportBit);
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Scale(1.0f, -1.0f, 1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void UnBind()
        {
            if (!isFboBound)
            {
                throw new Exception("RenderTarget is not bound!");
            }

            isFboBound = false;
            ResetViewport();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
            GL.DrawBuffer(DrawBufferMode.Back);
            CheckGLError();
        }

        private void ResetViewport()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Scale(1.0f, 1.0f, 1.0f);
            GL.MatrixMode(MatrixMode.Modelview);

            GL.PopAttrib();
        }

        public Texture2D Texture
        {
            get
            {
                Texture2D texture = new Texture2D(new System.Drawing.RectangleF(0, 0, 1, 1), ColorTexture);
                texture.Width = Width;
                texture.Height = Height;
                return texture;
            }
        }

        private static void CheckGLError()
        {
            if (debug)
            {
                var err = GL.GetError();
                if (err != ErrorCode.NoError)
                {
                    Console.WriteLine(err);
                }
            }
        }

        private static void CheckFboError()
        {
            if (debug)
            {
                var fboErr = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
                if (fboErr != FramebufferErrorCode.FramebufferComplete)
                {
                    Console.WriteLine(fboErr);
                }
            }
        }
    }
}
