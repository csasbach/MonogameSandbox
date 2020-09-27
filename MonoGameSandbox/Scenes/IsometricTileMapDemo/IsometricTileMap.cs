using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Utilities.DrawableGameComponents;
using Utilities.Extensions;

namespace MonoGameSandbox.Scenes.TexturePackerDemo
{
    public class IsometricTileMap : SpriteBase
    {
        /// <summary>
        /// A list of paths to texture files used in this isometric tile map
        /// </summary>
        private readonly List<string> _texturePaths = new List<string>();
        /// <summary>
        /// A dictionary of loaded Texture2D objects index by source filepath
        /// </summary>
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        /// <summary>
        /// Used to map a texture from the _textures dictionary to a position in the _spriteArray
        /// </summary>
        private readonly Dictionary<Vector3, string> _textureMap = new Dictionary<Vector3, string>();
        /// <summary>
        /// Indexed using a 3 dimensional cordinate system to describe the logical position of each sprite
        /// </summary>
        private readonly Dictionary<Vector3, Sprite> _spriteArray = new Dictionary<Vector3, Sprite>();

        public int TileSize { get; set; }
        public int TileArrayWidth { get; set; }
        public int TileArrayHeight { get; set; }
        public int TileArrayLayers { get; set; }

        public IsometricTileMap(ISprite parent, params string[] texturePaths) : base(parent)
        {
            foreach (var path in texturePaths)
            {
                _texturePaths.Add(path);
            }
        }

        protected override void LoadContent()
        {
            LoadContentAsync(p =>
            {
                p.Report((0, "Loading..."));

                float totalItemsToLoad = _texturePaths.Count;
                int elementsLoaded = 0;
                foreach (var path in _texturePaths)
                {
                    _textures[path] = Game.Content.Load<Texture2D>(path);
                    elementsLoaded++;
                    float ratio = elementsLoaded / totalItemsToLoad;
                    var ratioRoundedToHundreths = Math.Round(ratio, 2);
                    var percentComplete = (int)(ratioRoundedToHundreths * 100);
                    p.Report((percentComplete, $"Loading {path}"));
                    //Thread.Sleep(3000);
                }

                UpdateSpriteArray();

                p.Report((100, "Complete!"));

            });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var kvp in _spriteArray)
            {
                (var position, var layerDepth) = CalculateRenderPositionAndLayerDepth(kvp.Key);
                kvp.Value.Position = position;
                kvp.Value.LayerDepth = layerDepth;
            }
            base.Update(gameTime);
        }

        protected override void DrawMyContent(SpriteBatch spriteBatch)
        {
            if (LoadContentCompleted) return;

            // draws the loading message while content is still being loaded

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            spriteBatch.DrawString(
                Game.Content.Load<SpriteFont>("LogFont"),
                $"Loading Textures...",
                new Vector2(10, 50),
                Microsoft.Xna.Framework.Color.White);
            spriteBatch.DrawString(
                Game.Content.Load<SpriteFont>("LogFont"),
                $"{LoadContentProgressMessage} {LoadContentProgressPercent}% completed...",
                new Vector2(10, 70),
                Microsoft.Xna.Framework.Color.White);

            base.DrawMyContent(spriteBatch);
        }

        /// <summary>
        /// Maps the named texture to the listed logical tile map positions
        /// </summary>
        /// <param name="texturePath"></param>
        /// <param name="tileMapPositions"></param>
        public void MapTextures(string texturePath, params Vector3[] tileMapPositions)
        {
            foreach (var position in tileMapPositions)
            {
                _textureMap[position] = texturePath;
            }

            UpdateSpriteArray();
        }

        /// <summary>
        /// Repopulates the sprite array whenever the mapping of textures to the logical map has changed.
        /// </summary>
        private void UpdateSpriteArray()
        {
            // unload all the sprites
            Children.ForEach(c => c.ForceUnloadContent());

            // reload the sprites to populate the configured dimensions of the tile array
            // only populate sprites for positions to which a texture has been mapped
            // and the named texture has been loaded
            for (var x = 0; x < TileArrayWidth; x++)
            {
                for (var y = 0; y < TileArrayHeight; y++)
                {
                    for (var z = 0; z < TileArrayLayers; z++)
                    {
                        var key = new Vector3(x, y, z);
                        if (_textureMap.TryGetValue(key, out var texturePath) &&
                            _textures.TryGetValue(texturePath, out var texture))
                            _spriteArray[key] = this.AddSprite(Vector2.Zero, texture);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the correct draw position and layerdepth for the given logical array position
        /// </summary>
        /// <param name="arrayPosition"></param>
        /// <returns></returns>
        private (Vector2, float) CalculateRenderPositionAndLayerDepth(Vector3 arrayPosition)
        {
            // tiles alternate between being half a tile to the right and 1 tile to the right
            var x2x3Offset = (TileSize * arrayPosition.X * 0.5f) + (arrayPosition.X % 2 * 0.5f);
            // starting from the left-most position, each tile alternates between being one half a tile to the right and 1 tile to the right
            var x2y3Offset = (TileSize * TileArrayHeight * 0.5f) - (TileSize * arrayPosition.Y * 0.5f) + (arrayPosition.Y % 2 * 0.5f);
            // z index does not affect x offset
            var x2z3Offset = 0;
            var x2 = x2x3Offset + x2y3Offset + x2z3Offset;

            // I suspect that the 45/157 ratio is an artifact of how I've created the tile images and is not a proper constant here
            // but for now, it makes the tile grid work with the demo tile images
            // in comments below it is reffered to simply as 'r'

            // tiles alernate between being 1r below and 2r below the previous
            var y2x3Offset = (TileSize * arrayPosition.X * (45f / 157f)) + (arrayPosition.X % 2 * (45f / 157f));
            // tiles alternate from being 1r below and 2r below the previous
            var y2y3Offset = (TileSize * arrayPosition.Y * (45f / 157f)) + (arrayPosition.Y % 2 * (45f / 157f));
            // starting from the bottom, tiles are 2r above the previous
            var y2z3Offset = (TileSize * (TileArrayLayers - 1) * (45f / 157f)) - (TileSize * arrayPosition.Z * 2 * (45f / 157f));
            // create a small diff to disambiguate when tiles on a different z layer are infront of other identical tiles
            y2z3Offset += (arrayPosition.Z * 4);
            var y2 = y2x3Offset + y2y3Offset + y2z3Offset;

            var position = new Vector2(x2, y2);

            // x index has the lowest depth weight
            var xLayerDepth = arrayPosition.X * 0.0000001f;
            // y index has the next highest depth weight
            var yLayerDepth = arrayPosition.Y * 0.00001f;
            // z index has the highest depth weight
            var zLayerDepth = arrayPosition.Z * 0.001f;

            // depth increases as you draw further behind other sprites, so we must subtract
            // the weighted depths from a starting highest depth
            var layerDepth = 0.01f - xLayerDepth - yLayerDepth - zLayerDepth;

            return (position, layerDepth);
        }
    }
}
