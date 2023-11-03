using Application = UnityEngine.Application;

namespace Falcon.FalconCore.Editor.Utils
{
    public static class FalconEditorInfo
    {
        public static string AppDataPath { get; private set; }

        public static void Init()
        {
            AppDataPath = Application.dataPath;
        }
    }
}
