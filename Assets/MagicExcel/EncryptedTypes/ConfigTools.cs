using System;

//https://github.com/RickJiangShu
namespace MagicExcel {
    public class ConfigTools {

        private static Type SourceBaseType2Type(string sourceBaseType) {
            switch (sourceBaseType) {
                case "bool":
                    return typeof(bool);
                case "byte":
                    return typeof(byte);
                case "ushort":
                    return typeof(ushort);
                case "uint":
                    return typeof(uint);
                case "sbyte":
                    return typeof(sbyte);
                case "short":
                    return typeof(short);
                case "int":
                    return typeof(int);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "string":
                    return typeof(string);
                case "EncryptedInt":
                    return typeof(EncryptedInt);
                case "EncryptedLong":
                    return typeof(EncryptedLong);
                case "EncryptedFloat":
                    return typeof(EncryptedFloat);
                case "EncryptedDouble":
                    return typeof(EncryptedDouble);
                default:
                    return null;
            }
        }

        private static object SourceValue2BaseObject(string sourceBaseType, string sourceValue) {
            switch (sourceBaseType) {
                case "bool":
                    return sourceValue != "0" && sourceValue != "false" && sourceValue != "False" && sourceValue != "FALSE";
                case "byte":
                    return byte.Parse(sourceValue);
                case "ushort":
                    return ushort.Parse(sourceValue);
                case "uint":
                    return uint.Parse(sourceValue);
                case "sbyte":
                    return sbyte.Parse(sourceValue);
                case "short":
                    return short.Parse(sourceValue);
                case "int":
                    return int.Parse(sourceValue);
                case "long":
                    return long.Parse(sourceValue);
                case "ulong":
                    return ulong.Parse(sourceValue);
                case "float":
                    return float.Parse(sourceValue);
                case "double":
                    return double.Parse(sourceValue);
                case "string":
                    return sourceValue;
                case "EncryptedInt":
                    return new EncryptedInt(int.Parse(sourceValue));
                case "EncryptedLong":
                    return new EncryptedLong(long.Parse(sourceValue));
                case "EncryptedFloat":
                    return new EncryptedFloat(float.Parse(sourceValue));
                case "EncryptedDouble":
                    return new EncryptedDouble(double.Parse(sourceValue));
                default:
                    return sourceValue;
            }
        }

        private static bool IsArrayType(string sourceType, out string sourceBaseType) {
            int idx = sourceType.LastIndexOf('[');
            if (idx != -1) {
                sourceBaseType = sourceType.Substring(0, idx);
            } else {
                sourceBaseType = sourceType;
            }
            return idx != -1;
        }

        public static object SourceValue2Object(string sourceType, string sourceValue) {
            if (IsArrayType(sourceType, out string sourceBaseType)) {
                return SourceValue2ArrayObject(sourceValue, sourceBaseType);
            } else {
                return SourceValue2BaseObject(sourceType, sourceValue);
            }
        }

        /// <summary>
        /// Chuyển đổi string thành mảng (VD 1, 2, 3 thành mảng int[] { 1, 2, 3 })
        /// </summary>
        public static Array SourceValue2ArrayObject(string sourceValue, string sourceBaseType) {
            if (string.IsNullOrEmpty(sourceValue) || sourceValue == "0") return null;
            string[] values = sourceValue.Split(',');
            Type type = SourceBaseType2Type(sourceBaseType);
            Array array = Array.CreateInstance(type, values.Length);
            for (int i = 0, l = array.Length; i < l; i++) {
                object value = SourceValue2BaseObject(sourceBaseType, values[i]);
                array.SetValue(value, i);
            }
            return array;
        }
    }
}
