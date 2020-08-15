using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AperiodicTiling
{
    public class AperiodicTiling : MonoBehaviour
    {
        private const int Red = 0;
        private const int Green = 1;
        private const int Yellow = 2;
        private const int Blue = 3;

        private const int RowCount = 2;
        private const int ColumnCount = 4;

        public Texture2D[] TileTextures = new Texture2D[RowCount * ColumnCount];
        public Tile[] Tileset = new Tile[RowCount * ColumnCount];
        public Texture2D TilesetTexture;

        public int PatternSize = 256;
        public Texture2D PatternTexture;

        /// <summary>
        /// Generates a tileset consisting of 8 Wang tiles.
        /// </summary>
        /// <returns>Returns a tileset as a list of tiles.</returns>
        public Tile[] generateTileset()
        {
            Tile[] tileset = new Tile[RowCount * ColumnCount];

            // Tile # = NESW
            // Tile 0 = 0213
            // Tile 0 = 1313
            // Tile 0 = 0202
            // Tile 0 = 1302
            // Tile 0 = 0312
            // Tile 0 = 1212
            // Tile 0 = 0303
            // Tile 0 = 1203

            tileset[0] = new Tile(0, TileTextures[0], 0, 2, Red, Yellow, Green, Blue);
            tileset[1] = new Tile(1, TileTextures[1], 1, 3, Green, Blue, Green, Blue);
            tileset[2] = new Tile(2, TileTextures[2], 1, 1, Red, Yellow, Red, Yellow);
            tileset[3] = new Tile(3, TileTextures[3], 1, 2, Green, Blue, Red, Yellow);
            tileset[4] = new Tile(4, TileTextures[4], 0, 0, Red, Blue, Green, Yellow);
            tileset[5] = new Tile(5, TileTextures[5], 0, 3, Green, Yellow, Green, Yellow);
            tileset[6] = new Tile(6, TileTextures[6], 0, 1, Red, Blue, Red, Blue);
            tileset[7] = new Tile(7, TileTextures[7], 1, 0, Green, Yellow, Red, Blue);

            return tileset;
        }

        /// <summary>
        /// Generates a tileset texture.
        /// </summary>
        /// <param name="tileset">The tileset to be used for the tileset texture.</param>
        /// <returns></returns>
        public Texture2D generateTilesetTexture(Tile[] tileset)
        {
            int width = tileset[0].Texture.width * ColumnCount;
            int height = tileset[0].Texture.height * RowCount;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, true);

            foreach (Tile tile in tileset)
            {
                int x = tile.Column * tile.Texture.width;
                int y = tile.Row * tile.Texture.height;

                for (int i = 0; i < tile.Texture.width; i++)
                {
                    for (int j = 0; j < tile.Texture.height; j++)
                    {
                        texture.SetPixel(x + i, y + j, tile.Texture.GetPixel(i, j));
                    }
                }
            }

            texture.anisoLevel = 4;
            texture.Apply();

            Debug.Log("Generated new tileset.");
            return texture;
        }

        /// <summary>
        /// Generates a pattern texture.
        /// </summary>
        /// <param name="width">The desired width of the texture.</param>
        /// <param name="height">The desired height of the texture.</param>
        /// <param name="tileset">The tileset to be used for the Wang pattern.</param>
        /// <returns></returns>
        public Texture2D generatePatternTexture(int width, int height, Tile[] tileset)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile west = null;
                    if (i > 0)
                    {
                        int id = Mathf.RoundToInt(texture.GetPixel(i - 1, j).r / 10f * 255f);
                        west = findTile(id, tileset);
                    }

                    Tile south = null;
                    if (j > 0)
                    {
                        int id = Mathf.RoundToInt(texture.GetPixel(i, j - 1).r / 10f * 255f);
                        south = findTile(id, tileset);
                    }

                    Tile[] compatibleTiles = findCompatibleTiles(west, south, tileset);
                    Tile tile = randomTile(compatibleTiles);
                    texture.SetPixel(i, j, new Color(tile.Id * 10f / 255f, tile.Row / 255f, tile.Column / 255f));
                }
            }

            texture.Apply();
            texture.filterMode = FilterMode.Point;
            Debug.Log("Generated new pattern.");
            return texture;
        }

        /// <summary>
        /// Chooses a random tile from a tileset.
        /// </summary>
        /// <param name="tileset">The tileset from which the tile is randomly chosen.</param>
        /// <returns>Returns the randomly chosen tile.</returns>
        private Tile randomTile(Tile[] tileset)
        {
            return tileset[Random.Range(0, tileset.Length)];
        }

        /// <summary>
        /// Finds the tile in the tileset that corresponds to the supplied tile id.
        /// </summary>
        /// <param name="id">The id of the tile that needs to be found.</param>
        /// <param name="tileset">The tileset in which the tile should be found.</param>
        /// <returns>Returns the found tile.</returns>
        private Tile findTile(int id, Tile[] tileset)
        {
            return tileset.Where(tile => tile.Id == id).First();
        }

        /// <summary>
        /// Finds the tiles in the supplied tileset that have compatible colors on the west and south sides.
        /// </summary>
        /// <param name="west">The tile that lies west to the current tile.</param>
        /// <param name="south">The tile that lies south to the current tile.</param>
        /// <param name="tileset">The tileset in which the tiles should be found.</param>
        /// <returns></returns>
        private Tile[] findCompatibleTiles(Tile west, Tile south, Tile[] tileset)
        {
            if (west == null && south == null)
            {
                return tileset;
            }
            else if (west != null && south == null)
            {
                return tileset.Where(tile => tile.West == west.East).ToArray();
            }
            else if (west == null && south != null)
            {
                return tileset.Where(tile => tile.South == south.North).ToArray();
            }
            else // (west != null && south != null)
            {
                return tileset.Where(tile => tile.West == west.East && tile.South == south.North).ToArray();
            }
        }

        public class Tile
        {
            public int Id;

            public Texture2D Texture;

            public int Row;
            public int Column;

            public int North;
            public int East;
            public int South;
            public int West;

            public Tile(int id, Texture2D texture, int row, int column, int north, int east, int south, int west)
            {
                this.Id = id;
                this.Texture = texture;
                this.Row = row;
                this.Column = column;
                this.North = north;
                this.East = east;
                this.South = south;
                this.West = west;
            }
        }
    }
}