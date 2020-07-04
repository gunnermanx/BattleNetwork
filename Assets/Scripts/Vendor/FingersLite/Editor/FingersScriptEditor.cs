using System;

using UnityEngine;
using UnityEditor;

namespace DigitalRubyShared
{
    [CustomEditor(typeof(FingersScript))]
    public class FingersScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {            
            DrawDefaultInspector();
        }
    }
}