using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// A composite tree data structure that contains 2D drawable game components
    /// </summary>
    public interface ISprite
    {
        Vector2 Position { get; set; }
        int Width { get; }
        int Height { get; }
        Rectangle LocalBounds { get; }
        float Rotation { get; set; }
        Vector2 Origin { get; }
        Vector2 Scale { get; set; }
        /// <summary>
        /// Matrix used to calculate global position rotation and scale
        /// </summary>
        Matrix Transform { get; }
        float LayerDepth { get; set; }
        Color? Color { get; set; }
        SpriteEffects Effects { get; set; }
        Game Game { get; }
        /// <summary>
        /// The SpriteBatch that will be used to draw this sprite
        /// </summary>
        SpriteBatch SpriteBatch { get; }
        /// <summary>
        /// This sprite's parent in the sprite tree
        /// (null if this sprite is the root node)
        /// </summary>
        ISprite Parent { get; }
        /// <summary>
        /// The children of this node in the sprite tree
        /// (empty if this sprite is a leaf node)
        /// </summary>
        List<ISprite> Children { get; }
        bool IsInitialized { get; }

        /// <summary>
        /// The standard GameComponent override
        /// </summary>
        void Initialize();
        /// <summary>
        /// The standard Update override
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
        /// <summary>
        /// The standard DrawableGameComponent override
        /// </summary>
        /// <param name="gameTime"></param>
        void Draw(GameTime gameTime);
        /// <summary>
        /// A special draw method for ISprite where spriteBatch's Draw method will be called
        /// </summary>
        /// <param name="spriteBatch"></param>
        void Draw(SpriteBatch spriteBatch);
        /// <summary>
        /// Makes this sprite the root of its own sprite tree
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DetachFromTree(SpriteBatch spriteBatch);
        void SetSpriteBatch(SpriteBatch spriteBatch);
        void SetParent(ISprite parent);
        void ForceUnloadContent();
    }
}
