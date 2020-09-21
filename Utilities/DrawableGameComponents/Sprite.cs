using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D images
    /// </summary>
    public class Sprite : SpriteBase
    {
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public override int Width => Texture.Width;
        public override int Height => Texture.Height;

        /// <summary>
        /// Root node constructor
        /// requires the Game and SpriteBatch that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        public Sprite(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

        /// <summary>
        /// Child node constructor
        /// </summary>
        /// <param name="parent"></param>
        public Sprite(ISprite parent) : base(parent) { }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture is null) return;

            DecomposeTransform(Transform, out var position, out var rotation, out var scale);
            spriteBatch.Draw(Texture, position, SourceRectangle, Color ?? Microsoft.Xna.Framework.Color.White, rotation, Origin, scale, Effects, LayerDepth);
        }
    }
}
