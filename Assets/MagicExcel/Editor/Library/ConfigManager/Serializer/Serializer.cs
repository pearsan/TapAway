using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//https://github.com/RickJiangShu
namespace MagicExcel {
    public class Serializer {
        public static object Serialize(List<SheetSource> sheets) {
            Type t = FindType("SerializableSet");
            if (t == null) {
                Debug.LogError("Không tìm thấy class SerializableSet");
                return null;
            }
            object set = ScriptableObject.CreateInstance(t);
            foreach (SheetSource source in sheets) {
                string fieldName = source.originalName + "s";
                Array configs = Source2Configs(source);
                FieldInfo fieldInfo = t.GetField(fieldName);
                fieldInfo.SetValue(set, configs);
            }
            return set;
        }

        /// <summary>
        /// Từ dữ liệu nguồn thành mảng tương ứng
        /// </summary>
        /// <returns></returns>
        private static Array Source2Configs(SheetSource source) {
            Type configType = FindType(source.className);
            int count = source.Row - 3;
            Array configs = Array.CreateInstance(configType, count);
            for (int y = 3, i = 0; i < count; y++, i++) {
                object config = Activator.CreateInstance(configType);
                for (int x = 0; x < source.Column; x++) {
                    string valueType = source.matrix[1, x];
                    string valueField = source.matrix[2, x];
                    string valueString = source.matrix[y, x];
                    FieldInfo field = configType.GetField(valueField);
                    try {
                        object value = ConfigTools.SourceValue2Object(valueType, valueString);
                        field.SetValue(config, value);
                    } catch {
                        Debug.LogError(string.Format("SourceValue2Object Error!valueType={0},valueString={1},source={2},column={3},row={4}", valueType, valueString, source.originalName, x, y));
                    }
                }
                configs.SetValue(config, i);
            }
            return configs;
        }

        /// <summary>
        /// Chỉ để tìm SerializableSet vì nó được tạo bằng code
        /// </summary>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        private static Type FindType(string qualifiedTypeName) {
            Type t = Type.GetType(qualifiedTypeName);
            if (t != null) {
                return t;
            } else {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }
    }
}
