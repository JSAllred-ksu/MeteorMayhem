using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameArchitectureExample
{
    public class BasicTilemap
    {
        /// <summary>
        /// The map width
        /// </summary>
        public int MapWidth { get; init; }

        /// <summary>
        /// The map height
        /// </summary>
        public int MapHeight { get; init; }

        /// <summary>
        /// The width of a tile in the map
        /// </summary>
        public int TileWidth { get; init; }

        /// <summary>
        /// The height of a tile in the map
        /// </summary>
        public int TileHeight { get; init; }

        /// <summary>
        /// The texture containing the tiles
        /// </summary>
        public Texture2D TilesetTexture { get; init; }

        /// <summary>
        /// An array of source rectangles corresponding to
        /// tile positions in the texture
        /// </summary>
        public Rectangle[] Tiles { get; init; }

        /// <summary>
        /// The map data - an array of indices to the Tile array
        /// </summary>
        public int[] TileIndices { get; init; }

        /// <summary>
        /// Draws the tilemap. Assumes that spriteBatch.Begin() has been called.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">a spritebatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    // Indices start at 1, so shift by 1 for array coordinates
                    int index = TileIndices[y * MapWidth + x] - 1;

                    // Index of -1 (shifted from 0) should not be drawn
                    if (index == -1) continue;

                    // Draw the current tile
                    spriteBatch.Draw(
                        TilesetTexture,
                        new Rectangle(
                            x * TileWidth,
                            y * TileHeight,
                            TileWidth,
                            TileHeight
                            ),
                        Tiles[index],
                        Color.White
                        );
                }
            }

        }
    }
}
