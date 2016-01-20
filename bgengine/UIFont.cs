using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
namespace BGEngine
{
    public class UIFont
    {
        public class Glyph
        {
            public char Character { get; set; }
            public RectangleF TextureCoords { get; set; }
            public float PixelWidth { get; set; }
            public float PixelHeight { get; set; }

            public Glyph()
            {
                Character = ' ';
                PixelWidth = 0f;
                PixelHeight = 0f;
            }
        }

        public string FontFamily { get; private set; }
        public float PointSize { get; private set; }
        public float MaximumGlyphHeight { get; private set; }
        public Texture2D FontTexture { get; set; }
        public Dictionary<char, Glyph> Glyphs { get; set; }
        private static string DefaultCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$%^&*()-_=+;:'\",<.>/?\\|`";

        public UIFont(string fontFamily, float pointSize)
        {
            FontFamily = fontFamily;
            PointSize = pointSize;

            BuildFont();
        }

        /// <summary>
        /// Returns the size of a texture which will fit the entire Glyph map.The texture
        /// has power of 2 dimensions and width is guaranteed to be >= height.
        /// </summary>
        /// <param name="totalWidth">Total width of all glyphs</param>
        /// <param name="maxGlyphHeight">Maximum height of a single glyph</param>
        /// <param name="lineSpacing">Spacing between lines</param>
        /// <returns>Size of a texture which can hold all glyphs</returns>
        private Size GetMinimumPowerOfTwoSize(float totalWidth, float maxGlyphHeight, float lineSpacing)
        {
            // find the first power of two greater than total width
            int power = 1;
            int area = 2;
            while (area < totalWidth * (maxGlyphHeight + lineSpacing))
            {
                area *= 2;
                power++;
            }
            power++;
            return new Size((int)Math.Pow(2.0, Math.Ceiling((double)power / 2.0)),
                (int)Math.Pow(2.0, Math.Floor((double)power / 2.0)));            
        }

        private Bitmap BuildGlyphImage(Font font)
        {
            Glyphs = new Dictionary<char, Glyph>();

            float totalWidth = 0;
            MaximumGlyphHeight = 0;
            float interCharacterSpacing = 2;
            float lineSpacing = 2;

            using (Image measureImage = new Bitmap(1, 1))
            {
                using (Graphics measureGfx = Graphics.FromImage(measureImage))
                {
                    measureGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    foreach (char c in DefaultCharacterSet)
                    {
                        SizeF size = measureGfx.MeasureString(c.ToString(), font, 0, StringFormat.GenericTypographic);

                        Glyph glyph = new Glyph()
                        {
                            Character = c,
                            PixelWidth = (float)size.Width,
                            PixelHeight = (float)size.Height
                        };

                        totalWidth += glyph.PixelWidth + interCharacterSpacing;

                        if (glyph.PixelHeight > MaximumGlyphHeight)
                            MaximumGlyphHeight = glyph.PixelHeight;

                        Glyphs.Add(c, glyph);
                    }

                    CharacterRange[] ranges = { new CharacterRange(1, 1) };
                    StringFormat format = new StringFormat();
                    format.SetMeasurableCharacterRanges(ranges);
                    Region[] regions = measureGfx.MeasureCharacterRanges("a a", font, new Rectangle(0, 0, 1024, 1024), format);
                    RectangleF spaceRect = regions[0].GetBounds(measureGfx);

                    Glyph spaceGlyph = new Glyph()
                    {
                        Character = ' ',
                        PixelWidth = (float)spaceRect.Width,
                        PixelHeight = (float)spaceRect.Height
                    };
                    Glyphs.Add(' ', spaceGlyph);
                    totalWidth += spaceGlyph.PixelWidth;
                }
            }
            Size texSize = GetMinimumPowerOfTwoSize(totalWidth, MaximumGlyphHeight, lineSpacing);

            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Bitmap FontBitmap = new Bitmap(texSize.Width, texSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics drawGfx = Graphics.FromImage(FontBitmap))
            {
                drawGfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                drawGfx.Clear(Color.White);
                float xOffset = 0;
                float yOffset = 0;
                foreach (KeyValuePair<char, Glyph> glyph in Glyphs)
                {
                    if ((xOffset + glyph.Value.PixelWidth) > texSize.Width)
                    {
                        yOffset += MaximumGlyphHeight + lineSpacing;
                        xOffset = 0;
                    }

                    drawGfx.DrawString(glyph.Key.ToString(), font, drawBrush, new PointF((float)xOffset, (float)yOffset), StringFormat.GenericTypographic);                    
                    glyph.Value.TextureCoords = new RectangleF(xOffset / (float)texSize.Width, yOffset / (float)texSize.Height,
                        glyph.Value.PixelWidth / (float)texSize.Width, glyph.Value.PixelHeight / (float)texSize.Height);

                    xOffset += glyph.Value.PixelWidth + interCharacterSpacing;
                }
            }

            FontBitmap.Save("testfont.bmp");

            return FontBitmap;
        }

        private void BuildFont()
        {            
            Font font = new Font(FontFamily, PointSize);
            Bitmap FontBitmap = BuildGlyphImage(font);

            BitmapData bmpData = FontBitmap.LockBits(new Rectangle(0, 0, FontBitmap.Width, FontBitmap.Height), 
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelData = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmpData.Scan0, pixelData, 0, bmpData.Stride * bmpData.Height);
            for (int r = 0; r < bmpData.Height; ++r)
            {
                int rowOffset = r * bmpData.Stride;
                for (int c = 0; c < bmpData.Width; c += 1)
                { 
                    int offset = rowOffset + c * 4;
                    byte grayValue = (byte)(pixelData[offset] * 0.11 + pixelData[offset + 1] * 0.59 + pixelData[offset + 2] * 0.3);

                    pixelData[offset] = 255;
                    pixelData[offset + 1] = 255;
                    pixelData[offset + 2] = 255;
                    pixelData[offset + 3] = (byte)(255 - grayValue);
                }
            }

            Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length);

            int textureID = GL.GenTexture();
            FontTexture = new Texture2D(new RectangleF(0.0f, 0.0f, 1f, 1f), textureID);
            FontTexture.Width = bmpData.Width;
            FontTexture.Height = bmpData.Height;

            GL.BindTexture(TextureTarget.Texture2D, FontTexture.ID);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            FontBitmap.UnlockBits(bmpData);
            FontBitmap.Dispose();
        }

        public Vector2 MeasureString(string text)
        {
            if (text.Length == 0)
            {
                return new Vector2(0f, MaximumGlyphHeight);
            }

            float width = 0f;
            float height = 0f;

            foreach (char c in text)
            {
                Glyph glyph = null;
                if (Glyphs.TryGetValue(c, out glyph))
                {
                    width += glyph.PixelWidth;
                    if (glyph.PixelHeight > height)
                        height = glyph.PixelHeight;
                }
            }
            return new Vector2(width, height);
        }
    }
}
