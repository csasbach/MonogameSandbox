using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            return new Button(sprite)
            {
                Position = position,
                Texture = texture,
                LayerDepth = 1
            };
        }

        public static Sprite AddSprite(this ISprite sprite, Vector2 position, Vector2 size)
        {
            var texture = sprite.Game.GraphicsDevice.CreateRectangleTexture(size.X.ToInt(), size.Y.ToInt());
            return sprite.AddSprite(position, texture);
        }

        public static Sprite AddSprite(this ISprite sprite, Vector2 position, Texture2D texture)
        {
            return new Sprite(sprite)
            {
                Position = position,
                Texture = texture,
                LayerDepth = 0.00001f
            };
        }

        public static StringSprite AddStringSprite(this ISprite sprite, Vector2 position, SpriteFont font, Func<string> textFunc)
        {
            return new StringSprite(sprite)
            {
                Position = position,
                SpriteFont = font,
                Text = textFunc,
                LayerDepth = 0.000005f
            };
        }
    }
}
