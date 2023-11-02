using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    public static class XLSXUtils {
        private const string TMP_PATH = "Temp/MagicExcel.xlsx";
        private static readonly string[] ENCRYPTED_TYPE = new string[] { "int", "float", "long", "double" };

        public static void Download(string spreadsheetId) {
            EditorUtility.DisplayProgressBar("Magic Excel", "Downloading Google Spreadsheet", 0);
            using (var client = new WebClient()) {
                client.DownloadFile($"https://docs.google.com/spreadsheets/d/e/{spreadsheetId}/pub?output=xlsx", TMP_PATH);
            }
        }

        public static List<SheetSource> Read(string protectedSheetName, string ignoreSheetName) {
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(TMP_PATH))) {
                var result = new List<SheetSource>();
                foreach (var sheet in excelPackage.Workbook.Worksheets) {
                    if (sheet.Name.StartsWith(ignoreSheetName)) {
                        continue;
                    }
                    string key = sheet.Name;
                    if (!string.IsNullOrWhiteSpace(protectedSheetName) && sheet.Name.StartsWith(protectedSheetName)) {
                        key = key.Substring(protectedSheetName.Length).TrimStart(new char[] { ' ', '-', '_' });
                    }
                    int row, column;
                    for (row = 1; row < sheet.Dimension.Rows; row++) {
                        //Check cột 1 là id
                        if (string.IsNullOrWhiteSpace(sheet.GetValue<string>(row + 1, 1))) {
                            break;
                        }
                    }
                    for (column = 1; column < sheet.Dimension.Columns; column++) {
                        //Check dòng 2 là kiểu dữ liệu cho chắc
                        if (string.IsNullOrWhiteSpace(sheet.GetValue<string>(2, column + 1))) {
                            break;
                        }
                    }
                    if (row == 1 || column == 1) {
                        Debug.LogError($"Sheet {sheet.Name} is empty");
                        continue;
                    }
                    string[,] matrix = new string[row, column];
                    for (int i = 0; i < row; i++) {
                        for (int j = 0; j < column; j++) {
                            matrix[i, j] = sheet.Cells[i + 1, j + 1].Text;
                        }
                    }
                    SheetSource sheetSource = new SheetSource {
                        originalName = key,
                        className = key + "Sheet",
                        matrix = matrix
                    };
                    if (string.IsNullOrWhiteSpace(protectedSheetName) || sheet.Name.StartsWith(protectedSheetName)) {
                        for (int i = 1; i < sheetSource.Column; i++) {
                            var type = sheetSource.matrix[1, i];
                            if (ENCRYPTED_TYPE.Any(ot => type.StartsWith(ot))) {
                                sheetSource.matrix[1, i] = "Encrypted" + char.ToUpper(type[0]) + type.Substring(1);
                            }
                        }
                    }
                    result.Add(sheetSource);
                }
                return result;
            }
        }
    }
}