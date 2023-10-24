using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MagicExcel {
    public class MagicExcelWindow : EditorWindow {
        private const string SETTING_PATH = "Assets/MagicExcelCache.json";
        private const string ASSET_NAME = "SerializableSet.asset";
        private static bool justRecompiled;
        private List<SheetSource> sheets;

        static MagicExcelWindow() {
            justRecompiled = true;
        }

        [MenuItem("Window/Magic Excel")]
        public static MagicExcelWindow Get() {
            return GetWindow<MagicExcelWindow>("Magic Excel Config");
        }

        public SettingData setting;

        private void Awake() {
            setting = LoadSetting();
        }

        private void OnDestroy() {
            SaveSetting();
        }

        public void OnGUI() {
            //Base Settings
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 200;
            setting.configOutputFolder = EditorGUILayout.TextField("Config Output", setting.configOutputFolder);
            setting.assetOutputFolder = EditorGUILayout.TextField("Asset Output", setting.assetOutputFolder);
            setting.spreadsheetId = EditorGUILayout.TextField("Spreadsheet Id", setting.spreadsheetId);
            setting.ignoreSheetName = EditorGUILayout.TextField("Bỏ qua Sheet khi tên bắt đầu bằng", setting.ignoreSheetName);
            setting.protectedSheetName = EditorGUILayout.TextField("Mã hóa Sheet khi tên bắt đầu bằng", setting.protectedSheetName);
            GUILayout.Label("Lưu ý 1: Để trường phía trên trống nếu muốn mã hóa tất cả");
            GUILayout.Label("Lưu ý 2: Chỉ int, long, float, double và kiểu mảng của chúng được mã hóa");
            if (!string.IsNullOrWhiteSpace(setting.protectedSheetName)) {
                GUILayout.Label($"Lưu ý 3: Phần tiền tố sẽ bị bỏ đi, Sheet tên là {setting.protectedSheetName}_ABC thì tên class sẽ là ABC");
            }

            //Operation
            EditorGUILayout.Space();
            GUILayout.Label("Operation", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear Output")) {
                if (EditorUtility.DisplayDialog("Clear Output",
                "Are you sure you want to clear " + setting.configOutputFolder + " and " + setting.assetOutputFolder + "/" + ASSET_NAME,
                "Yes", "No")) {
                    ClearOutput();
                }
            }

            if (GUILayout.Button("Output")) {
                Output();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Lưu ý:");
            GUILayout.Label("  - Thiết lâp 'File/Publish to the web' để tool có thể đọc được file Google Sheet");
            GUILayout.Label("  - Không được để trống dòng comment");

            if (GUI.changed) {
                SaveSetting();
            }
        }

        private void ClearOutput() {
            //Clear Config
            if (Directory.Exists(setting.configOutputFolder)) {
                Directory.Delete(setting.configOutputFolder, true);
                File.Delete(setting.configOutputFolder + ".meta");
            }

            //Clear Asset
            string assetPath = setting.assetOutputFolder + "/" + ASSET_NAME;
            if (File.Exists(assetPath)) {
                File.Delete(assetPath);
                File.Delete(assetPath + ".meta");
            }

            AssetDatabase.Refresh();
        }

        private void Output() {
            DownloadData();
            if (sheets == null || sheets.Count == 0) {
                Debug.LogError("Sheets Id is invalid or Sheets empty");
                return;
            }

            if (!Directory.Exists(setting.configOutputFolder)) {
                Directory.CreateDirectory(setting.configOutputFolder);
            }
            if (!Directory.Exists(setting.serializerOutputFolder)) {
                Directory.CreateDirectory(setting.serializerOutputFolder);
            }

            SheetGenerator.Generate(sheets, setting.configOutputFolder);
            SerializableSetGenerator.Generate(sheets, setting.serializerOutputFolder);
            DeserializerGenerator.Generate(sheets, setting.serializerOutputFolder);
            AssetDatabase.Refresh();

            if (EditorApplication.isCompiling) {
                waitingForSerialize = true;
                Debug.Log("Waiting For Serialize...");
            } else {
                Serialize();
            }
        }

        private void DownloadData() {
            try {
                XLSXUtils.Download(setting.spreadsheetId);
                EditorUtility.DisplayProgressBar("Magic Excel", $"Reading class info", 0);
                sheets = XLSXUtils.Read(setting.protectedSheetName, setting.ignoreSheetName);
            } catch (Exception) {
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ReadDataFromDisk() {
            try {
                EditorUtility.DisplayProgressBar("Magic Excel", $"Reading xlsx file", 0);
                sheets = XLSXUtils.Read(setting.protectedSheetName, setting.ignoreSheetName);
            } catch (Exception) {
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }

        private bool waitingForSerialize = false;
        private void Update() {
            if (justRecompiled && waitingForSerialize) {
                waitingForSerialize = false;
                Serialize();
            }
            justRecompiled = false;
        }

        private void Serialize() {
            ReadDataFromDisk();
            if (sheets.Count == 0) {
                Debug.LogError("Sheets Empty!");
                return;
            }

            if (!Directory.Exists(setting.assetOutputFolder)) {
                Directory.CreateDirectory(setting.assetOutputFolder);
            }

            UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sheets);
            string o = setting.assetOutputFolder + "/" + ASSET_NAME;
            if (File.Exists(o)) {
                UnityEngine.Object old = AssetDatabase.LoadMainAssetAtPath(o);
                if (old != null) {
                    EditorUtility.CopySerialized(set, old);
                } else {
                    AssetDatabase.CreateAsset(set, o);
                }
            } else {
                AssetDatabase.CreateAsset(set, o);
            }
            Debug.Log("Complete!");
        }

        public static SettingData LoadSetting() {
            if (File.Exists(SETTING_PATH)) {
                string content = File.ReadAllText(SETTING_PATH);
                return JsonUtility.FromJson<SettingData>(content);
            } else {
                return new SettingData();
            }
        }

        private void SaveSetting() {
            string json = JsonUtility.ToJson(setting, true);
            File.WriteAllText(SETTING_PATH, json);
        }

        [Serializable]
        public class SettingData {
            public string configOutputFolder = "Assets/Output";
            public string assetOutputFolder = "Assets/Resources";
            public string spreadsheetId = "1ewITUu3LEUuFTUeZ9v1ErzzIg7wp7ZQGF1Nd6lKJmkw";
            public string ignoreSheetName = "Source";
            public string protectedSheetName = "Protected";
            public string serializerOutputFolder { get { return configOutputFolder + "/Serializer"; } }
        }
    }
}