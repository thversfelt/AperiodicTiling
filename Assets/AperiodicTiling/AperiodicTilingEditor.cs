using UnityEditor;
using UnityEngine;

namespace AperiodicTiling
{
    [CustomEditor(typeof(AperiodicTiling))]
    public class AperiodicTilingEditor : Editor
    {
        private AperiodicTiling aperiodicTiling;

        private void OnEnable()
        {
            aperiodicTiling = (AperiodicTiling)target;
        }

        public override void OnInspectorGUI()
        {
            GUIStyle header = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            GUIStyle footer = new GUIStyle(GUI.skin.label)
            {
                fontSize = 8,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic
            };

            EditorGUILayout.Space();

            GUILayout.Label("Aperiodic Tiling", header);
            GUILayout.Label("Author: Thijs Versfelt", footer);

            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Box(aperiodicTiling.TilesetTexture, GUILayout.Width(320), GUILayout.Height(160));

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    aperiodicTiling.TileTextures[0] = TextureField("Tile 0", aperiodicTiling.TileTextures[0]);
                    aperiodicTiling.TileTextures[1] = TextureField("Tile 1", aperiodicTiling.TileTextures[1]);
                    aperiodicTiling.TileTextures[2] = TextureField("Tile 2", aperiodicTiling.TileTextures[2]);
                    aperiodicTiling.TileTextures[3] = TextureField("Tile 3", aperiodicTiling.TileTextures[3]);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    aperiodicTiling.TileTextures[4] = TextureField("Tile 4", aperiodicTiling.TileTextures[4]);
                    aperiodicTiling.TileTextures[5] = TextureField("Tile 5", aperiodicTiling.TileTextures[5]);
                    aperiodicTiling.TileTextures[6] = TextureField("Tile 6", aperiodicTiling.TileTextures[6]);
                    aperiodicTiling.TileTextures[7] = TextureField("Tile 7", aperiodicTiling.TileTextures[7]);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (GUILayout.Button("Generate tileset"))
                {
                    aperiodicTiling.Tileset = aperiodicTiling.generateTileset();
                    aperiodicTiling.TilesetTexture = aperiodicTiling.generateTilesetTexture(aperiodicTiling.Tileset);
                    aperiodicTiling.GetComponent<Renderer>().material.SetTexture("_TilesetTex", aperiodicTiling.TilesetTexture);
                }
            }
            GUILayout.EndVertical();
            
            EditorGUILayout.Space();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Box(aperiodicTiling.PatternTexture, GUILayout.Width(320), GUILayout.Height(320));

                aperiodicTiling.PatternSize = EditorGUILayout.IntField("Pattern size (w x h):", aperiodicTiling.PatternSize);

                if (GUILayout.Button("Generate pattern"))
                {
                    aperiodicTiling.PatternTexture = aperiodicTiling.generatePatternTexture(aperiodicTiling.PatternSize, aperiodicTiling.PatternSize, aperiodicTiling.Tileset);
                    aperiodicTiling.GetComponent<Renderer>().material.SetTexture("_PatternTex", aperiodicTiling.PatternTexture);
                }
            }
            GUILayout.EndVertical();
        }

        private static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fixedWidth = 70
                };
                GUILayout.Label(name);
                texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            }
            GUILayout.EndVertical();
            return texture;
        }
    }

    
}