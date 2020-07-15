using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SimpleWang
{
    [CustomEditor(typeof(PatternGenerator))]
    public class PatternGeneratorEditor : Editor
    {
        private PatternGenerator patternGenerator;

        private const int Red = 0;
        private const int Green = 1;
        private const int Yellow = 2;
        private const int Blue = 3;

        private void OnEnable()
        {
            patternGenerator = (PatternGenerator)target;
            patternGenerator.Tileset = createTileset();
        }

        public override void OnInspectorGUI() //2
        {
            GUIStyle header = new GUIStyle();
            header.fontSize = 16;
            header.alignment = TextAnchor.LowerCenter;
            header.padding.top = 10;
            header.padding.left = 5;
            header.fontStyle = FontStyle.Bold;

            GUIStyle footer = new GUIStyle();
            footer.fontSize = 8;
            footer.alignment = TextAnchor.UpperCenter;
            footer.padding.bottom = 5;
            footer.fontStyle = FontStyle.Italic;

            GUILayout.Label("Aperiodic Tiling", header);
            GUILayout.Label("Author: Thijs Versfelt", footer);
            GUILayout.Label(patternGenerator.Pattern, footer);

            patternGenerator.PatternSize = EditorGUILayout.IntField("Pattern size (w x h):", patternGenerator.PatternSize);

            if (GUILayout.Button("Generate pattern"))
            {
                patternGenerator.Pattern = generatePattern(patternGenerator.PatternSize, patternGenerator.PatternSize, patternGenerator.Tileset);
                patternGenerator.GetComponent<Renderer>().material.SetTexture("_PatternTex", patternGenerator.Pattern);
            }

            if (GUILayout.Button("Save pattern"))
            {
                savePattern(patternGenerator.Pattern);
            }
        }

        /// <summary>
        /// Creates a tileset consisting of 8 Wang tiles.
        /// </summary>
        /// <returns>Returns a tileset as a list of tiles.</returns>
        private List<Tile> createTileset()
        {
            List<Tile> tileset = new List<Tile>();

            tileset.Add(new Tile(0, 0, 2, Red, Green, Yellow, Blue));
            tileset.Add(new Tile(1, 1, 3, Green, Green, Blue, Blue));
            tileset.Add(new Tile(2, 1, 1, Red, Red, Yellow, Yellow));
            tileset.Add(new Tile(3, 1, 2, Green, Red, Blue, Yellow));
            tileset.Add(new Tile(4, 0, 0, Red, Green, Blue, Yellow));
            tileset.Add(new Tile(5, 0, 3, Green, Green, Yellow, Yellow));
            tileset.Add(new Tile(6, 0, 1, Red, Red, Blue, Blue));
            tileset.Add(new Tile(7, 1, 0, Green, Red, Yellow, Blue));

            return tileset;
        }

        /// <summary>
        /// Generates a pattern texture.
        /// </summary>
        /// <param name="width">The desired width of the texture.</param>
        /// <param name="height">The desired height of the texture.</param>
        /// <param name="tileset">The tileset to be used for the Wang pattern.</param>
        /// <returns></returns>
        private Texture2D generatePattern(int width, int height, List<Tile> tileset)
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

                    List<Tile> compatibleTiles = findCompatibleTiles(west, south, tileset);
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
        /// Saves the given pattern texture.
        /// </summary>
        /// <param name="pattern">The pattern texture to be saved.</param>
        private void savePattern(Texture2D pattern)
        {
            byte[] bytes = patternGenerator.Pattern.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/AperiodicTiling/Pattern.png", bytes);
            Debug.Log("Saved pattern.");
        }

        /// <summary>
        /// Chooses a random tile from a tileset.
        /// </summary>
        /// <param name="tileset">The tileset from which the tile is randomly chosen.</param>
        /// <returns>Returns the randomly chosen tile.</returns>
        private Tile randomTile(List<Tile> tileset)
        {
            return tileset[Random.Range(0, tileset.Count)];
        }

        /// <summary>
        /// Finds the tile in the tileset that corresponds to the supplied tile id.
        /// </summary>
        /// <param name="id">The id of the tile that needs to be found.</param>
        /// <param name="tileset">The tileset in which the tile should be found.</param>
        /// <returns>Returns the found tile.</returns>
        private Tile findTile(int id, List<Tile> tileset)
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
        private List<Tile> findCompatibleTiles(Tile west, Tile south, List<Tile> tileset)
        {
            if (west == null && south == null)
            {
                return tileset;
            }
            else if (west != null && south == null)
            {
                return tileset.Where(tile => tile.West == west.East).ToList();
            }
            else if (west == null && south != null)
            {
                return tileset.Where(tile => tile.South == south.North).ToList();
            }
            else // (west != null && south != null)
            {
                return tileset.Where(tile => tile.West == west.East && tile.South == south.North).ToList();
            }
        }
    }

    public class Tile
    {
        public int Id;
        public int Row;
        public int Column;
        public int North;
        public int South;
        public int East;
        public int West;

        public Tile(int id, int row, int column, int north, int south, int east, int west)
        {
            this.Id = id;
            this.Row = row;
            this.Column = column;
            this.North = north;
            this.South = south;
            this.East = east;
            this.West = west;
        }
    }
}