using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    [CustomPropertyDrawer(typeof(EncryptedLong))]
    public class EncryptedLongDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var encryptedValue = property.FindPropertyRelative("encryptedValue");
            long value = EncryptedLong.Decrypt(encryptedValue.longValue);
            EditorGUI.LabelField(position, label.text, value.ToString());
        }
    }
}
