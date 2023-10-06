using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopItemSO), true)]
public class ShopItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Skip the default "script" field

        // Loop through every property
        do
        {
            // Handle special cases
            if (property.name == "CanUnlockByGold")
            {
                EditorGUILayout.PropertyField(property);
                if (property.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Price"));
                }
            }
            else if (property.name == "CanUnlockByAds")
            {
                EditorGUILayout.PropertyField(property);
                if (property.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AdsToUnlock"));
                }
            }
            else
            {
                // Default handling for all other fields
                EditorGUILayout.PropertyField(property, true);
            }
        } while (property.NextVisible(false));

        serializedObject.ApplyModifiedProperties();
    }
}
