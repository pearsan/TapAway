using System.Collections.Generic;
using System.IO;
using UnityEngine;

//https://github.com/RickJiangShu
namespace MagicExcel {
    public class DeserializerGenerator : ScriptableObject {

        private const string template =
 @"using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class Deserializer
{
    public static void Deserialize(SerializableSet set)
    {
        SerializableSet localSet = null;
        try {
            string key = MagicExcel.Security.Encrypt(nameof(SerializableSet) + Application.version);
            if (PlayerPrefs.HasKey(key)) {
                string json = MagicExcel.Security.Decrypt(PlayerPrefs.GetString(key));
                localSet = JsonConvert.DeserializeObject<SerializableSet>(json);
            }
        } catch (System.Exception ex) {
            Debug.LogError(ex.ToString());
        }
/*SetDictionaries*/
    }
}
";
        private const string template2 =
@"       
        /*ConfigName*/.SetDictionary((localSet?./*SourceName*/s ?? set./*SourceName*/s).ToDictionary(x => x./*IDField*/));";

        public static void Generate(List<SheetSource> sheets, string outputFolder) {
            string outputPath = outputFolder + "/Deserializer.cs";
            string content = template;

            //sheets
            string setDictionaries = "";
            foreach (SheetSource sheet in sheets) {
                string idField = sheet.matrix[2, 0];
                string setScript = template2;

                setScript = setScript.Replace("/*ConfigName*/", sheet.className);
                setScript = setScript.Replace("/*SourceName*/", sheet.originalName);
                setScript = setScript.Replace("/*IDField*/", idField);

                setDictionaries += setScript;
            }

            content = content.Replace("/*SetDictionaries*/", setDictionaries);

            File.WriteAllText(outputPath, content);
        }
    }
}
