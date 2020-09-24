using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.Serializers;
using MonoGameSandbox.SharedComponents;
using System;
using System.Diagnostics;
using System.IO;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Services;
using Utilities.Extensions;

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
        private readonly ISaveService<SaveData, string> _textSaveService;
        private readonly ISaveService<SaveData, byte[]> _binarySaveService;

        public SimpleSaveDemoScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{28ABC73C-5A83-4D0F-9B3F-BF4D3EEDEF2B}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            _input = Game.Services.GetService<IInputService>();
            _textSaveService = new SaveService<SaveData, string>(game, new JsonTextSerializer<SaveData>())
            {
                SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };
            _binarySaveService = new SaveService<SaveData, byte[]>(game, new JsonBinarySerializer<SaveData>())
            {
                SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };

            Logger?.LogTrace(scope, "{13283024-88FA-4F1A-B13D-ACE03F0A8C0C}", $"Finished [{Stopwatch.GetTimestamp()}]", null);
        }

        protected override void LoadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{5B3E6C83-3E94-407C-A7D1-75692FB8DC63}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            LoadContentAsync(p =>
            {
                p.Report((0, "Creating heads up display..."));
                var hud = new Hud(Game, SpriteBatch);

                p.Report((33, "Adding content to heads up display..."));
                static string text() => "Save: 'B' Save binary file | 'J' Save json file | L' Load last saved file";
                var position = new Vector2(Game.GraphicsDevice.Viewport.Width - (logFont.MeasureString(text()).X + 10), Game.GraphicsDevice.Viewport.Height - 30);
                hud.AddStringSprite(position, logFont, text);

                p.Report((66, "Registering heads up display..."));
                IndependentSprites.Add(hud);

                p.Report((100, "Done!"));
            });

            Logger?.LogTrace(scope, "{61172719-1F28-4DB8-928B-B84FDD00E257}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.LoadContent();

            Logger?.LogTrace(scope, "{C2E5D61C-8CA3-482A-8ADF-22AB728E304B}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }

        private bool _savedSomething = false;
        private bool _loadedSomething = false;
        private string _lastSaved;
        private Type _lastSavedType;
        private string _lastLoaded;
        public override void Update(GameTime gameTime)
        {
            if (!LoadContentCompleted) return;

            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            _input.OnReleased(() =>
            {
                var timestamp = Stopwatch.GetTimestamp();
                _savedSomething = _textSaveService.TrySaveData(Path.Combine("saveData", $"{timestamp}.save"),
                    new SaveData { SomeImportantState = $"I saved a thing at {timestamp}!" });
                if (_savedSomething)
                {
                    _lastSaved = $"{timestamp}.save";
                    _lastSavedType = typeof(string);
                }
            }, g => g.A, Keys.J);

            _input.OnReleased(() =>
            {
                var timestamp = Stopwatch.GetTimestamp();
                _savedSomething = _binarySaveService.TrySaveData(Path.Combine("saveData", $"{timestamp}.save"),
                    new SaveData { SomeImportantState = $"I saved a thing at {timestamp}!" });
                if (_savedSomething)
                {
                    _lastSaved = $"{timestamp}.save";
                    _lastSavedType = typeof(byte[]);
                }
            }, g => g.A, Keys.B);

            _input.OnReleased(() =>
            {
                if (string.IsNullOrEmpty(_lastSaved)) return;
                SaveData loadedData = null;
                if (_lastSavedType == typeof(string))
                    _loadedSomething = _textSaveService.TryLoadData(Path.Combine("saveData", _lastSaved), out loadedData);
                if (_lastSavedType == typeof(byte[]))
                    _loadedSomething = _binarySaveService.TryLoadData(Path.Combine("saveData", _lastSaved), out loadedData);
                if (_loadedSomething) _lastLoaded = loadedData?.SomeImportantState;
            }, g => g.A, Keys.L);

            base.Update(gameTime);
        }

        protected override void DrawMyContent(SpriteBatch spriteBatch)
        {
            if (_savedSomething)
            {
                spriteBatch.DrawString(Game.Content.Load<SpriteFont>("LogFont"), $"Last file saved was a {(_lastSavedType.Name == typeof(string).Name ? "json" : "binary")} file: {_lastSaved}", new Vector2(100, 100), Microsoft.Xna.Framework.Color.White);
            }

            if (_loadedSomething)
            {
                spriteBatch.DrawString(Game.Content.Load<SpriteFont>("LogFont"), $"The content loaded from the last file saved was: {_lastLoaded}", new Vector2(100, 200), Microsoft.Xna.Framework.Color.White);
            }

            base.DrawMyContent(spriteBatch);
        }

        protected override void UnloadContent()
        {
            using var scope = Logger?.BeginScope($"{nameof(SimpleSaveDemoScene)} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Logger?.LogTrace(scope, "{21A434BF-8F14-481B-B0B6-C960D95A03FD}", $"Started [{Stopwatch.GetTimestamp()}]", null);

            var cleanUpDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "saveData");
            if (Directory.Exists(cleanUpDir)) Directory.Delete(cleanUpDir, true);

            Logger?.LogTrace(scope, "{2D424FB9-0426-4173-AD3F-3133F1724A86}", $"Finished Override [{Stopwatch.GetTimestamp()}]", null);

            base.UnloadContent();

            Logger?.LogTrace(scope, "{20EDC4B5-CC99-41CA-B5BC-C8824CD0B702}", $"Finished Base [{Stopwatch.GetTimestamp()}]", null);
        }
    }
}
