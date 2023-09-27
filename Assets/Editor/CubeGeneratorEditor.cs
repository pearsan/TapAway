using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubeGenerator), true)]

public class CubeGeneratorEditor : Editor
{
    private CubeGenerator _generator;

    private void Awake()
    {
        _generator = (CubeGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Cubes"))
        {
            _generator.ResetGame();
        }
        
        if (GUILayout.Button("Clear"))
        {
            _generator.ClearCube();
        }
    }
}
