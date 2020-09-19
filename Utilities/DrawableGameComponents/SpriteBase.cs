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
        public new Game Game { get; }
        public ISprite Parent { get; private set; }
        public List<ISprite> Children { get; } = new List<ISprite>();

        /// <summary>
        /// SpriteBatch is only set when this node is made a root node.
        /// </summary>
        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch => _spriteBatch ?? Parent.SpriteBatch;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public virtual int Width { get; }
        public virtual int Height { get; }
        public Rectangle LocalBounds => new Rectangle(0, 0, Width, Height);
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0.0f;
        public Matrix Transform => GetTransform();
        public float LayerDepth { get; set; } = 0.0f;
        public SpriteEffects Effects { get; set; }
        public Color? Color { get; set; }
        public bool IsInitialized { get; protected set; }
        /// <summary>
        /// An object that can apply an additional transfor matrix at render time
        /// </summary>
        protected ITransformer Transformer { get; set; }

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        protected SpriteBase(Game game, SpriteBatch spriteBatch) : base(game)
        {
            Game = game;
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
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
            Game = game;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Parent.Children.Add(this);
        }

        public override void Initialize()
        {
            IsInitialized = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // root nodes must be added to the game components list to be included in the game loop
            //
            // IMPORTANT!
            // An event is fired that calls LoadContent on components whenever they are added to a GameComponentCollection
            // therefore, I have opted to add the components here rather than in the consturctor, otherwise constructor
            // chains and hierarchy resolution order in related components can become a difficult to manage problem
            Game.Components.Add(this);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var child in Children)
            {
                child.Update(gameTime);
            }

            base.Update(gameTime);
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

            // if this node stops being a root node it no longer needs to be included in the game loop
            if (!(_spriteBatch is null))
            {
                Game.Components.Remove(this);
                _spriteBatch = null;
            }

            // if this node already had a parent, remove it from that parent's children
            Parent?.Children.Remove(this);

            Parent = parent;
        }

        public void ForceUnloadContent()
        {
            UnloadContent();
        }

        protected override void UnloadContent()
        {
            if (!(_spriteBatch is null))
            {
                Game.Components.Remove(this);
            }
            Parent?.Children.Remove(this);

            base.UnloadContent();
            Dispose();
        }

        /// <summary>
        /// Creates a matrix composed of origin, scale, rotation, position, and parent transform
        /// </summary>
        /// <returns></returns>
        protected Matrix GetTransform()
        {
            return Matrix.CreateTranslation(Origin.X, Origin.Y, 0.0f) *
                   Matrix.CreateScale(Scale.X, Scale.Y, 1.0f) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateTranslation(Position.X, Position.Y, 0.0f) *
                   Parent?.Transform ?? Matrix.Identity;
        }

        /// <summary>
        /// Determines if a point is between the globaly transformed bounds of this sprite
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected bool IsInBounds(Point point)
        {
            DecomposeTransform(GetTopLeftCorner(), out var topLeft, out _, out _);
            DecomposeTransform(GetBottomRightCorner(), out var bottomRight, out _, out _);
            return point.X > topLeft.X &&
                point.Y > topLeft.Y
                && point.X < bottomRight.X
                && point.Y < bottomRight.Y;
        }

        /// <summary>
        /// Breaks a transform down into its position, rotation, and scale
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        protected static void DecomposeTransform(Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
        {
            matrix.Decompose(out Vector3 scale3, out Quaternion rotationQ, out Vector3 position3);
            Vector2 direction = Vector2.Transform(Vector2.UnitX, rotationQ);
            rotation = (float)Math.Atan2(direction.Y, direction.X);
            position = new Vector2(position3.X, position3.Y);
            scale = new Vector2(scale3.X, scale3.Y);
        }

        protected Matrix GetTopLeftCorner()
        {
            return GetTransform();
        }

        protected Matrix GetBottomRightCorner()
        {
            return Matrix.CreateTranslation(Width, Height, 0.0f) * GetTransform();
        }
    }
}
