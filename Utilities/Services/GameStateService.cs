using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace Utilities.Services
{
    public class GameStateService : ComponentService, IGameStateService
    {
        private Func<Scene> _gameStateSetter;
        private Scene _gameState;
        public Type GameState => _gameState.GetType();

        public GameStateService(Game game) : base(game, typeof(IGameStateService))
        {
            using (var scope = Logger?.BeginScope($"{nameof(GameStateService)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
                Logger?.LogTrace(scope, "{9D4CE9CF-193F-4AFD-A83D-64BE1FDE5AA5}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        public void SetGameState<T>(SpriteBatch spriteBatch) where T : Scene
        {
            SetGameState(typeof(T), spriteBatch);
        }

        public void SetGameState(Type sceneType, SpriteBatch spriteBatch)
        {
            using (var scope = Logger?.BeginScope($"{nameof(GameStateService)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{FDCC0AAF-B3BB-4359-9711-56F1F461190E}", $"Started [{Stopwatch.GetTimestamp()}] with {nameof(sceneType)}:{sceneType.Name}", null);

                // define the function that will set the new game state when Update is next called
                _gameStateSetter = () => Activator.CreateInstance(sceneType, Game, spriteBatch) as Scene;

                Logger?.LogTrace(scope, "{B1B94284-5F01-4413-99CC-9D70E5B8EB4C}", $"Finished [{Stopwatch.GetTimestamp()}] with {nameof(sceneType)}:{sceneType.Name}", null);
            }
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
