using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    [CustomPropertyDrawer(typeof(EncryptedFloat))]
    public class EncryptedFloatDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var encryptedValue = property.FindPropertyRelative("encryptedValue");
            float value = EncryptedFloat.Decrypt(encryptedValue.intValue);
            EditorGUI.LabelField(position, label.text, value.ToString());
        }
    }
}
