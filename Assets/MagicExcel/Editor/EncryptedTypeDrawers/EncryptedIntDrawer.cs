using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    [CustomPropertyDrawer(typeof(EncryptedInt))]
    public class EncryptedIntDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var encryptedValue = property.FindPropertyRelative("encryptedValue");
            int value = EncryptedInt.Decrypt(encryptedValue.intValue);
            EditorGUI.LabelField(position, label.text, value.ToString());
        }
    }
}
