using System;

namespace Falcon.FalconCore.Editor
{
    public abstract class PluginInstallResponder
    {
        protected PluginInstallResponder()
        {
        }

        public abstract String GetPackageName();
        public abstract void OnPluginInstalled(String installLocation);
    }
}