using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoldManager))]
public class GoldManagerEditor : Editor
{
    private string modifyGoldValue;
    private bool showModifyUI;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        showModifyUI = EditorGUILayout.Toggle("Show Modify UI", showModifyUI);

        if (showModifyUI)
        {
            modifyGoldValue = EditorGUILayout.TextField("Modify Gold Value", modifyGoldValue);

            int value = 0;

            if (GUILayout.Button("Modify"))
            {
                if (int.TryParse(modifyGoldValue, out value))
                {
                    GoldManager goldManager = (GoldManager)target;
                    goldManager.HardModifyGoldValue(value);

                    EditorUtility.DisplayDialog("Succeed", $"Đã sửa thành công vàng thành {value}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Input", "Please enter a valid integer.", "OK");
                }
            }
        }
    }
}
