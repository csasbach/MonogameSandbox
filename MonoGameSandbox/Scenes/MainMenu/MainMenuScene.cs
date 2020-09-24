using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameSandbox.Scenes.Simple2dCameraDemo;
using MonoGameSandbox.Scenes.SimpleSaveDemo;
using MonoGameSandbox.Scenes.SomeOtherDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;
using Utilities.Services;

namespace MonoGameSandbox.Scenes.MainMenu
{
    [Scene("Main Menu")]
    public class MainMenuScene : Scene
    {
        private readonly Dictionary<Type, Button> _sceneLinks = new Dictionary<Type, Button>();

        public MainMenuScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{4B90083B-F0E0-4C84-9184-8527908D0392}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            BackgroundColor = Microsoft.Xna.Framework.Color.Black;
            InitializeSceneLinks();

            Logger?.LogTrace(scope, "{75F7347E-11C9-4B5E-B596-C155BE30FC54}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{C983C39D-19A8-4F37-8362-36706621536D}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            LoadContentAsync(p =>
            {
                p.Report((0, "Loading Menu font..."));
                var titleFont = Game.Content.Load<SpriteFont>("MenuTitleFont");
                p.Report((20, "Loading Log font..."));
                var logFont = Game.Content.Load<SpriteFont>("LogFont");
                p.Report((40, "Reticulating splines..."));
                var viewportCenter = Game.GraphicsDevice.Viewport.Bounds.GetCenter();
                p.Report((60, "Creating title..."));
                CreateTitle(titleFont, viewportCenter);
                p.Report((80, "Creating scene links..."));
                CreateButtonArray(logFont, viewportCenter);
                p.Report((100, "Loading Complete!"));
            });

            Logger?.LogTrace(scope, "{627638E6-C88C-4D3C-BE6D-6FCF02445954}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{D111AC5B-8A9C-4A9A-B413-8826829908D5}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        public override void Update(GameTime gameTime)
        {
            if (!LoadContentCompleted) return;

            // effectively disabling pause for this scene
            Pause.Paused = false;

            foreach (var link in _sceneLinks)
            {
                link.Value.OnClicked(() => GameState.SetGameState(link.Key, SpriteBatch));
            }

            base.Update(gameTime);
        }

        private void InitializeSceneLinks()
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{98F8AC0D-7455-4E71-BE1D-111263933ADC}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _sceneLinks[typeof(Simple2dCameraDemoScene)] = null;
            _sceneLinks[typeof(SimpleSaveDemoScene)] = null;
            _sceneLinks[typeof(Demo02)] = null;
            _sceneLinks[typeof(Demo03)] = null;
            _sceneLinks[typeof(Demo04)] = null;
            _sceneLinks[typeof(Demo05)] = null;
            _sceneLinks[typeof(Demo06)] = null;
            _sceneLinks[typeof(Demo07)] = null;
            _sceneLinks[typeof(Demo08)] = null;

            Logger?.LogTrace(scope, "{647D63CB-4FCA-4BC3-8F0E-74888BFE5FA4}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateTitle(SpriteFont titleFont, Vector2 viewportCenter)
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{C9424696-E23C-46AA-9917-BD842B7E82DD}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            static string text() => "Select a Demo";
            var titleWidth = titleFont.MeasureString(text()).X;
            this.AddStringSprite(new Vector2(viewportCenter.X - titleWidth * 0.5f, 70), titleFont, text);

            Logger?.LogTrace(scope, "{D13CCB04-E6B3-4B8E-A944-F2031EC19637}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateButtonArray(SpriteFont logFont, Vector2 viewportCenter)
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{E1F74DD6-78F8-465C-9814-1CE2818D7442}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var buttonWidth = 500;
            var buttonHeight = 60;
            var gutter = 30;
            var buttonArrayBounds = new Rectangle(gutter, 200,
                Game.GraphicsDevice.Viewport.Bounds.Width - gutter,
                Game.GraphicsDevice.Viewport.Bounds.Height - gutter);
            var index = _sceneLinks.ToList();
            var x = buttonArrayBounds.X;
            var y = buttonArrayBounds.Y;
            var actualButtonArrayWidth = buttonArrayBounds.Width;
            foreach (var link in index)
            {
                if (x + buttonWidth > buttonArrayBounds.Width)
                {
                    // calculated for centering later
                    actualButtonArrayWidth = x - gutter;

                    // start drawing buttons at the beginning of next row
                    x = buttonArrayBounds.X;
                    y = y + buttonHeight + gutter;

                    // if you've gone too far
                    if (y + buttonHeight > buttonArrayBounds.Height)
                    {
                        var e = new InvalidOperationException("You've got too many buttons for this simple implementation.");

                        Logger?.LogError(scope, "{BAB92C08-B381-4F6C-B992-307F2731FE32}", $"Too many buttons [{Stopwatch.GetTimestamp()}]", e);

                        throw e;
                    }
                }

                CreateAButton(logFont, buttonWidth, buttonHeight, x, y, link);

                // move draw positiion to the next column
                x += buttonWidth + gutter;
            }

            CenterButttonArray(viewportCenter, actualButtonArrayWidth);

            Logger?.LogTrace(scope, "{AEF14D98-A473-425A-B37B-FF4FB245257F}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CreateAButton(SpriteFont logFont, int buttonWidth, int buttonHeight, int x, int y, KeyValuePair<Type, Button> link)
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{8DE69395-76CE-4EBB-8A73-4471B6224DBD}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var labelText = link.Key.GetCustomAttribute<SceneAttribute>().DisplayName;
            _sceneLinks[link.Key] = this.AddButton(
                new Vector2(x, y),
                new Vector2(buttonWidth, buttonHeight))
                .SetColor(
                Microsoft.Xna.Framework.Color.DarkSlateGray,
                Microsoft.Xna.Framework.Color.Gray,
                Microsoft.Xna.Framework.Color.LightCoral)
                .SetLabel(logFont, () => labelText,
                Microsoft.Xna.Framework.Color.LightGray,
                Microsoft.Xna.Framework.Color.White,
                Microsoft.Xna.Framework.Color.Aqua);

            Logger?.LogTrace(scope, "{94734262-60C1-4EE4-803A-D478AE155F47}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        private void CenterButttonArray(Vector2 viewportCenter, int actualButtonArrayWidth)
        {
            using var scope = Logger?.BeginScope($"{nameof(MainMenuScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{FFB252DC-0456-4CDC-9B08-EEAB555279CB}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var actualButtonArrayMiddle = actualButtonArrayWidth * 0.5f;
            foreach (var button in _sceneLinks.Values)
            {
                button.Position += new Vector2(viewportCenter.X - actualButtonArrayMiddle, 0);
            }

            Logger?.LogTrace(scope, "{FFB252DC-0456-4CDC-9B08-EEAB555279CB}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
