using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameSandbox.Scenes.Simple2dCameraDemo;
using MonoGameSandbox.Scenes.SomeOtherDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.Scenes.MainMenu
{
    [Scene("Main Menu")]
    public class MainMenuScene : Scene
    {
        private readonly Dictionary<Type, Button> _sceneLinks = new Dictionary<Type, Button>();

        public MainMenuScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            BackgroundColor = Microsoft.Xna.Framework.Color.Black;
            InitializeSceneLinks();
        }

        protected override void LoadContent()
        {
            var titleFont = Game.Content.Load<SpriteFont>("MenuTitleFont");
            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            var viewportCenter = Game.GraphicsDevice.Viewport.Bounds.GetCenter();

            CreateTitle(titleFont, viewportCenter);
            CreateButtonArray(logFont, viewportCenter);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
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
            _sceneLinks[typeof(Simple2dCameraDemoScene)] = null;
            _sceneLinks[typeof(Demo01)] = null;
            _sceneLinks[typeof(Demo02)] = null;
            _sceneLinks[typeof(Demo03)] = null;
            _sceneLinks[typeof(Demo04)] = null;
            _sceneLinks[typeof(Demo05)] = null;
            _sceneLinks[typeof(Demo06)] = null;
            _sceneLinks[typeof(Demo07)] = null;
            _sceneLinks[typeof(Demo08)] = null;
        }

        private void CreateTitle(SpriteFont titleFont, Vector2 viewportCenter)
        {
            static string text() => "Select a Demo";
            var titleWidth = titleFont.MeasureString(text()).X;
            this.AddStringSprite(new Vector2(viewportCenter.X - titleWidth * 0.5f, 70), titleFont, text);
        }

        private void CreateButtonArray(SpriteFont logFont, Vector2 viewportCenter)
        {
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
                        throw new InvalidOperationException("You've got too many buttons for this simple implementation.");
                }

                CreateAButton(logFont, buttonWidth, buttonHeight, x, y, link);

                // move draw positiion to the next column
                x += buttonWidth + gutter;
            }

            CenterButttonArray(viewportCenter, actualButtonArrayWidth);
        }

        private void CreateAButton(SpriteFont logFont, int buttonWidth, int buttonHeight, int x, int y, KeyValuePair<Type, Button> link)
        {
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
        }

        private void CenterButttonArray(Vector2 viewportCenter, int actualButtonArrayWidth)
        {
            var actualButtonArrayMiddle = actualButtonArrayWidth * 0.5f;
            foreach (var button in _sceneLinks.Values)
            {
                button.Position += new Vector2(viewportCenter.X - actualButtonArrayMiddle, 0);
            }
        }
    }
}
