using System;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhFunnelLog : DwhMessage
    {
        public string action;
        public string funnelName;
        public int priority;
        public string funnelDay;

        [Preserve]
        [JsonConstructor]
        public DwhFunnelLog(string funnelName, string action, int priority)
        {
            CheckStringNotEmpty(funnelName, nameof(funnelName), 200);
            CheckStringNotEmpty(action, nameof(action), 200);
            CheckIntNonNegative(priority, nameof(priority));
            
            if (priority != 0 && !FData.Instance.HasKey(funnelName + (priority - 1)))
            {
                throw new DwhMessageException("Dwh Log invalid logic : Funnel not created in order in this device instance");
            }
            
            if (FData.Instance.HasKey(funnelName + priority))
            {
                throw new DwhMessageException("Dwh Log invalid logic : This device already joined the funnel");
            }

            FData.Instance.Save(funnelName + priority, DateTime.Now.ToUniversalTime());

            this.action = action;
            this.funnelName = funnelName;
            this.priority = priority;

            DateTime day = FData.Instance.GetOrSet(funnelName + 0, DateTime.Today.ToUniversalTime()).ToLocalTime();
            funnelDay = FTime.DateToString(day);
        }

        protected override string GetAPI()
        {
            return DwhConstants.FunnelApi;
        }
    }
}