using System;
using UnityEngine;
using UnityEngine.Scripting;
using System.Globalization;
using Falcon;
using Newtonsoft.Json;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Falcon.FalconCore.Scripts.Utils
{
    public class FalconInfoUtils : IFalconInit
    {
        private const string AccountIdKey = "FALCON_SDK_account_id";
        private const string FirstLoginDateKey = "FIRST_DATE";
        private const string FirstLoginMillisKey = "CREATE_DATE";
        
        public static readonly string GameId = Application.identifier.ToLower();
        public static readonly string GameName = Application.productName.ToLower();
        public static readonly string Platform = Application.platform.ToString().ToLower();
        public static readonly string AppVersion = Application.version.ToLower();
        public static readonly string SdkCoreVersion = "2.1.5";
        public static readonly string DeviceName = SystemInfo.deviceName.ToLower();
        public static readonly string DeviceId = GetDeviceId();
        public static readonly DateTime FirstLogInDateTime = FalconData.Instance.GetOrSet(FirstLoginDateKey, DateTime.Now);
        public static readonly long FirstLogInMillis = FalconData.Instance.GetOrSet(FirstLoginMillisKey, FalconTimeUtils.CurrentTimeMillis());
        private static string GetDeviceId()
        {

#if UNITY_EDITOR || UNITY_ANDROID
            return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_IOS
        string key = "falcon_" + Application.identifier + "_uuid";
        string uuid = UUIDiOS.GetKeyChainValue(key);

        if (!string.IsNullOrEmpty(uuid))
        {
            return uuid;
        }
        else
        {
            uuid = Guid.NewGuid().ToString();
            UUIDiOS.SaveKeyChainValue(key, uuid);
            return uuid;
        }
#endif
        }

        public static string AccountID
        {
            get { return FalconData.Instance.GetOrDefault(AccountIdKey, DeviceId); }
            set
            {
                FalconThreadUtils.Execute(() =>
                {
                    FalconData.Instance.Save(AccountIdKey, value);
                });
            }
        }

        [Preserve]
        public FalconInfoUtils(){}
    
        public void Init()
        {
            FalconLogUtils.Info("GameId :" + GameId, "#ecbd77");
            FalconLogUtils.Info("GameName :" + GameName, "#ecbd77");
            FalconLogUtils.Info("Platform :" + Platform, "#ecbd77");
            FalconLogUtils.Info("AppVersion :" + AppVersion, "#ecbd77");
            FalconLogUtils.Info("SdkCoreVersion :" + SdkCoreVersion, "#ecbd77");
            FalconLogUtils.Info("DeviceName :" + DeviceName, "#ecbd77");
            FalconLogUtils.Info("DeviceId :" + DeviceId, "#ecbd77");
        }

        public int GetPriority()
        {
            return 30;
        }

        public bool InitInMainThread()
        {
            return true;
        }
    }
}