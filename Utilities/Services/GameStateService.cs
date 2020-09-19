using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace Utilities.Services
{
    public class GameStateService : ComponentService, IGameStateService
    {
        private Func<Scene> _gameStateSetter;
        private Scene _gameState;
        public Type GameState => _gameState.GetType();

        public GameStateService(Game game) : base(game, typeof(IGameStateService)) { }

        public void SetGameState<T>(SpriteBatch spriteBatch) where T : Scene
        {
            SetGameState(typeof(T), spriteBatch);
        }

        public void SetGameState(Type sceneType, SpriteBatch spriteBatch)
        {
            // define the function that will set the new game state when Update is next called
            _gameStateSetter = () => Activator.CreateInstance(sceneType, Game, spriteBatch) as Scene;
        }

        public override void Update(GameTime gameTime)
        {
            if (_gameStateSetter is null) return;

            _gameState?.ForceUnloadContent();
            _gameState = _gameStateSetter();
            _gameStateSetter = null;
            _gameState.Initialize();

            base.Update(gameTime);
        }
    }
}
