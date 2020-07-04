using UnityEditor;
using UnityEngine;

class Common
{
    public static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        addButtonContent = new GUIContent("+", "add element"),
        deleteButtonContent = new GUIContent("-", "delete");

    public static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
}

