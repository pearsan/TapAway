using System.Collections.Generic;
using System.IO;

//https://github.com/RickJiangShu
namespace MagicExcel {
    public class SheetGenerator {

        private const string TEMPLETE =
@"using System.Collections.Generic;
using MagicExcel;

[System.Serializable]
public class /*ClassName*/
{
/*DeclareProperties*/
    private static Dictionary</*IDType*/, /*ClassName*/> dictionary = new Dictionary</*IDType*/, /*ClassName*/>();

    /// <summary>
    /// Get /*ClassName*/ by /*IDField*/
    /// </summary>
    /// <param name=""/*IDField*/""></param>
    /// <returns>/*ClassName*/</returns>
    public static /*ClassName*/ Get(/*IDType*/ /*IDField*/)
    {
        return dictionary[/*IDField*/];
    }
    
    public static Dictionary</*IDType*/, /*ClassName*/> GetDictionary()
    {
        return dictionary;
    }

    public static void SetDictionary(Dictionary</*IDType*/, /*ClassName*/> dic) {
        dictionary = dic;
    }
}
";
        private static string templete2 =
@"    /// <summary>
    /// {0}
    /// </summary>
    public {1} {2};

";

        public static void Generate(List<SheetSource> sheets, string outputFolder) {
            foreach (SheetSource src in sheets) {
                string content = TEMPLETE;
                string outputPath = outputFolder + "/" + src.className + ".cs";

                string idType = src.matrix[1, 0];
                string idField = src.matrix[2, 0];

                string declareProperties = "";
                for (int x = 0; x < src.Column; x++) {
                    string comment = src.matrix[0, x];
                    string csType = src.matrix[1, x];
                    string field = src.matrix[2, x];
                    string declare = string.Format(templete2, comment, csType, field);
                    declareProperties += declare;
                }

                content = content.Replace("/*ClassName*/", src.className);
                content = content.Replace("/*DeclareProperties*/", declareProperties);
                content = content.Replace("/*IDType*/", idType);
                content = content.Replace("/*IDField*/", idField);

                File.WriteAllText(outputPath, content);
            }
        }
    }
}
