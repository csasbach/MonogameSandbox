using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.GameComponents;
using MonoGameSandbox.Scenes.MainMenu;
using System;
using System.Collections.Concurrent;
using System.Linq;
using Utilities.Abstractions;
using Utilities.Services;

namespace MonoGameSandbox
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly ConcurrentBag<Exception> CaughtExceptions = new ConcurrentBag<Exception>();
        private readonly IInputService _input;
        private readonly IPauseService _pause;
        private readonly IGameStateService _gameState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // services and controllers for non-graphical game components can be instantiated here
            _input = new InputService(this);
            _pause = new PauseService(this, _input);
            _gameState = new GameStateService(this);

            new Camera2dController(this, _graphics, _input);
        }

        protected override void Initialize()
        {
            // graphics must be initialized starting here because GraphicsDevice is not yet available in constructor
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // game components must be instantiated here after graphics but before base.Initialize()
            // the first scene is loaded here, then all subsequent scene loading and unloading is handled by
            // the scenes themselves
            _gameState.SetGameState<MainMenuScene>(_spriteBatch);

            // game components will have their Initialize() methods called here if you instantiated them above
            base.Initialize();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override void Update(GameTime gameTime)
        {
            // each frame, exceptions are handled during Update and Draw, the game loop is paused,
            // and then the exceptions are rethrown here
            // this is much more convenient for debugging
            if (_pause.Paused && CaughtExceptions.Any())
            {
                if (_graphics.IsFullScreen) _graphics.ToggleFullScreen(); // saves us from having to kill task
                throw new AggregateException("Exceptions were thrown during the last frame!",
                    CaughtExceptions.ToList().Select(x => CaughtExceptions.TryTake(out var taken) ? taken : null));
            }

            try
            {
                // checking exit immediately helps to prevent getting stuck in the game loop
                _input.OnReleased(Exit, b => b.Back, Keys.Escape);

                // only doing the pause update saves resources while paused
                if (_pause.Paused)
                {
                    _pause.PausedUpdate(gameTime);
                    return;
                }

                // game components will have their Update(gameTime) methods called here
                base.Update(gameTime);
            }
            catch (Exception e)
            {
                CaughtExceptions.Add(e);
                _pause.Paused = true;
            }
            finally
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override void Draw(GameTime gameTime)
        {
            try
            {
                // game components will have their Update(gameTime) methods called here
                base.Draw(gameTime);
            }
            catch (Exception e)
            {
                CaughtExceptions.Add(e);
                _pause.Paused = true;
            }
            finally
            {
            }
        }
    }
}
