using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class PauseService : ComponentService, IPauseService
    {
        private readonly IInputService _input;

        public bool Paused { get; set; }

        public PauseService(Game game, IInputService input) : base(game, typeof(IPauseService))
        {
            _input = input;
        }

        /// <summary>
        /// Can be called to keep updating even when the game is paused
        /// </summary>
        /// <param name="gameTime"></param>
        public void PausedUpdate(GameTime gameTime)
        {
            Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // automatically pause when the window loses focus
            if (!Game.IsActive) Paused = true;

            // we need to keep caching input so that un-pause and exit input can still be recognized
            if (Paused) _input.CacheInput();

            // toggle the paused state
            _input.OnReleased(() => Paused = !Paused, g => g.Start, Keys.Space);

            base.Update(gameTime);
        }
    }
}
