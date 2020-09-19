using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.Scenes.Demo.DrawableGameComponents
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
            var camera = Game.Services.GetService<ICameraService>();
            var backgroundSprite = new Sprite(Game, this)
            {
                Texture = texture,
                Position = new Vector2(0, 0),
                Color = Microsoft.Xna.Framework.Color.Black,
                LayerDepth = 0.009999f
            };
            var signedIntegerFormat = @"+00000;-00000;+00000";
            var percentFormat = @"00000%";
            var degreesFormat = @"+000;-000;+000";
            var stringSprite = new StringSprite(Game, this)
            {
                SpriteFont = logFont,
                Text = () =>
                {
                    return _pause?.Paused ?? false ? "PAUSED" : $"CamPos:X={camera?.Position.X.ToString(signedIntegerFormat)},Y={camera?.Position.Y.ToString(signedIntegerFormat)} " +
                                        $"CamZoom:{camera?.Zoom.ToString(percentFormat)} " +
                                        $"CamRotDeg:{MathHelper.ToDegrees(camera?.Rotation ?? 0).ToString(degreesFormat)}";
                },
                Position = new Vector2(10, 10),
                LayerDepth = 0.000001f
            };

            base.LoadContent();
        }
    }
}
