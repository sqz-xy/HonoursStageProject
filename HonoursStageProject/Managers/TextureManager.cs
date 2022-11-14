using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace HonoursStageProject.Managers;

public static class TextureManager
{
        private static int _textureIndex;
        private static int[] _texture_IDs;

        private static TextureUnit _currentTextureUnit;
        private static List<string> _textureUnitAsString;
        
        public static void Initialize(int pTextureCount)
        {
            _texture_IDs = new int[pTextureCount];
            _textureIndex = 0;
            _currentTextureUnit = TextureUnit.Texture0;
            _textureUnitAsString = TextureUnitsToString();
        }

        /// <summary>
        /// Buffers and Binds texture data
        /// </summary>
        /// <param name="pFilePath">The path of the texture to bind</param>
        public static int BindTextureData(string pFilePath)
        {
            var filepath = @pFilePath;
            if (File.Exists(filepath))
            {            
                var TextureBitmap = new Bitmap(filepath);
                var TextureData = TextureBitmap.LockBits(
                new Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(_currentTextureUnit);
                GL.GenTextures(1, out _texture_IDs[_textureIndex]);
                GL.BindTexture(TextureTarget.Texture2D, _texture_IDs[_textureIndex]);
  
                GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
                TextureBitmap.UnlockBits(TextureData);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                _textureIndex++;
                IncrementTextureUnit();

                return _textureIndex - 1;
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }
        }

        /// <summary>
        /// Increments the current texture unit to the next one
        /// </summary>
        private static void IncrementTextureUnit()
        {
            var nextUnitString = _textureUnitAsString[_textureIndex];
            Enum.TryParse<TextureUnit>(nextUnitString, out var nextUnit);
            _currentTextureUnit = nextUnit;
        }

        /// <summary>
        /// Creates a list of strings pertaining to textureUnit enum values
        /// </summary>
        /// <returns>A list of strings</returns>
        private static List<string> TextureUnitsToString()
        {
            var textureUnits = new List<string>();
            foreach (TextureUnit textureUnit in Enum.GetValues(typeof(TextureUnit)))
            {
                textureUnits.Add(textureUnit.ToString());
            }
            return textureUnits;
        }

        /// <summary>
        /// Deletes the textures
        /// </summary>
        public static void ClearData()
        {
            _textureIndex = 0;
            Array.Clear(_texture_IDs);
            GL.DeleteTextures(_textureIndex, _texture_IDs);
            _currentTextureUnit = TextureUnit.Texture0;
            _textureUnitAsString = TextureUnitsToString();
        }
    }