using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.Demo;
using System;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace MonoGameSandbox.Scenes.MainMenu
{
    public class MainMenuScene : Scene
    {
        private readonly IInputService _input;
        private Sprite _button;

        public MainMenuScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            SceneName = "Menu";
            BackgroundColor = Color.Black;
            _input = Game.Services.GetService<IInputService>();
        }

        protected override void LoadContent()
        {
            var width = 200;
            var height = 50;
            var texture = new Texture2D(Game.GraphicsDevice, 1, 1);
            var logFont = Game.Content.Load<SpriteFont>("LogFont");
            texture.SetData(new[] { Color.White });
            _button = new Sprite(Game, this)
            {
                DestinationRectangle = new Rectangle(
                    (int)Math.Round(Game.GraphicsDevice.Viewport.Bounds.Width * 0.5f - width * 0.5f),
                    (int)Math.Round(Game.GraphicsDevice.Viewport.Bounds.Height * 0.5f - height * 0.5f), width, height),
                Texture = texture,
                Color = Color.DarkSlateGray,
                MouseOverColor = Color.Gray
            };
            Children.Add(_button);
            var label = new StringSprite(Game, _button)
            {
                SpriteFont = logFont,
                Text = () => $"START (E)",
                Color = Color.LightGray,
                Position = new Vector2(46, 16)
            };
            _button.Children.Add(label);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _input.OnReleased(() => GameState.SetGameState<DemoScene>(SpriteBatch), g => g.A, Keys.E);

            if (_button.IsMouseOver)
            {
                _input.OnReleased(() => GameState.SetGameState<DemoScene>(SpriteBatch), null, m => m.LeftButton);
            }

            base.Update(gameTime);
        }
    }
}
