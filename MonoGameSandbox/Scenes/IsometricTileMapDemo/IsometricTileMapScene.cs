using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.Scenes.TexturePackerDemo;
using MonoGameSandbox.SharedComponents;
using System.Diagnostics;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;
using Utilities.Services;

namespace MonoGameSandbox.Scenes.IsometricTileMapDemo
{
    [Scene("Isometric Tile Map Demo")]
    public class IsometricTileMapScene : Scene
    {
        private readonly ICameraService _camera;
        private readonly IInputService _input;
        private IsometricTileMap _isoTileMap;

        public IsometricTileMapScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(IsometricTileMapScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{2348A687-C155-468B-9B14-ED05EDD2F3EB}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _camera = Game.Services.GetService<ICameraService>();
            _input = Game.Services.GetService<IInputService>();
            Transformer = _camera;

            BackgroundColor = Microsoft.Xna.Framework.Color.CadetBlue;

            Logger?.LogTrace(scope, "{EE426881-0FEB-4BBC-83D3-1D70694AC7B0}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(IsometricTileMapScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{D443E5FC-EA3E-4F52-B586-16D21E69373E}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            LoadContentAsync(p =>
            {
                p.Report((0, "Creating heads up display..."));
                var hud = new Hud(Game, SpriteBatch);

                p.Report((33, "Adding content to heads up display..."));
                static string text() => "Some instructions go here";
                var position = new Vector2(Game.GraphicsDevice.Viewport.Width - (logFont.MeasureString(text()).X + 10), Game.GraphicsDevice.Viewport.Height - 30);
                hud.AddStringSprite(position, logFont, text);

                p.Report((66, "Registering heads up display..."));
                IndependentSprites.Add(hud);

                CreateIsometricTileMap();

                _camera.Enabled = true;
                _camera.Position = new Vector2(5310, 4475);
                _camera.Zoom = 0.6666f;

                p.Report((100, "Done!"));
            });

            Logger?.LogTrace(scope, "{FD5146DA-5B55-4334-8DE7-A662D07E7865}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{05971541-BA82-48E7-96C0-E18E9C6A1BF7}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        public override void Update(GameTime gameTime)
        {
            if (!LoadContentCompleted) return;

            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            base.Update(gameTime);
        }

        protected override void DrawMyContent(SpriteBatch spriteBatch)
        {

            base.DrawMyContent(spriteBatch);
        }

        protected override void UnloadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(IsometricTileMapScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{0046AF56-2079-4ED5-8A27-6F0F1B88A2FE}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            // unload stuff

            Logger?.LogTrace(scope, "{36722C17-0517-42BE-96B8-B57EB9980B58}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.UnloadContent();

            Logger?.LogTrace(scope, "{20B42479-6602-4350-9306-AAB52AE3EFBB}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateIsometricTileMap()
        {
            _isoTileMap = new IsometricTileMap(this,
                                "tileGuides/backCorner",
                                "tileGuides/backLeftWedge",
                                "tileGuides/backRightWedge",
                                "tileGuides/corner",
                                "tileGuides/leftCorner",
                                "tileGuides/rightCorner",
                                "tileGuides/leftWedge",
                                "tileGuides/charIncline",
                                "tileGuides/rightWedge",
                                "tileGuides/box",
                                "tileGuides/charFlat"
                            )
            {
                TileWidth = 314,
                TileHeight = 363,
                TileArrayWidth = 32,
                TileArrayHeight = 32,
                TileArrayLayers = 16,
            };

            /*
             * TODO:  The below should all be populated by some data file. 
             */

            // ground level
            for (var x = _isoTileMap.TileArrayWidth; x >= 0; x--)
            {
                for (var y = _isoTileMap.TileArrayHeight; y >= 0; y--)
                {
                    _isoTileMap.MapTextures("tileGuides/box", new Vector3(x, y, 0));
                }
            }

            // stuff above ground level...

            _isoTileMap.MapTextures("tileGuides/box"
                , new Vector3(15, 15, 1)
                , new Vector3(16, 15, 1)
                , new Vector3(17, 15, 1)

                , new Vector3(15, 16, 1)
                , new Vector3(16, 16, 1)
                , new Vector3(17, 16, 1)

                , new Vector3(15, 17, 1)
                , new Vector3(16, 17, 1)
                , new Vector3(17, 17, 1)



                , new Vector3(16, 16, 2)
                , new Vector3(17, 16, 2)

                , new Vector3(16, 17, 2)
                , new Vector3(17, 17, 2)



                , new Vector3(17, 17, 3)
            );

            _isoTileMap.MapTextures("tileGuides/leftWedge",
                new Vector3(15, 18, 1)
            );

            _isoTileMap.MapTextures("tileGuides/leftCorner",
                new Vector3(14, 18, 1)
            );

            _isoTileMap.MapTextures("tileGuides/leftBackWedge",
                new Vector3(14, 17, 1)
            );

            _isoTileMap.MapTextures("tileGuides/rightWedge",
                new Vector3(18, 15, 1)
            );

            _isoTileMap.MapTextures("tileGuides/rightCorner",
                new Vector3(18, 14, 1)
            );

            _isoTileMap.MapTextures("tileGuides/rightBackWedge",
                new Vector3(17, 14, 1)
            );

            _isoTileMap.MapTextures("tileGuides/charFlat"
                , new Vector3(17, 17, 4)
            );

            _isoTileMap.MapTextures("tileGuides/charFlat"
                , new Vector3(16, 17, 3)
            );

            _isoTileMap.MapTextures("tileGuides/charFlat",
                new Vector3(16, 18, 1)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline"
                , new Vector3(15, 18, 2)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline",
                new Vector3(14, 18, 2)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline",
                new Vector3(14, 17, 2)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline",
                new Vector3(18, 15, 2)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline",
                new Vector3(18, 14, 2)
            );

            _isoTileMap.MapTextures("tileGuides/charIncline",
                new Vector3(17, 14, 2)
            );
        }
    }
}
