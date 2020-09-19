using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameSandbox.Scenes.MainMenu;
using MonoGameSandbox.SharedComponents;
using Utilities.Abstractions;
using Utilities.Attributes;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.Scenes.Simple2dCameraDemo
{
    [Scene("Simple 2D Camera Demo")]
    public class Simple2dCameraDemoScene : Scene
    {
        private readonly ICameraService _camera;
        private readonly IInputService _input;

        public Simple2dCameraDemoScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            // using service location pattern to access services in constructors
            // of game objects
            _camera = Game.Services.GetService<ICameraService>();
            _input = Game.Services.GetService<IInputService>();
            Transformer = _camera;

            BackgroundColor = Microsoft.Xna.Framework.Color.CadetBlue;
        }

        protected override void LoadContent()
        {
            var logFont = Game.Content.Load<SpriteFont>("LogFont");

            // a scene can be responsible for instantiating
            // another root node, without having it be a child of the object insantiating it
            // if it makes sense for that node to be rendered independently, such as components
            // not rendered in the main camera view
            var hud = new Hud(Game, SpriteBatch);
            // we can add more content to the Hud if we want
            static string text() => "Camera: '(,)' Rotate | '<,>,ScroolWheel' Zoom | 'W,A,S,D,Arrow Keys' Move | 'R' Reset";
            new StringSprite(Game, hud)
            {
                SpriteFont = logFont,
                Text = text,
                Position = new Vector2(Game.GraphicsDevice.Viewport.Width - (logFont.MeasureString(text()).X + 10), Game.GraphicsDevice.Viewport.Height - 30)
            };
            // but then that node should be registered under IndependentSprites in the scene
            // so that its drawable game component methods will be called
            IndependentSprites.Add(hud);

            // camera is disabled by default to prevent it running during times when it should not be
            _camera.Enabled = true;

            // just something to look at with the camera
            CreateBoxArray(logFont);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _input.OnReleased(() => GameState.SetGameState<MainMenuScene>(SpriteBatch), g => g.A, Keys.E);

            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            _camera.Enabled = false;
            base.UnloadContent();
        }

        private void CreateBoxArray(SpriteFont logFont)
        {
            var arraySize = 20;
            var boxSize = 100;
            var gapSize = 10;
            var texture = GraphicsDevice.CreateRectangleTexture(boxSize, boxSize);
            var viewportHorizontalCenter = Game.GraphicsDevice.Viewport.Width * 0.5f;
            var viewportVerticalCenter = Game.GraphicsDevice.Viewport.Height * 0.5f;
            int boxArrayDimension = arraySize * boxSize + (arraySize + 1) * gapSize;
            var boxArrayCenter = boxArrayDimension * 0.5f;
            int position(int index, float center)
            {
                return ((boxSize + gapSize) * (index - 1) + (center - boxArrayCenter)).ToInt();
            }
            for (var x = arraySize; x > 0; x--)
            {
                for (var y = arraySize; y > 0; y--)
                {
                    var xPos = position(x, viewportHorizontalCenter);
                    var yPos = position(y, viewportVerticalCenter);
                    var box = new Sprite(Game, this)
                    {
                        Texture = texture,
                        Position = new Vector2(xPos, yPos),
                        //Color = Microsoft.Xna.Framework.Color.White,
                        LayerDepth = 0.009999f
                    };
                    var labelText = $"{x},{y}";
                    var label = new StringSprite(Game, this)
                    {
                        SpriteFont = logFont,
                        Text = () => labelText,
                        Position = new Vector2(xPos, yPos),
                        Color = Microsoft.Xna.Framework.Color.Black,
                        LayerDepth = 0.009998f
                    };
                }
            }
        }
    }
}
