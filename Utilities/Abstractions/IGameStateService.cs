using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilities.DrawableGameComponents;

namespace Utilities.Abstractions
{
    public interface IGameStateService
    {
        Type GameState { get; }
        void SetGameState<T>(Game game, SpriteBatch spriteBatch, ITransformer transformer, IPauseService pause) where T : Scene;
    }
}
