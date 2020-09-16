using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        protected IGameStateService GameState { get; }
        protected Color BackgroundColor { get; set; } = Color.Black;
        protected IPauseService Pause { get; }

        public Scene(Game game, SpriteBatch spriteBatch, ITransformer transformer, IPauseService pause, IGameStateService gameState) : base(game, spriteBatch, transformer)
        {
            Pause = pause;
            GameState = gameState;
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

        public void ForceUnloadContent()
        {
            base.UnloadContent();
        }
    }
}
