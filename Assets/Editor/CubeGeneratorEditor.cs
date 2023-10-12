using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR


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
        if (GUILayout.Button("Create Cubes base on 3D Grid"))
        {
            _generator.Create3DGridLevel();
        }
        
        if (GUILayout.Button("Create Cubes base on 3D model"))
        {
            _generator.Create3DShapeLevel();
        }
        
        if (GUILayout.Button("Clear"))
        {
            _generator.ClearCube();
        }
        
        if (GUILayout.Button("Export"))
        {
            _generator.ExportObject();
        }
        
        if (GUILayout.Button("LoadLevel"))
        {
            _generator.StartCoroutine(_generator.SetupLevel());
        }
    }
}
#endif