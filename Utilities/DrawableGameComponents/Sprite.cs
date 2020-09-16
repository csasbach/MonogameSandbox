using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Abstractions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D images
    /// </summary>
    public class Sprite : SpriteBase
    {
        public Texture2D Texture { get; set; }
        public Rectangle DestinationRectangle { get; set; } = Rectangle.Empty;
        public Rectangle? SourceRectangle { get; set; } = null;

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public Sprite(Game game, SpriteBatch spriteBatch, ITransformer transformer) : base(game, spriteBatch, transformer) { }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public Sprite(Game game, ISprite parent) : base(game, parent) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture is null) return;
            spriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color, Rotation, Origin, Effects, LayerDepth);
        }
    }
}
