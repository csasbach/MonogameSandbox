using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace Utilities.Services
{
    public class GameStateService : IGameStateService
    {
        private Scene _gameState;
        public Type GameState
        {
            get
            {
                return _gameState.GetType();
            }
        }

        public void SetGameState<T>(Game game, SpriteBatch spriteBatch, ITransformer transformer, IPauseService pause) where T : Scene
        {
            _gameState?.ForceUnloadContent();
            _gameState = (T)Activator.CreateInstance(typeof(T), game, spriteBatch, transformer, pause, this);
        }
    }
}
