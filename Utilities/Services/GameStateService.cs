using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace Utilities.Services
{
    public class GameStateService : ServiceBase, IGameStateService
    {
        private Scene _gameState;
        public Type GameState
        {
            get
            {
                return _gameState.GetType();
            }
        }

        public GameStateService(Game game) : base(game, typeof(IGameStateService))
        {
        }

        public void SetGameState<T>(SpriteBatch spriteBatch, ITransformer transformer, IPauseService pause) where T : Scene
        {
            _gameState?.ForceUnloadContent();
            _gameState = (T)Activator.CreateInstance(typeof(T), Game, spriteBatch, transformer, pause, this);
        }
    }
}
