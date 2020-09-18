using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Base object for rendered 2D images
    /// </summary>
    public class Sprite : SpriteBase
    {
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; } = null;
        private Rectangle _destinationRectangle = Rectangle.Empty;
        public Rectangle DestinationRectangle
        {
            get
            {
                _destinationRectangle.X = (int)Math.Round(Position.X);
                _destinationRectangle.Y = (int)Math.Round(Position.Y);
                return _destinationRectangle;
            }
            set
            {
                Position = new Vector2(value.X, value.Y);
                _destinationRectangle = value;
            }
        }
        public bool IsMouseOver { get; private set; }
        private Color? _mouseOverColor;
        public Color MouseOverColor { get => _mouseOverColor ?? Color; set => _mouseOverColor = value; }
        private Color? _mouseOutColor;
        public Color MouseOutColor { get => _mouseOutColor ?? Color; set => _mouseOutColor = value; }

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public Sprite(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public Sprite(Game game, ISprite parent) : base(game, parent) { }

        public override void Update(GameTime gameTime)
        {
            if (IsInBounds(Input.MousePosition))
            {
                if (_mouseOutColor is null) MouseOutColor = Color;
                Color = MouseOverColor;
                IsMouseOver = true;
            }
            else
            {
                Color = MouseOutColor;
                IsMouseOver = false;
            }

            base.Update(gameTime);
        }

        private bool IsInBounds(Point point)
        {
            return point.X > Position.X &&
                point.Y > Position.Y &&
                point.X < DestinationRectangle.Width + Position.X &&
                point.Y < DestinationRectangle.Height + Position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture is null) return;
            spriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color, Rotation, Origin, Effects, LayerDepth);
        }
    }
}
