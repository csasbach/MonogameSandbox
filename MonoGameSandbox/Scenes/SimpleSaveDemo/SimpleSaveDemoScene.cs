using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.Serializers;
using MonoGameSandbox.SharedComponents;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Services;

namespace MonoGameSandbox.Scenes.SimpleSaveDemo
{
    [Scene("Simple Save Demo")]
    public class SimpleSaveDemoScene : Scene
    {
        private class SaveData
        {
            public string SomeImportantState { get; set; }
        }

        private readonly IInputService _input;
        private readonly ISaveService<SaveData> _saveService;

        public SimpleSaveDemoScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{82DBEF8D-2232-4999-B2E4-16A05F4D0439}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _input = Game.Services.GetService<IInputService>();
            _saveService = new SaveService<SaveData, string>(game, new JsonTextSerializer<SaveData>()) 
            {
                SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };

            Logger?.LogTrace(scope, "{C1952E44-54E8-43DF-AFF7-4913774D6609}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{9B4A72CF-AB97-46D3-9825-5753EFE5E50B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            LoadContentAsync(p =>
            {
                p.Report((0, "Creating heads up display..."));
                var hud = new Hud(Game, SpriteBatch);

                p.Report((50, "Registering heads up display..."));
                IndependentSprites.Add(hud);

                p.Report((100, "Done!"));
            });

            Logger?.LogTrace(scope, "{1893C918-D4F2-4BF7-A10B-E560C5AFBEA7}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{959C9D69-85E1-463D-B107-37A859851087}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        private bool _savedSomething = false;
        private bool _loadedSomething = false;
        private string _lastSaved;
        private string _lastLoaded;
        public override void Update(GameTime gameTime)
        {
            if (!LoadContentCompleted) return;

            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            _input.OnReleased(() => {
                var timestamp = Stopwatch.GetTimestamp();
                _savedSomething = _saveService.TrySaveData(Path.Combine("saveData", $"{timestamp}.save"),
                    new SaveData { SomeImportantState = $"I saved a thing at {timestamp}!" });
                if (_savedSomething) _lastSaved = $"{timestamp}.save";
            }, g => g.A, Keys.S);

            _input.OnReleased(() => {
                if (string.IsNullOrEmpty(_lastSaved)) return;
                _loadedSomething = _saveService.TryLoadData(Path.Combine("saveData", _lastSaved), out var loadedData);
                if (_loadedSomething) _lastLoaded = loadedData.SomeImportantState;
            }, g => g.A, Keys.L);

            base.Update(gameTime);
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            if(_savedSomething)
            {
                spriteBatch.DrawString(Game.Content.Load<SpriteFont>("LogFont"), _lastSaved, new Vector2(100,100), Microsoft.Xna.Framework.Color.White);
            }

            if (_loadedSomething)
            {
                spriteBatch.DrawString(Game.Content.Load<SpriteFont>("LogFont"), _lastLoaded, new Vector2(100, 200), Microsoft.Xna.Framework.Color.White);
            }

            base.Draw(spriteBatch);
        }

        protected override void UnloadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{9B4A72CF-AB97-46D3-9825-5753EFE5E50B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var cleanUpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "saveData");
            Directory.Delete(cleanUpDir, true);

            Logger?.LogTrace(scope, "{1893C918-D4F2-4BF7-A10B-E560C5AFBEA7}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.UnloadContent();

            Logger?.LogTrace(scope, "{959C9D69-85E1-463D-B107-37A859851087}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
