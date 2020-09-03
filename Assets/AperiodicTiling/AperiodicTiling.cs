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

        private const int RowCount = 4;
        private const int ColumnCount = 4;

        public Texture2D[] TileTextures = new Texture2D[RowCount * ColumnCount];
        public Tile[] Tileset = new Tile[RowCount * ColumnCount];
        public Texture2D TilesetTexture;

        public int PatternSize = 256;
        public Texture2D PatternTexture;

        /// <summary>
        /// Generates a tileset consisting of 16 Wang tiles.
        /// </summary>
        /// <returns>A tileset as an array of tiles.</returns>
        public Tile[] generateTileset()
        {
            Tile[] tileset = new Tile[RowCount * ColumnCount];

            // The numerical representation of the tileset.
            // N = North, E = East, S = South, W = West.
            // NESW
            // 1313
            // 0313
            // 1213
            // 0213
            // 1303
            // 0303
            // 1203
            // 0203
            // 1312
            // 0312
            // 1212
            // 0212
            // 1302
            // 0302
            // 1202
            // 0202

            tileset[0] = new Tile(0, TileTextures[0], 0, 0, Green, Blue, Green, Blue);
            tileset[1] = new Tile(1, TileTextures[1], 1, 0, Red, Blue, Green, Blue);
            tileset[2] = new Tile(2, TileTextures[2], 0, 1, Green, Yellow, Green, Blue);
            tileset[3] = new Tile(3, TileTextures[3], 1, 1, Red, Yellow, Green, Blue);
            tileset[4] = new Tile(4, TileTextures[4], 3, 0, Green, Blue, Red, Blue);
            tileset[5] = new Tile(5, TileTextures[5], 2, 0, Red, Blue, Red, Blue);
            tileset[6] = new Tile(6, TileTextures[6], 3, 1, Green, Yellow, Red, Blue);
            tileset[7] = new Tile(7, TileTextures[7], 2, 1, Red, Yellow, Red, Blue);
            tileset[8] = new Tile(8, TileTextures[8], 0, 3, Green, Blue, Green, Yellow);
            tileset[9] = new Tile(9, TileTextures[9], 1, 3, Red, Blue, Green, Yellow);
            tileset[10] = new Tile(10, TileTextures[10], 0, 2, Green, Yellow, Green, Yellow);
            tileset[11] = new Tile(11, TileTextures[11], 1, 2, Red, Yellow, Green, Yellow);
            tileset[12] = new Tile(12, TileTextures[12], 3, 3, Green, Blue, Red, Yellow);
            tileset[13] = new Tile(13, TileTextures[13], 2, 3, Red, Blue, Red, Yellow);
            tileset[14] = new Tile(14, TileTextures[14], 3, 2, Green, Yellow, Red, Yellow);
            tileset[15] = new Tile(15, TileTextures[15], 2, 2, Red, Yellow, Red, Yellow);

            return tileset;
        }

        /// <summary>
        /// Generates a tileset texture.
        /// </summary>
        /// <param name="tileset">The tileset to be used for the tileset texture.</param>
        /// <returns>The generated tileset texture.</returns>
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
        /// <returns>The generated pattern texture.</returns>
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
        /// <returns>The randomly chosen tile.</returns>
        private Tile randomTile(Tile[] tileset)
        {
            return tileset[Random.Range(0, tileset.Length)];
        }

        /// <summary>
        /// Finds the tile in the tileset that corresponds to the supplied tile id.
        /// </summary>
        /// <param name="id">The id of the tile that needs to be found.</param>
        /// <param name="tileset">The tileset in which the tile should be found.</param>
        /// <returns>The found tile.</returns>
        private Tile findTile(int id, Tile[] tileset)
        {
            return tileset.Where(tile => tile.Id == id).First();
        }

        /// <summary>
        /// Finds the tiles in the supplied tileset that have compatible colors on the west and south edges.
        /// </summary>
        /// <param name="west">The tile that lies west to the current tile.</param>
        /// <param name="south">The tile that lies south to the current tile.</param>
        /// <param name="tileset">The tileset in which the tiles should be found.</param>
        /// <returns>The array of tiles that are compatible with the given edge colors.</returns>
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

        /// <summary>
        /// Saves the given texture.
        /// </summary>
        /// <param name="texture">The texture to be saved.</param>
        /// <param name="name">The name of the file.</param>
        public void saveTexture(Texture2D texture, string name)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/AperiodicTiling/" + name + ".png", bytes);
            Debug.Log("Saved texture.");
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