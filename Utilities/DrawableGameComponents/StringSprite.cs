using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Abstractions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D text
    /// </summary>
    public class StringSprite : SpriteBase
    {
        public SpriteFont SpriteFont { get; set; }
        public Func<string> Text { get; set; } = () => "";

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public StringSprite(Game game, SpriteBatch spriteBatch, ITransformer transformer) : base(game, spriteBatch, transformer) { }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public StringSprite(Game game, ISprite parent) : base(game, parent) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (SpriteFont is null) return;
            spriteBatch.DrawString(SpriteFont, Text(), Position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
        }
    }
}
