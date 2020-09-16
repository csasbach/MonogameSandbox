using Microsoft.Xna.Framework;

namespace Utilities.Abstractions
{
    /// <summary>
    /// Provides hooks for the game and game components to manage the paused state
    /// </summary>
    public interface IPauseService
    {
        bool Paused { get; set; }
        void PausedUpdate(GameTime gameTime);
    }
}
