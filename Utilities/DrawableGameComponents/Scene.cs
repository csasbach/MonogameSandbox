using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.Services;

namespace Utilities.DrawableGameComponents
{
    /// <summary>
    /// Meant to be used as the primary 2D canvas for the entire scene
    /// Should be instantiated by the game or a game state manager
    /// The active scene takes control of the game state
    /// </summary>
    public class Scene : Canvas2d
    {
        private string _gameTitle;
        private string _sceneName;
        protected string SceneName
        {
            get => string.IsNullOrWhiteSpace(_sceneName) ? _gameTitle : $"{_gameTitle} ({_sceneName})";
            set => _sceneName = value;
        }
        protected IGameStateService GameState { get; }
        protected Color BackgroundColor { get; set; } = Microsoft.Xna.Framework.Color.Black;
        protected IPauseService Pause { get; }
        protected List<ISprite> IndependentSprites { get; } = new List<ISprite>();

        /// <summary>
        /// Root node constructor
        /// requires the Game and SpriteBatch that will be used by
        /// this sprite and every sprite in its tree
        /// </summary>
        /// <param name="game"></param>
        /// <param name="spriteBatch"></param>
        public Scene(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            using (var scope = Logger?.BeginScope($"{nameof(Scene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{6FF9A266-8330-4E02-882B-3DF1A380B5D5}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                SceneName = GetType().GetCustomAttribute<SceneAttribute>().DisplayName;
                Pause = Game.Services.GetService<IPauseService>();
                GameState = Game.Services.GetService<IGameStateService>();

                Logger?.LogTrace(scope, "{996C333E-0E7D-448A-84CE-A654CB297095}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public override void Initialize()
        {
            using (var scope = Logger?.BeginScope($"{nameof(Scene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{123492DD-B31A-466E-A5C6-BA3D7CA7BBCD}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                _gameTitle = _gameTitle ?? Game.Window.Title;
                Game.Window.Title = SceneName;

                Logger?.LogTrace(scope, "{30A5EE10-0E07-4A18-8621-D999BDB58E97}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

                base.Initialize();

                Logger?.LogTrace(scope, "{F71971FC-B236-4CBD-AC45-9F7600FF7906}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var sprite in IndependentSprites)
            {
                if (!sprite.IsInitialized) sprite.Initialize();
            }

            base.Update(gameTime);
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            // not drawing when paused saves resources
            if (Pause.Paused) return;

            // as the root node sprite container for the entire scene,
            // this node has the responsibility of clearing the render for each frame.
            GraphicsDevice.Clear(BackgroundColor);

            base.Draw(spriteBatch);
        }

        protected override void UnloadContent()
        {
            // this mechanism will prevent follow-up
            // events such as that fired by the GameComponentsCollection
            // from trying to Unload the content more than once
            if (isUnloading) return;
            isUnloading = true;

            using (var scope = Logger?.BeginScope($"{nameof(Scene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}"))
            {
                Logger?.LogTrace(scope, "{FA8ED8F2-EFDC-43C9-B482-C512EEB21CFB}", $"Started [{Stopwatch.GetTimestamp()}]", null);

                Game.Window.Title = _gameTitle;

                foreach (var sprite in IndependentSprites)
                {
                    sprite.ForceUnloadContent();
                }

                Logger?.LogTrace(scope, "{5229B282-872C-43A9-927B-070614904148}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

                base.UnloadContent();

                Logger?.LogTrace(scope, "{29490D81-069E-41C7-B937-0ED622C2B072}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
            }
        }
    }
}
