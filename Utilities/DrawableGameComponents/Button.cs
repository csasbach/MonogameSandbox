using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Abstractions;
using Utilities.Extensions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// A 2D Sprite with mouse interactions
    /// </summary>
    public class Button : Sprite
    {
        private StringSprite _label;

        protected IInputService Input { get; private set; }
        public bool IsMouseOver { get; private set; }
        public bool IsMouseDown { get; private set; }
        public Texture2D MouseOverTexture { get; set; }
        public Texture2D MouseDownTexture { get; set; }
        public Texture2D MouseOutTexture { get; set; }
        private Color? _mouseOverColor;
        public Color? MouseOverColor { get => _mouseOverColor ?? Color; set => _mouseOverColor = value; }
        private Color? _mouseDownColor;
        public Color? MouseDownColor { get => _mouseDownColor ?? Color; set => _mouseDownColor = value; }
        private Color? _mouseOutColor;
        public Color? MouseOutColor { get => _mouseOutColor ?? Color; set => _mouseOutColor = value; }
        protected Color? TextMouseOverColor { get; set; }
        protected Color? TextMouseDownColor { get; set; }
        protected Color? TextMouseOutColor { get; set; }

        /// <summary>
        /// Root node constructor
        /// requires the SpriteBatch and ITransformer that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="transformer"></param>
        public Button(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            Input = Game.Services.GetService<IInputService>();
        }

        /// <summary>
        /// Child node constructor
        /// inherits SpriteBatch and ITransformer from its ancestor root node
        /// via the parent
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent"></param>
        public Button(Game game, ISprite parent) : base(game, parent)
        {
            Input = Game.Services.GetService<IInputService>();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsInBounds(Input.MousePosition))
            {
                IsMouseOver = true;
                Input.OnPressed(() => IsMouseDown = true, null, m => m.LeftButton);
                Input.OnReleased(() => IsMouseDown = false, null, m => m.LeftButton);
                if (_mouseOutColor is null) MouseOutColor = Color;
                if (TextMouseOutColor is null) TextMouseOutColor = _label?.Color;
                Color = IsMouseDown ? MouseDownColor : MouseOverColor;
                if (!(_label is null)) _label.Color = IsMouseDown ? TextMouseDownColor : TextMouseOverColor;
            }
            else
            {
                IsMouseDown = false;
                IsMouseOver = false;
                Color = MouseOutColor;
                if (!(_label is null)) _label.Color = TextMouseOutColor;
            }

            base.Update(gameTime);
        }

        public void OnClicked(Action del)
        {
            if (IsMouseDown)
            {
                Input.OnReleased(del, null, m => m.LeftButton);
            }
        }

        public Button SetColor(Color color, Color mouseOverColor, Color mouseDownColor)
        {
            MouseDownColor = mouseDownColor;
            return SetColor(color, mouseOverColor);
        }

        public Button SetColor(Color color, Color mouseOverColor)
        {
            MouseOverColor = mouseOverColor;
            return SetColor(color);
        }

        public Button SetColor(Color color)
        {
            Color = color;
            return this;
        }

        public Button SetLabel(SpriteFont logFont, Func<string> stringFunction, Color color, Color mouseOverColor, Color mouseDownColor)
        {
            TextMouseDownColor = mouseDownColor;
            return SetLabel(logFont, stringFunction, color, mouseOverColor);
        }

        public Button SetLabel(SpriteFont logFont, Func<string> stringFunction, Color color, Color mouseOverColor)
        {
            TextMouseOverColor = mouseOverColor;
            return SetLabel(logFont, stringFunction, color);
        }

        public Button SetLabel(SpriteFont logFont, Func<string> stringFunction, Color color)
        {
            var textSize = logFont.MeasureString(stringFunction());
            var textBoundingBox = new Rectangle(0, 0, textSize.X.ToInt(), textSize.Y.ToInt());
            _label = new StringSprite(Game, this)
            {
                Position = textBoundingBox.CenterInside(Texture.Bounds),
                SpriteFont = logFont,
                Text = stringFunction,
                Color = color
            };
            return this;
        }
    }
}
