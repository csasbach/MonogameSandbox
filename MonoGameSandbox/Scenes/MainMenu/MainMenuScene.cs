using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.Demo;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.Scenes.MainMenu
{
    public class MainMenuScene : Scene
    {
        private readonly IInputService _input;
        private Button _button;

        public MainMenuScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            SceneName = "Menu";
            BackgroundColor = Microsoft.Xna.Framework.Color.Black;
            _input = Game.Services.GetService<IInputService>();
        }

        protected override void LoadContent()
        {
            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            _button = this.AddButton(new Vector2(0, 0), new Vector2(200, 50))
                .SetColor(
                Microsoft.Xna.Framework.Color.DarkSlateGray,
                Microsoft.Xna.Framework.Color.Gray,
                Microsoft.Xna.Framework.Color.LightCoral)
                .SetLabel(logFont, () => $"START (E)",
                Microsoft.Xna.Framework.Color.LightGray,
                Microsoft.Xna.Framework.Color.White,
                Microsoft.Xna.Framework.Color.Aqua);

            _button.Position = _button.LocalBounds.CenterInside(Game.GraphicsDevice.Viewport.Bounds);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _input.OnReleased(() => GameState.SetGameState<DemoScene>(SpriteBatch), g => g.A, Keys.E);

            _button.OnClicked(() => GameState.SetGameState<DemoScene>(SpriteBatch));

            base.Update(gameTime);
        }
    }
}
