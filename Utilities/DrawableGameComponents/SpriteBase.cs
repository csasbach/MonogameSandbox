using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utilities.Abstractions;
using Utilities.Services;

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
        protected LoggerService Logger { get; }

        /// <summary>
        /// Root node constructor
        /// requires the Game and SpriteBatch that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        protected SpriteBase(Game game, SpriteBatch spriteBatch) : base(game)
        {
            Game = game;
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

            Logger = Game.Services.GetService<LoggerService>();
        }

        /// <summary>
        /// Child node constructor
        /// </summary>
        /// <param name="parent"></param>
        protected SpriteBase(ISprite parent) : base(parent.Game)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Parent.Children.Add(this);
            Game = Parent.Game;

            Logger = Game.Services.GetService<LoggerService>();
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
            if (!(_spriteBatch is null))
            {
                if (!Game.Components.Contains(this)) Game.Components.Add(this);
            }

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

            DrawMyContent(SpriteBatch);

            foreach (var child in Children) child.Draw(gameTime);

            base.Draw(gameTime);

            if (!(_spriteBatch is null))
            {
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Override this method with the code for rendering the sprite implementation
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawMyContent(SpriteBatch spriteBatch)
        {
            // here this is a no nop
            // but any child class that renders its own content
            // will need to call spriteBatch.Draw or spriteBatch.DrawString here
        }

        public virtual void DetachFromTree(SpriteBatch spriteBatch)
        {
            using (var scope = Logger?.BeginScope($"{nameof(SpriteBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{91FC4B95-C782-4914-976A-791D26E2F00C}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                if (spriteBatch is null) throw new ArgumentNullException(nameof(spriteBatch));
                if (Parent is null) throw new InvalidOperationException("A root node cannot be removed from the tree.");

                Parent.Children.Remove(this);
                Parent = null;
                _spriteBatch = spriteBatch;
                // when this node becomes a root node it must be added to the game components list so that it will be included in the game loop
                if (!Game.Components.Contains(this)) Game.Components.Add(this);

                Logger?.LogTrace(scope, "{C9F06B17-3D3C-4E56-BC10-34A51FE530DB}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public virtual void SetSpriteBatch(SpriteBatch spriteBatch)
        {
            using (var scope = Logger?.BeginScope($"{nameof(SpriteBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{0D4E0C37-A56C-4173-8086-E34D6E36DE61}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                if (spriteBatch is null) throw new ArgumentNullException(nameof(spriteBatch));
                if (!(Parent is null)) throw new InvalidOperationException("You cannot set the SpriteBatch on a child node.");

                _spriteBatch = spriteBatch;

                Logger?.LogTrace(scope, "{89203EFD-7BD8-43BC-9305-C6CFCD2FD1C3}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public virtual void SetParent(ISprite parent)
        {
            using (var scope = Logger?.BeginScope($"{nameof(SpriteBase)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{21B5B113-85D0-46D4-A65E-C6BA2578FA1F}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                if (parent is null) throw new ArgumentNullException(nameof(parent));

                if (!(_spriteBatch is null))
                {
                    // if this node was a root, setting its parent makes it
                    // no longer a root, so it should have its SpriteBatch unset
                    // and this node should be removed from the Game's components list
                    Game.Components.Remove(this);
                    _spriteBatch = null;
                }

                // if this node already had a parent, remove it from that parent's children
                Parent?.Children.Remove(this);

                Parent = parent;

                Logger?.LogTrace(scope, "{31D0103C-91F0-4ED9-97D1-A929DD50294D}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public void ForceUnloadContent()
        {
            // if this is a root node then 
            // remove it from the game coponents list
            if (!(_spriteBatch is null))
            {
                // NOTE: This will end up firing an event that
                // results in UnloadContent being called but we
                // can neither wait nor rely on that if we want
                // this to behave nicely
                Game.Components.Remove(this);
            }

            // this ensures we don't try to call UnloadContent
            // after it has already been called for this sprite
            if (isUnloading) return;

            // this ensures UnloadContent is called synchronously
            // and for non-root nodes as well
            UnloadContent();
        }

        protected bool isUnloading = false;
        protected override void UnloadContent()
        {
            // this mechanism will prevent follow-up
            // events such as that fired by the GameComponentsCollection
            // from trying to Unload the content more than once
            if (isUnloading) return;
            isUnloading = true;

            // remove this from its parent
            Parent?.Children.Remove(this);

            // unload and remove all of its children
            var sweep = new ISprite[Children.Count];
            Children.CopyTo(sweep);
            foreach (var child in sweep)
            {
                Children.ElementAt(Children.IndexOf(child)).ForceUnloadContent();
                Children.Remove(child);
            }

            // perform base unload duties
            base.UnloadContent();

            // make sure this thing is completely removed from
            // memory as there are many ways in which references
            // to it and its children might potentially linger otherwise
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
