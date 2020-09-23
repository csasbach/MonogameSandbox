using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.SharedComponents;
using System.Diagnostics;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;
using Utilities.Services;

namespace MonoGameSandbox.Scenes.Simple2dCameraDemo
{
    [Scene("Simple 2D Camera Demo")]
    public class Simple2dCameraDemoScene : Scene
    {
        private readonly ICameraService _camera;
        private readonly IInputService _input;

        public Simple2dCameraDemoScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(Simple2dCameraDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{82DBEF8D-2232-4999-B2E4-16A05F4D0439}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            // using service location pattern to access services in constructors
            // of game objects
            _camera = Game.Services.GetService<ICameraService>();
            _input = Game.Services.GetService<IInputService>();
            Transformer = _camera;

            BackgroundColor = Microsoft.Xna.Framework.Color.CadetBlue;

            Logger?.LogTrace(scope, "{C1952E44-54E8-43DF-AFF7-4913774D6609}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(Simple2dCameraDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{9B4A72CF-AB97-46D3-9825-5753EFE5E50B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            LoadContentAsync(p =>
            {
                p.Report((0, "Creating heads up display..."));
                // a scene can be responsible for instantiating
                // another root node, without having it be a child of the object insantiating it
                // if it makes sense for that node to be rendered independently, such as components
                // not rendered in the main camera view
                var hud = new Hud(Game, SpriteBatch);

                p.Report((20, "Adding content to heads up display..."));
                // we can add more content to the Hud if we want
                static string text() => "Camera: '(,)' Rotate | '<,>,ScroolWheel' Zoom | 'W,A,S,D,Arrow Keys' Move | 'R' Reset";
                var position = new Vector2(Game.GraphicsDevice.Viewport.Width - (logFont.MeasureString(text()).X + 10), Game.GraphicsDevice.Viewport.Height - 30);
                hud.AddStringSprite(position, logFont, text);

                p.Report((40, "Registering heads up display..."));
                // but then that node should be registered under IndependentSprites in the scene
                // so that its drawable game component methods will be called
                IndependentSprites.Add(hud);

                p.Report((60, "Enabling the camera..."));
                // camera is disabled by default to prevent it running during times when it should not be
                // so we need to explicitly enable it here
                _camera.Enabled = true;

                p.Report((80, "Creating something to look at..."));
                // just something to look at with the camera
                CreateBoxArray(logFont);

                p.Report((100, "Done!"));
            });

            Logger?.LogTrace(scope, "{1893C918-D4F2-4BF7-A10B-E560C5AFBEA7}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{959C9D69-85E1-463D-B107-37A859851087}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        public override void Update(GameTime gameTime)
        {
            if (!LoadContentCompleted) return;

            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(Simple2dCameraDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{9B4A72CF-AB97-46D3-9825-5753EFE5E50B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _camera.Enabled = false;

            Logger?.LogTrace(scope, "{1893C918-D4F2-4BF7-A10B-E560C5AFBEA7}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.UnloadContent();

            Logger?.LogTrace(scope, "{959C9D69-85E1-463D-B107-37A859851087}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateBoxArray(SpriteFont logFont)
        {
            using var scope = Logger?.BeginScope($"{nameof(Simple2dCameraDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{4E60DA2C-98EC-4411-9394-E911831BD5F2}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var arraySize = 20;
            var boxSize = 100;
            var gapSize = 10;
            var texture = GraphicsDevice.CreateRectangleTexture(boxSize, boxSize);
            var viewportHorizontalCenter = Game.GraphicsDevice.Viewport.Width * 0.5f;
            var viewportVerticalCenter = Game.GraphicsDevice.Viewport.Height * 0.5f;
            int boxArrayDimension = arraySize * boxSize + (arraySize + 1) * gapSize;
            var boxArrayCenter = boxArrayDimension * 0.5f;
            int position(int index, float center)
            {
                return ((boxSize + gapSize) * (index - 1) + (center - boxArrayCenter)).ToInt();
            }
            for (var x = arraySize; x > 0; x--)
            {
                for (var y = arraySize; y > 0; y--)
                {
                    var xPos = position(x, viewportHorizontalCenter);
                    var yPos = position(y, viewportVerticalCenter);
                    var box = this.AddSprite(new Vector2(xPos, yPos), texture);
                    var labelText = $"{x},{y}";
                    var label = box.AddStringSprite(Vector2.Zero, logFont, () => labelText);
                    label.Color = Microsoft.Xna.Framework.Color.Black;
                }
            }

            Logger?.LogTrace(scope, "{F2E2516F-F5A3-49CA-A601-A0E3867CDAD2}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
