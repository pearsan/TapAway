using System.Net.Http;
using System.Threading;
using Falcon.FalconCore.Scripts.Utils;
using UnityEditor;
using UnityEditorInternal;

namespace Falcon.FalconCore.Editor
{
    public class FalconKeyManager
    {
        private const string ValidateURL = "https://data4game.com/falcon-sdk/validate?fkey=";
        private static FalconKeyManager _instance;

        public static FalconKeyManager Instance
        {
            get
            {
                if (_instance == null) _instance = new FalconKeyManager();

                return _instance;
            }
        }

        private const string FalconKeyPrefName = "FALCON_FKEY";

        public string GetFalconKey()
        {
            if (InternalEditorUtility.CurrentThreadIsMainThread())
            {
                return EditorPrefs.GetString(FalconKeyPrefName, "");
            }
            else
            {
                var result = "";
                var oneTimeAction = FalconThreadUtils.MainThread(() => { result = EditorPrefs.GetString(FalconKeyPrefName, "");});
                while(!oneTimeAction.IsDone) Thread.Sleep(100);
                return result;
            }
        }

        public void SaveFalconKey(string fKey)
        {
            FalconGameObjectUtils.Instance.Trigger(() => { EditorPrefs.SetString(FalconKeyPrefName, fKey); });
        }

        public void DeleteFalconKey()
        {
            FalconGameObjectUtils.Instance.Trigger(() => { EditorPrefs.DeleteKey(FalconKeyPrefName); });
        }

        public bool HasFalconKey()
        {
            if (InternalEditorUtility.CurrentThreadIsMainThread())
            {
                return EditorPrefs.HasKey(FalconKeyPrefName);
            }
            else
            {
                var result = false;
                var oneTimeAction = FalconThreadUtils.MainThread(() => { result = EditorPrefs.HasKey(FalconKeyPrefName);});
                while(!oneTimeAction.IsDone) Thread.Sleep(100);
                return result;
            }
        }

        public bool ValidateFalconKey(string fKey)
        {
            var result = FalconNetUtils.DoRequest(HttpMethod.Get, ValidateURL + fKey);
            if (result == "ok\n")
            {
                SaveFalconKey(fKey);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}