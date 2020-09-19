using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.DrawableGameComponents;

namespace Utilities.Extensions
{
    public static class SpriteExtensions
    {
        public static Button AddButton(this ISprite sprite, Vector2 position, Vector2 size)
        {
            var texture = sprite.Game.GraphicsDevice.CreateRectangleTexture(size.X.ToInt(), size.Y.ToInt());
            return sprite.AddButton(position, texture);
        }

        public static Button AddButton(this ISprite sprite, Vector2 position, Texture2D texture)
        {
            return new Button(sprite.Game, sprite)
            {
                Position = position,
                Texture = texture
            };
        }
    }
}
