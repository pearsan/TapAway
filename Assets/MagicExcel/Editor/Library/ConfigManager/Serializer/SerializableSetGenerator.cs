using System.Collections.Generic;
using System.IO;

//https://github.com/RickJiangShu
namespace MagicExcel {
    public class SerializableSetGenerator {
        private const string template =
@"[System.Serializable]
public class SerializableSet : UnityEngine.ScriptableObject
{
/*ConfigDeclarations*/
}
";
        private const string template2 =
@"    public /*ConfigName*/[] /*SourceName*/s;
";

        public static void Generate(List<SheetSource> sheets, string outputFolder) {
            string outputPath = outputFolder + "/SerializableSet.cs";
            string content = template;

            string configDeclarations = "";
            foreach (SheetSource sheet in sheets) {
                string declaration = template2;
                declaration = declaration.Replace("/*ConfigName*/", sheet.className);
                declaration = declaration.Replace("/*SourceName*/", sheet.originalName);
                configDeclarations += declaration;
            }
            content = content.Replace("/*ConfigDeclarations*/", configDeclarations);

            File.WriteAllText(outputPath, content);
        }
    }
}
