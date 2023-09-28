using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopGUIManager))]
public class ShopGUIManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("Contents have to attached in Tab script. Tab will control contents", MessageType.Warning);
    }
}
