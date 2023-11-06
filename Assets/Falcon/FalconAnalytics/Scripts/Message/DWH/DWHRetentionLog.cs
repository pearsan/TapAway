using System;
using Falcon.FalconCore.Scripts.Utils;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhRetentionLog : DwhMessage
    {
        public int day;
        public string localDate;
        
        public DwhRetentionLog(int day, DateTime localDate)
        {
            CheckIntNonNegative(day, nameof(day));
            this.day = day;
            this.localDate = FTime.DateToString(localDate);
        }

        [Preserve]
        [JsonConstructor]
        public DwhRetentionLog(int day, string localDate) : this (day, FTime.StringToDate(localDate))
        {
        }

        protected override string GetAPI()
        {
            return DwhConstants.RetentionApi;
        }
    }
}