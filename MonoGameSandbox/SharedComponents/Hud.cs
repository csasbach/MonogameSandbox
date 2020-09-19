using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.SharedComponents
{
    public sealed class Hud : Canvas2d
    {
        private readonly IPauseService _pause;

        public Hud(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            _pause = Game.Services.GetService<IPauseService>();
        }

        protected override void LoadContent()
        {
            var logFont = Game.Content.Load<SpriteFont>("LogFont");
            var texture = Game.GraphicsDevice.CreateRectangleTexture(GraphicsDevice.Viewport.Width, 40);

            CreateTopBar(logFont, texture);

            CreateBottomBar(logFont, texture);

            base.LoadContent();
        }

        private void CreateTopBar(SpriteFont logFont, Texture2D texture)
        {
            var topBackGround = new Sprite(Game, this)
            {
                Texture = texture,
                Position = new Vector2(0, 0),
                Color = Microsoft.Xna.Framework.Color.Black,
                LayerDepth = 0.009999f
            };
            var camera = Game.Services.GetService<ICameraService>();
            var signedIntegerFormat = @"+00000;-00000;+00000";
            var percentFormat = @"00000%";
            var degreesFormat = @"+000;-000;+000";
            var stringSprite = new StringSprite(Game, this)
            {
                SpriteFont = logFont,
                Text = () =>
                {
                    return _pause?.Paused ?? false ? 
                    "PAUSED" : $"CamPos:X={camera?.Position.X.ToString(signedIntegerFormat)},Y={camera?.Position.Y.ToString(signedIntegerFormat)} " +
                    $"CamZoom:{camera?.Zoom.ToString(percentFormat)} " +
                    $"CamRotDeg:{MathHelper.ToDegrees(camera?.Rotation ?? 0).ToString(degreesFormat)}";
                },
                Position = new Vector2(10, 10),
                LayerDepth = 0.000001f
            };
        }

        private void CreateBottomBar(SpriteFont logFont, Texture2D texture)
        {
            var bottomBackground = new Sprite(Game, this)
            {
                Texture = texture,
                Position = new Vector2(0, GraphicsDevice.Viewport.Height - 40),
                Color = Microsoft.Xna.Framework.Color.Black,
                LayerDepth = 0.009999f
            };
            var bottomStringSprite = new StringSprite(Game, bottomBackground)
            {
                SpriteFont = logFont,
                Text = () => "'Space' Pause | 'E' Main Menu | 'Esc' Exit Game",
                Position = new Vector2(10, 10)
            };
        }
    }
}
