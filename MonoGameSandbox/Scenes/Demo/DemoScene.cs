using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameSandbox.Scenes.Demo.DrawableGameComponents;
using System;
using Utilities.Abstractions;
using Utilities.DrawableGameComponents;

namespace MonoGameSandbox.Scenes.Demo
{
    public class DemoScene : Scene
    {
        private readonly ICameraService _camera;
        public DemoScene(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            BackgroundColor = Color.CadetBlue;
            _camera = Game.Services.GetService<ICameraService>();
            _camera.Enabled = true;
            Transformer = _camera;
        }

        protected override void LoadContent()
        {
            // a scene (or any object) can be responsible for instantiating
            // another root node, without having it be a child of the object insantiating it
            // if it makes sense for that node to be rendered independently
            IndependentSprites.Add(new Hud(Game, SpriteBatch));

            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            var logFont = Game.Content.Load<SpriteFont>("LogFont");
            var arraySize = 20;
            var boxSize = 100;
            var gapSize = 10;
            var viewportHorizontalCenter = Game.GraphicsDevice.Viewport.Width * 0.5f;
            var viewportVerticalCenter = Game.GraphicsDevice.Viewport.Height * 0.5f;
            int boxArrayDimension = arraySize * boxSize + (arraySize + 1) * gapSize;
            var boxArrayCenter = boxArrayDimension * 0.5f;
            int position(int index, float center)
            {
                return (int)Math.Round((boxSize + gapSize) * (index - 1) + (center - boxArrayCenter));
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
                        DestinationRectangle = new Rectangle(xPos, yPos, boxSize, boxSize),
                        Color = Color.White,
                        LayerDepth = 0.009999f
                    };
                    var labelText = $"{x},{y}";
                    var label = new StringSprite(Game, this)
                    {
                        SpriteFont = logFont,
                        Text = () => labelText,
                        Position = new Vector2(xPos, yPos),
                        Color = Color.Black,
                        LayerDepth = 0.009998f
                    };
                }
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _camera.Enabled = false;
            base.UnloadContent();
        }
    }
}
