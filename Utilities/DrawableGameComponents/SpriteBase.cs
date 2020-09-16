using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Utilities.Abstractions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D objects
    /// </summary>
    public abstract class SpriteBase : DrawableGameComponent, ISprite
    {
        public ISprite Parent { get; private set; }
        public List<ISprite> Children { get; } = new List<ISprite>();

        /// <summary>
        /// SpriteBatch is only set when this node is made a root node.
        /// </summary>
        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch => _spriteBatch ?? Parent.SpriteBatch;

        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0.0f;
        public float LayerDepth { get; set; } = 0.0f;
        public Color Color { get; set; } = Color.White;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        public ITransformer Transformer { get; }

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        protected SpriteBase(Game game, SpriteBatch spriteBatch, ITransformer transformer) : base(game)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            Transformer = transformer;
            // root nodes must be added to the game components list to be included in the game loop
            Game.Components.Add(this);
        }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        protected SpriteBase(Game game, ISprite parent) : base(game)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Parent.Children.Add(this);
        }

        /// <summary>
        /// Draws this Sprite and recursively draws all decendents in its tree
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Begin and End are only called from the root node where the spritebatch is set
            if (!(_spriteBatch is null))
            {
                SpriteBatch.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    Transformer?.Transform ?? Matrix.Identity);
            }

            Draw(SpriteBatch);

            foreach (var child in Children) child.Draw(gameTime);

            base.Draw(gameTime);

            if (!(_spriteBatch is null))
            {
                SpriteBatch.End();
            }
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void DetachFromTree(SpriteBatch spriteBatch)
        {
            if (spriteBatch is null) throw new ArgumentNullException(nameof(spriteBatch));
            if (Parent is null) throw new InvalidOperationException("The root node cannot be removed from the tree.");

            Parent.Children.Remove(this);
            Parent = null;
            _spriteBatch = spriteBatch;
            // when this node becomes a root node it must be added to the game components list so that it will be included in the game loop
            Game.Components.Add(this);
        }

        public virtual void SetSpriteBatch(SpriteBatch spriteBatch)
        {
            if (spriteBatch is null) throw new ArgumentNullException(nameof(spriteBatch));
            if (!(Parent is null)) throw new InvalidOperationException("You cannot set the SpriteBatch on a child node.");

            _spriteBatch = spriteBatch;
        }

        public virtual void SetParent(ISprite parent)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));

            if (!(_spriteBatch is null)) Game.Components.Remove(this);

            _spriteBatch = null;
            // when this node stops being a root node it no longer needs to be included in the game loop
            Parent?.Children.Remove(this);
            Parent = parent;
        }
    }
}
