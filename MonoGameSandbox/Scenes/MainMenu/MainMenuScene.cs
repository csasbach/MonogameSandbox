using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.Demo;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace MonoGameSandbox.Scenes.MainMenu
{
    public class MainMenuScene : Scene
    {
        private readonly IInputService _input;
        public MainMenuScene(Game game, SpriteBatch spriteBatch, ITransformer transformer, IPauseService pause, IGameStateService gameState)
            : base(game, spriteBatch, transformer, pause, gameState)
        {
            BackgroundColor = Color.Black;
            _input = Game.Services.GetService<IInputService>();
        }

        public override void Update(GameTime gameTime)
        {
            _input.OnReleased(() => GameState.SetGameState<DemoScene>(SpriteBatch, Transformer, Pause), g => g.A, m => m.LeftButton, Keys.E);

            base.Update(gameTime);
        }
    }
}
