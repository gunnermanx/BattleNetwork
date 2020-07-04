using UnityEditor;
using UnityEngine;
using BattleNetwork.Battle;

[CustomEditor(typeof(ArenaData))]
public class ArenaDataEditor : Editor
{
    private SerializedProperty tiles;
    private SerializedProperty player1StartingTileName;
    private SerializedProperty player2StartingTileName;

    private void OnEnable()
    {
        tiles = serializedObject.FindProperty("tiles");
        player1StartingTileName = serializedObject.FindProperty("player1StartingTileName");
        player2StartingTileName = serializedObject.FindProperty("player2StartingTileName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ShowAnchorList(tiles);

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowAnchorList(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list, false);

        EditorGUILayout.PropertyField(player1StartingTileName);
        EditorGUILayout.PropertyField(player2StartingTileName);

        EditorGUI.indentLevel += 1;
        if (list.isExpanded)
        {
            for (int i = 0; i < list.arraySize; i++)
            {                
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    if (GUILayout.Button(Common.moveButtonContent, EditorStyles.miniButtonRight, Common.miniButtonWidth))
                    {
                        list.MoveArrayElement(i, i + 1);
                    }
                    if (GUILayout.Button(Common.duplicateButtonContent, EditorStyles.miniButtonRight, Common.miniButtonWidth))
                    {
                        list.InsertArrayElementAtIndex(i);
                    }
                    if (GUILayout.Button(Common.deleteButtonContent, EditorStyles.miniButtonRight, Common.miniButtonWidth)) {
                        int oldSize = list.arraySize;
                        list.DeleteArrayElementAtIndex(i);
                        if (list.arraySize == oldSize)
                        {
                            list.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            
            if (list.arraySize == 0 && GUILayout.Button(Common.addButtonContent))
            {
                list.arraySize += 1;
            }

        }        
        EditorGUI.indentLevel -= 1;
    }
}