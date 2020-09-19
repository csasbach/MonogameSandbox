using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Extensions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D text
    /// </summary>
    public class StringSprite : SpriteBase
    {
        public SpriteFont SpriteFont { get; set; }
        public Func<string> Text { get; set; } = () => "";
        public override int Width => SpriteFont.MeasureString(Text()).X.ToInt();
        public override int Height => SpriteFont.MeasureString(Text()).Y.ToInt();

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public StringSprite(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public StringSprite(Game game, ISprite parent) : base(game, parent)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (SpriteFont is null) return;

            DecomposeTransform(Transform, out var position, out var rotation, out var scale);
            spriteBatch.DrawString(SpriteFont, Text(), position, Color ?? Microsoft.Xna.Framework.Color.White, rotation, Origin, scale, Effects, LayerDepth);
        }
    }
}
