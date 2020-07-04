using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BattleNetwork.Battle;

[CanEditMultipleObjects]
[CustomEditor(typeof(Arena))]
public class ArenaEditor : Editor
{
    private SerializedProperty arenaAnchorsParent;
    private SerializedProperty arenaData;

    private void OnEnable()
    {
        arenaAnchorsParent = serializedObject.FindProperty("arenaAnchorsParent");
        arenaData = serializedObject.FindProperty("arenaData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(arenaData);
        EditorGUILayout.PropertyField(arenaAnchorsParent);
        if (GUILayout.Button("Initialize"))
        {
            InitializeArena();
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void InitializeArena()
    {
        if (arenaData != null)
        {
            Arena a = target as Arena;
            a.Initialize();
        } 
        else
        {
            Debug.Log("arenaData is unset, can't initialize the arena");
        }
    }
}
