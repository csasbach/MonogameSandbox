using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Utilities.Extensions
{
    public static class GraphicsDeviceExtensions
    {
        /// <summary>
        /// Creates a simple rectangle Texture2D
        /// </summary>
        /// <param name="device"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D CreateRectangleTexture(this GraphicsDevice device, int width, int height)
        {
            var texture = new Texture2D(device, width, height);
            var dataSize = width * height;
            var data = new Color[dataSize];
            for (var i = 0; i < dataSize; i++) data[i] = Microsoft.Xna.Framework.Color.White;
            texture.SetData(data);
            return texture;
        }
    }
}
