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
        private Vector2 _position = Vector2.Zero;
        public Vector2 Position { get => _position + (Parent?.Position ?? Vector2.Zero); set => _position = value; }
        private Vector2 _origin = Vector2.Zero;
        public Vector2 Origin { get => _origin + (Parent?.Origin ?? Vector2.Zero); set => _origin = value; }
        private Vector2 _scale = Vector2.One;
        public Vector2 Scale { get => _scale * (Parent?.Scale ?? Vector2.One); set => _scale = value; }
        private float _rotation = 0.0f;
        public float Rotation { get => _rotation + (Parent?.Rotation ?? 0.0f); set => _rotation = value; }
        private float _layerDepth = 0.0f;
        public float LayerDepth { get => _layerDepth + (Parent?.LayerDepth ?? 0.0f); set => _layerDepth = value; }
        private SpriteEffects _effects = SpriteEffects.None;
        public SpriteEffects Effects { get => _effects | (Parent?.Effects ?? _effects); set => _effects = value; }
        public Color Color { get; set; } = Color.White;
        public ITransformer Transformer { get; protected set; }

        public bool IsInitialized { get; protected set; }
        protected IInputService Input { get; private set; }

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
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            Input = Game.Services.GetService<IInputService>();
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
            Input = Game.Services.GetService<IInputService>();
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
    }
}
