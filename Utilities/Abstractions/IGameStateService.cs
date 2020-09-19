using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.DrawableGameComponents;

namespace Utilities.Abstractions
{
    public interface IGameStateService
    {
        Type GameState { get; }
        void SetGameState<T>(SpriteBatch spriteBatch) where T : Scene;
        void SetGameState(Type sceneType, SpriteBatch spriteBatch);
    }
}
