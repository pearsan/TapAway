using System;
using System.Runtime.CompilerServices;
using Falcon.FalconAnalytics.Scripts.Counters;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconAnalytics.Scripts.Message.Wrapper;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public abstract class DwhMessage
    {
        public string abTestingValue;
        public string abTestingVariable;
        public string accountId;
        public string appVersion = FDeviceInfo.AppVersion;
        public string deviceId = FDeviceInfo.DeviceId;
        public string gameId = FDeviceInfo.GameId;
        public string gameName = FDeviceInfo.GameName;
        public string installDay = FTime.DateToString(RetentionCounter.FirstLoginDate);
        public int level = PlayerParams.MaxPassedLevel;
        public string platform = FDeviceInfo.Platform;
        public int retentionDay = RetentionCounter.Retention;
        public string sdkVersion = "2.2.0";
        public int apiId;

        protected DwhMessage()
        {
            abTestingValue = TrimIfTooLong(PlayerParams.AbTestingValue, 50);
            abTestingVariable = TrimIfTooLong(PlayerParams.AbTestingVariable, 100);
            accountId = TrimIfTooLong(PlayerParams.AccountID, 50);
            apiId = GetApiId();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetApiId()
        {
            var appIdKey = GetType().Name + "app_id";
            int result = FData.Instance.GetOrSet(appIdKey, 0);
            FData.Instance.Save(appIdKey, result + 1);
            return result;
        }
        
        protected abstract string GetAPI();

        public virtual DataWrapper Wrap()
        {
            AnalyticLogger.Instance.Info(GetType().Name + " message created : " + JsonConvert.SerializeObject(this));
            return new DataWrapper(this, GetAPI());
        }

        #region Check Params

        protected void CheckStringNotEmpty(string str, string fieldName, int maxLength)
        {
            if (str == null || str.Length > maxLength || str.Length < 1)
                throw new DwhMessageException(
                    string.Format(
                        "Dwh Log invalid field : the string length of field {0} of {1} must be between 1 and {2} character, input value '{3}' has the length of {4}",
                        fieldName, GetType().Name.Substring(3), maxLength, str, str?.Length ?? 0));
        }

        protected void CheckStringNullable(string str, string fieldName, int maxLength)
        {
            if (!String.IsNullOrEmpty(str)) CheckStringNotEmpty(str, fieldName, maxLength);
        }

        protected string TrimIfTooLong(string str, int maxLength)
        {
            if (str != null && str.Length > maxLength) return str.Substring(0, maxLength);
            return str;
        }

        protected void CheckIntNonNegative(int i, string fieldName)
        {
            CheckLongNonNegative(i, fieldName);
        }

        protected void CheckInt(int i, string fieldName, int minVal = 0, int maxVal = int.MaxValue)
        {
            if (i > maxVal || i < minVal)
                throw new DwhMessageException(
                    string.Format(
                        "Dwh Log invalid field : the value of field {0} of {1} must be between {2} and {3} character, input value '{4}'",
                        fieldName, GetType().Name.Substring(3), minVal, maxVal, i));
        }

        protected void CheckLongNonNegative(long i, string fieldName)
        {
            if (i < 0)
                throw new DwhMessageException(
                    string.Format(
                        "Dwh Log invalid field : the value of field {0} of {1} must be non-negative, input value '{2}'",
                        fieldName, GetType().Name.Substring(3), i));
        }

        #endregion
    }
}
