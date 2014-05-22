using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileTool))]
public class TileToolEditor : Editor
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
    TileTool targetTool;

    int currentTile;

    int repeatX;
    int repeatY;
	#endregion

	#region Class Constants
    const float MAX_WIDTH = 300.0f;

    const float DEFAULT_HEIGHT = 25.0f;
    const float DEFAULT_SPACING = 10.0f;

    const float TILE_POS_WIDTH = 150.0f;
    const float TILE_POS_SPACING = 10.0f;

    const float CONTROL_BUTTON_WIDTH = MAX_WIDTH;
    const float CONTROL_BUTTON_HEIGHT = 25.0f;
    const float CONTROL_BUTTON_SPACING = 10.0f;
	#endregion

	//Put all initialization code here
	//Remember tMonoBehaviouro comment!
	protected void OnEnable()
	{
        currentTile = 0;

        repeatX = 0;
        repeatY = 0;

        targetTool = (TileTool) target;
        targetTool.InitializeTool();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Height(DEFAULT_HEIGHT));
        EditorGUILayout.SelectableLabel("Current Tile Position: ", 
            GUILayout.ExpandWidth(true),
            GUILayout.Height(DEFAULT_HEIGHT));
        EditorGUILayout.SelectableLabel(targetTool.getPos().ToString(), 
            GUILayout.ExpandWidth(true),
            GUILayout.Height(DEFAULT_HEIGHT));
        GUILayout.EndHorizontal();

        targetTool.mTileSize = EditorGUILayout.Vector2Field("Tile Size: ", targetTool.mTileSize, 
            GUILayout.ExpandWidth(true),
            GUILayout.Height(DEFAULT_HEIGHT));
        targetTool.UpdateCrossHairs();

        #region Control Buttons
        GUILayout.BeginVertical(GUILayout.ExpandHeight(false));
        if (GUILayout.Button("UP", 
            GUILayout.ExpandWidth(true),
            GUILayout.Height(CONTROL_BUTTON_HEIGHT))
            || Input.GetKey(KeyCode.UpArrow))
            targetTool.Shift(0, targetTool.mTileSize.y);
        GUILayout.Space(CONTROL_BUTTON_SPACING / 2);

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(30));
        if (GUILayout.Button("LEFT",
            GUILayout.ExpandWidth(true),
            GUILayout.Height(CONTROL_BUTTON_HEIGHT))
            || Input.GetKey(KeyCode.LeftArrow))
            targetTool.Shift(-targetTool.mTileSize.x, 0);
        GUILayout.Space(CONTROL_BUTTON_SPACING);
        if (GUILayout.Button("RIGHT",
            GUILayout.ExpandWidth(true),
            GUILayout.Height(CONTROL_BUTTON_HEIGHT))
            || Input.GetKey(KeyCode.RightArrow))
            targetTool.Shift(targetTool.mTileSize.x, 0);
        GUILayout.EndHorizontal();

        GUILayout.Space(CONTROL_BUTTON_SPACING / 2);
        if (GUILayout.Button("DOWN",
            GUILayout.ExpandWidth(true),
            GUILayout.Height(CONTROL_BUTTON_HEIGHT))
            || Input.GetKey(KeyCode.DownArrow))
            targetTool.Shift(0, -targetTool.mTileSize.y);
        GUILayout.EndVertical();
        #endregion

        GUILayout.Space(DEFAULT_SPACING * 2);

        Object tileField = EditorGUILayout.ObjectField(
            new GUIContent("Tile Drop Zone: "),  null, typeof(GameObject));

        if (tileField != null)
        {
            Tile toAdd = ((GameObject) tileField).GetComponent<Tile>();
            targetTool.AddTile(toAdd);
        }

        currentTile = EditorGUILayout.Popup(currentTile, targetTool.TileNames());
        targetTool.TileToPlace(currentTile);

        if (GUILayout.Button("Place Tile", GUILayout.ExpandWidth(true), GUILayout.Height(DEFAULT_HEIGHT * 2.0f))
            || Input.GetKey(KeyCode.Space))
            targetTool.PlaceTile();

        GUILayout.Space(DEFAULT_SPACING * 2);

        repeatX = EditorGUILayout.IntField("Repeat X", repeatX, GUILayout.ExpandWidth(true));
        repeatY = EditorGUILayout.IntField("Repeat Y", repeatY, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Repeat Tiles", GUILayout.ExpandWidth(true), GUILayout.Height(DEFAULT_HEIGHT * 2.0f)))
        {
            Vector3 basePos = targetTool.transform.position;

            for (int x = 0; x < repeatX; x++)
            {
                for (int y = 0; y < repeatY; y++)
                {
                    targetTool.PlaceTile();
                    targetTool.Shift(0.0f, targetTool.mTileSize.y);
                }

                basePos.x += targetTool.mTileSize.x;
                targetTool.transform.position = basePos;
            }
        }

        GUILayout.Space(DEFAULT_SPACING * 2);

        if (GUILayout.Button("Empty Tile List", GUILayout.ExpandWidth(true), GUILayout.Height(DEFAULT_HEIGHT * 2.0f)))
            targetTool.ClearTiles();
    }

	#region Update Methods
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
