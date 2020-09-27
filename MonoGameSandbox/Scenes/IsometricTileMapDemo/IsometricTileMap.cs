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
        /// A list of paths to texture source files used by this isometric tile map
        /// </summary>
        private readonly List<string> _texturePaths = new List<string>();
        /// <summary>
        /// A dictionary of loaded Texture2D objects index by source filepath
        /// </summary>
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        /// <summary>
        /// Used to map a textures from the _textures dictionary to positions in the _spriteArray
        /// </summary>
        private readonly Dictionary<Vector3, string> _textureMap = new Dictionary<Vector3, string>();
        /// <summary>
        /// Indexed using a 3 dimensional cordinate system to describe the logical position of each tile
        /// </summary>
        private readonly Dictionary<Vector3, Sprite> _tileArray = new Dictionary<Vector3, Sprite>();

        /// <summary>
        /// The width of one isometric tile
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// The height of one isometric tile
        /// </summary>
        public int TileHeight { get; set; }
        /// <summary>
        /// The max number of tiles in the X dimension of the tile array.
        /// The X axis traverses from the top of the tile array to
        /// the bottom right.
        /// </summary>
        public int TileArrayWidth { get; set; }
        /// <summary>
        /// The max number of tiles in the Y dimension of the tile array.
        /// The Y axis traverses from the top of the tile array to
        /// the bottom left.
        /// </summary>
        public int TileArrayHeight { get; set; }
        /// <summary>
        /// The max number of vertical layers the tile array can have.
        /// The z axis traverses from the bottom of the tile array to
        /// the top
        /// </summary>
        public int TileArrayLayers { get; set; }
        /// <summary>
        /// The proper X spacing ratio for tiles could be dependent on
        /// the specific dimensions of the source tile set and might
        /// need to be adjusted until tile tesselation is visually
        /// correct.  The spacing ratios, along with correct layer depth,
        /// are what allow tiles overlap and nest properly to produce
        /// the illusion of 3 dimensions.
        /// 
        /// CAUTION! First try to correct this by setting TileWidth and
        /// TileHeight properly.
        /// </summary>
        public float TileXSpacingRatio { get; set; } = 0.5f;
        /// <summary>
        /// The proper Y spacing ratio for tiles could be dependent on
        /// the specific dimensions of the source tile set and might
        /// need to be adjusted until tile tesselation is visually
        /// correct.  The spacing ratios, along with correct layer depth,
        /// are what allow tiles overlap and nest properly to produce
        /// the illusion of 3 dimensions.
        /// 
        /// CAUTION! First try to correct this by setting TileWidth and
        /// TileHeight properly.
        /// </summary>
        public float TileYSpacingRatio { get; set; } = 0.25f;
        /// <summary>
        /// A small offset to disambiguate when tiles on a different z layer are in front of 
        /// other tiles on z layers below
        /// </summary>
        public int ZLayerDisambiguationOffset { get; set; } = 1;
        /// <summary>
        /// Should be set to false by any operation that changes the state of the
        /// tile array.
        /// Will be set to true again at the begining of the Update function when
        /// UpdateArray() is called.
        /// </summary>
        protected bool ArrayInvalidated { get; set; }

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

                ArrayInvalidated = true;

                p.Report((100, "Complete!"));

            });

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (ArrayInvalidated) UpdateArray();

            // recalculate draw parameters
            foreach (var kvp in _tileArray)
            {
                kvp.Value.Position = CalculateDrawPosition(kvp.Key);
                kvp.Value.LayerDepth = CalculateDrawLayerDepth(kvp.Key);
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
        /// Maps the named texture to the provided logical tile map positions
        /// </summary>
        /// <param name="texturePath"></param>
        /// <param name="tileMapPositions"></param>
        public void MapTextures(string texturePath, params Vector3[] tileMapPositions)
        {
            foreach (var position in tileMapPositions)
            {
                _textureMap[position] = texturePath;
            }

            ArrayInvalidated = true;
        }

        /// <summary>
        /// Repopulates the tile array whenever the mapping of textures to the logical map has changed.
        /// </summary>
        private void UpdateArray()
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
                            _tileArray[key] = this.AddSprite(Vector2.Zero, texture);
                    }
                }
            }

            ArrayInvalidated = false;
        }

        /// <summary>
        /// Calculates the 2 dimensional draw position from the logical 3 dimensional position in the tile array
        /// </summary>
        /// <param name="logicalPosition">Reperesents the logical position of the tile in the 3 dimensional tile array</param>
        /// <returns></returns>
        private Vector2 CalculateDrawPosition(Vector3 logicalPosition)
        {
            // use these values to traverse backwards from these limits instead of forward from top-left origin
            var tileArrayWidth = TileWidth * TileArrayWidth * TileXSpacingRatio;
            var tileLayerHeight = TileHeight * (TileArrayLayers - 1) * TileYSpacingRatio;

            var x2x3OffsetBase = TileWidth * logicalPosition.X * TileXSpacingRatio; // spacing in x draw dimension due to x position
            var x2y3OffsetBase = TileWidth * logicalPosition.Y * TileXSpacingRatio; // spacing in x draw dimension due to y position
            var x2AdditionalXOffsetAlternatingByX = logicalPosition.X % 2 * TileXSpacingRatio; // alternating value for proper hex nesting

            var x2x3Offset = x2x3OffsetBase + x2AdditionalXOffsetAlternatingByX; // x draw position increases from left to right by logical x position
            var x2y3Offset = tileArrayWidth - x2y3OffsetBase + x2AdditionalXOffsetAlternatingByX; // x draw position decreases from right to left by logical y position
            var x2z3Offset = 0; // z index does not affect x offset

            var x2 = x2x3Offset + x2y3Offset + x2z3Offset; // merge the x offsets

            var y2x3OffsetBase = TileHeight * logicalPosition.X * TileYSpacingRatio; // spacing in the y draw dimension due to x position
            var y2y3OffsetBase = TileHeight * logicalPosition.Y * TileYSpacingRatio; // spacing in the y draw dimension due to y position
            var y2z3OffsetBase = TileHeight * logicalPosition.Z * 2 * TileYSpacingRatio; // spacing in the z draw dimension due to z position
            var y2AdditionalYOffsetAlternatingByX = logicalPosition.X % 2 * TileYSpacingRatio; // alternating value for proper hex nesting
            var y2AdditionalYOffsetAlternatingByY = logicalPosition.Y % 2 * TileYSpacingRatio; // alternating value for proper hex nesting
            var zLayerDisambiguationOffset = logicalPosition.Z * ZLayerDisambiguationOffset; // move tiles down the Y draw axis by the z layer disambiguation offset

            var y2x3Offset = y2x3OffsetBase + y2AdditionalYOffsetAlternatingByX; // y draw position increases from top to bottom by logical x position
            var y2y3Offset = y2y3OffsetBase + y2AdditionalYOffsetAlternatingByY; // y draw position increases from top to bottom by logical y position
            var y2z3Offset = tileLayerHeight - y2z3OffsetBase + zLayerDisambiguationOffset; // y draw position decreases from bottom to top by logical z position

            var y2 = y2x3Offset + y2y3Offset + y2z3Offset; // mege the y offsets

            return new Vector2(x2, y2);
        }

        /// <summary>
        /// Calculates the layer depth to draw a given sprite at based on its position in the logical 3 dimensional array
        /// </summary>
        /// <param name="logicalPosition">Reperesents the logical position of the tile in the 3 dimensional tile array</param>
        /// <returns></returns>
        private static float CalculateDrawLayerDepth(Vector3 logicalPosition)
        {
            // logical x position has the lowest depth weight
            var xLayerDepth = logicalPosition.X * 0.0000001f;
            // logical y position has the next highest depth weight
            var yLayerDepth = logicalPosition.Y * 0.00001f;
            // logical z position has the highest depth weight
            var zLayerDepth = logicalPosition.Z * 0.001f;

            // depth increases as you draw further behind other sprites, so we must subtract
            // the weighted depths from a starting highest depth
            var layerDepth = 0.01f - xLayerDepth - yLayerDepth - zLayerDepth;

            return layerDepth;
        }
    }
}
