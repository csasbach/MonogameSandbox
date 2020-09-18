using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Utilities.Abstractions;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Meant to be used as the primary 2D canvas for the entire scene
    /// Should be instantiated by the game or a game state manager
    /// The active scene takes control of the game state
    /// </summary>
    public class Scene : Canvas2d
    {
        private string _gameTitle;
        private string _sceneName;
        protected string SceneName
        {
            get => string.IsNullOrWhiteSpace(_sceneName) ? _gameTitle : $"{_gameTitle} ({_sceneName})";
            set => _sceneName = value;
        }
        protected IGameStateService GameState { get; }
        protected Color BackgroundColor { get; set; } = Color.Black;
        protected IPauseService Pause { get; }
        protected List<ISprite> IndependentSprites { get; } = new List<ISprite>();

        public Scene(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            Pause = Game.Services.GetService<IPauseService>();
            GameState = Game.Services.GetService<IGameStateService>();
        }

        public override void Initialize()
        {
            _gameTitle = _gameTitle ?? Game.Window.Title;
            Game.Window.Title = SceneName;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var sprite in IndependentSprites)
            {
                if (!sprite.IsInitialized) sprite.Initialize();
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // not drawing when paused saves resources
            if (Pause.Paused) return;

            // as the root node sprite container for the entire scene,
            // this node has the responsibility of clearing the render for each frame.
            GraphicsDevice.Clear(BackgroundColor);

            base.Draw(spriteBatch);
        }

        protected override void UnloadContent()
        {
            Game.Window.Title = _gameTitle;
            foreach (var sprite in IndependentSprites)
            {
                sprite.ForceUnloadContent();
            }
            base.UnloadContent();
        }
    }
}
