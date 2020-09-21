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
        /// requires the Game and SpriteBatch that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        public StringSprite(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

        /// <summary>
        /// Child node constructor
        /// </summary>
        /// <param name="parent"></param>
        public StringSprite(ISprite parent) : base(parent)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            if (SpriteFont is null) return;

            DecomposeTransform(Transform, out var position, out var rotation, out var scale);
            spriteBatch.DrawString(SpriteFont, Text(), position, Color ?? Microsoft.Xna.Framework.Color.White, rotation, Origin, scale, Effects, LayerDepth);
        }
    }
}
