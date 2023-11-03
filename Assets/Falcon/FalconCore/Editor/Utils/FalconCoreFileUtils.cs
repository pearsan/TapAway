using System.IO;
using UnityEngine;

namespace Falcon.FalconCore.Editor.Utils
{
    public static class FalconCoreFileUtils
    {
        // private static void ChangeFileLine(string newText, string filePath, int lineToEdit)
        // {
        //     string[] arrLine = File.ReadAllLines(filePath);
        //     arrLine[lineToEdit - 1] = newText;
        //     File.WriteAllLines(filePath, arrLine);
        // }
        //
        // public static IEnumerator<int> SearchLinesHasSubString(string filePath, string subString)
        // {
        //     string[] arrLine = File.ReadAllLines(filePath);
        //     for (int i = 0; i < arrLine.Length; i++)
        //     {
        //         if (arrLine[i].Contains(subString))
        //         {
        //             yield return i;
        //         }
        //     }
        // }
        

        public static string GetFalconPluginFolder()
        {
            //will use FalconCore's parent folder as milestone for other plugins
            string[] directory =
                Directory.GetDirectories(Application.dataPath, "FalconCore", SearchOption.AllDirectories);
            if (directory.Length == 0) return null;
            else
            {
                DirectoryInfo result = Directory.GetParent(directory[0].Contains("Release") ? directory[1] : directory[0]);
                return result == null ? null : result.FullName ;
            }
        }

        public static void RewriteLineInFile(string filePath, string oldLine, string newLine)
        {
            string[] arrLine = File.ReadAllLines(filePath);

            for (int i = 0; i < arrLine.Length; i++)
            {
                if (arrLine[i].StartsWith(oldLine))
                {
                    arrLine[i] = newLine;
                    File.WriteAllLines(filePath, arrLine);
                    return;
                }
            }
        }

    }
}