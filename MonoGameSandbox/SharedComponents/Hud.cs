using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;
using Utilities.Services;

namespace MonoGameSandbox.SharedComponents
{
    public sealed class Hud : Canvas2d
    {
        private readonly IPauseService _pause;

        public Hud(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(Hud)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{9ED1A643-52EB-428B-A74F-A7DDA019B26F}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _pause = Game.Services.GetService<IPauseService>();

            Logger?.LogTrace(scope, "{90D04547-9299-4A06-A463-A0743B838A50}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(Hud)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{4D3CBE8C-E02B-4F3A-A80B-4F25750AD595}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var logFont = Game.Content.Load<SpriteFont>("LogFont");
            var texture = Game.GraphicsDevice.CreateRectangleTexture(GraphicsDevice.Viewport.Width, 40);

            CreateTopBar(logFont, texture);

            CreateBottomBar(logFont, texture);

            Logger?.LogTrace(scope, "{B97612E9-35E4-428E-B7E3-1222BC4676A7}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{CA4364CE-DF9F-41FA-AE71-75572B97F0C8}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateTopBar(SpriteFont logFont, Texture2D texture)
        {
            using var scope = Logger?.BeginScope($"{nameof(Hud)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{8F2B0F6E-2FAF-4D9A-85D1-BC66AA7DCD6B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

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

            Logger?.LogTrace(scope, "{CF52AEF1-BFCC-442B-B509-F9023F1111F4}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateBottomBar(SpriteFont logFont, Texture2D texture)
        {
            using var scope = Logger?.BeginScope($"{nameof(Hud)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{8E5DF959-36A9-4259-98A6-659CE582B6B9}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var bottomBackground = this.AddSprite(new Vector2(0, GraphicsDevice.Viewport.Height - 40), texture);
            bottomBackground.Color = Microsoft.Xna.Framework.Color.Black;
            bottomBackground.AddStringSprite(new Vector2(10, 10), logFont, () => "'Space' Pause | 'E' Main Menu | 'Esc' Exit Game");

            Logger?.LogTrace(scope, "{44F5B439-9FD9-4162-A6E3-F3A72E9489C9}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
