using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    [CustomPropertyDrawer(typeof(EncryptedDouble))]
    public class EncryptedDoubleDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var encryptedValue = property.FindPropertyRelative("encryptedValue");
            double value = EncryptedDouble.Decrypt(encryptedValue.longValue);
            EditorGUI.LabelField(position, label.text, value.ToString());
        }
    }
}
