using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// A root node for grouping sprites
    /// </summary>
    public abstract class Canvas2d : SpriteBase
    {
        /// <summary>
        /// Root node constructor
        /// requires the Game and SpriteBatch that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        protected Canvas2d(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }
    }
}
