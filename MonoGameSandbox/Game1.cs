using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Utilities.Abstractions;
using Utilities.GameComponents;
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
        private readonly LoggerService _logger;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // services and controllers for non-graphical game components can be instantiated here
            _logger = new LoggerService(this);
            _logger.AddLogQueue(new DebugLogQueue());
            _logger.SetLogLevel(LogLevel.Trace);
            using var scope = _logger.BeginScope($"{nameof(Game1)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            _input = new InputService(this);
            _pause = new PauseService(this, _input);
            _gameState = new GameStateService(this);
            new Camera2dController(this, _graphics, _input);

            _logger.LogInfo(scope, "8DE9D77C-531A-4101-B2A3-32F3C51A4863", $"Finished[{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void Initialize()
        {
            using var scope = _logger?.BeginScope($"{nameof(Game1)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _logger?.LogTrace(scope, "{60B0C932-3DE3-47C1-AAEF-4497EF00D752}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            Window.Title = "Sandbox";
            // graphics must be initialized starting here because GraphicsDevice is not yet available in constructor
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // game components must be instantiated here after graphics but before base.Initialize()
            // the first scene is loaded here, then all subsequent scene loading and unloading is handled by
            // the scenes themselves
            _gameState.SetGameState<MainMenuScene>(_spriteBatch);

            _logger?.LogTrace(scope, "{E977152D-0077-45B2-A52C-B086F00387AB}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            // game components will have their Initialize() methods called here if you instantiated them above
            base.Initialize();

            _logger?.LogTrace(scope, "{152B407E-C092-4295-BC46-E8117C401D0F}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
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
                _input.OnReleased(Exit, g => g.Back, Keys.Escape);

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
