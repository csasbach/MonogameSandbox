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
            var topBackground = this.AddSprite(Vector2.Zero, texture);
            topBackground.Color = Microsoft.Xna.Framework.Color.Black;
            var camera = Game.Services.GetService<ICameraService>();
            var signedIntegerFormat = @"+00000;-00000;+00000";
            var percentFormat = @"00000%";
            var degreesFormat = @"+000;-000;+000";
            string text()
            {
                return _pause?.Paused ?? false ?
                "PAUSED" : $"CamPos:X={camera?.Position.X.ToString(signedIntegerFormat)},Y={camera?.Position.Y.ToString(signedIntegerFormat)} " +
                $"CamZoom:{camera?.Zoom.ToString(percentFormat)} " +
                $"CamRotDeg:{MathHelper.ToDegrees(camera?.Rotation ?? 0).ToString(degreesFormat)}";
            };
            topBackground.AddStringSprite(new Vector2(10, 10), logFont, text);
        }

        private void CreateBottomBar(SpriteFont logFont, Texture2D texture)
        {
            var bottomBackground = this.AddSprite(new Vector2(0, GraphicsDevice.Viewport.Height - 40), texture);
            bottomBackground.Color = Microsoft.Xna.Framework.Color.Black;
            bottomBackground.AddStringSprite(new Vector2(10, 10), logFont, () => "'Space' Pause | 'E' Main Menu | 'Esc' Exit Game");
        }
    }
}
