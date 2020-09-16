using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Abstractions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// A root node for grouping sprites
    /// </summary>
    public class Canvas2d : SpriteBase
    {
        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public Canvas2d(Game game, SpriteBatch spriteBatch, ITransformer transformer) : base(game, spriteBatch, transformer) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // no op
        }
    }
}
