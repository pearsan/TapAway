using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhPropertyLog : DwhMessage
    {
        public string pName;
        public string pValue;
        public int priority;

        [Preserve]
        [JsonConstructor]
        public DwhPropertyLog(string pName, string pValue, int priority)
        {
            CheckIntNonNegative(priority, nameof(priority));
            CheckStringNotEmpty(pName, nameof(pName),50);
            CheckStringNotEmpty(pValue, nameof(pValue),50);
            
            this.pName = pName;
            this.pValue = pValue;
            this.priority = priority;
        }

        protected override string GetAPI()
        {
            return DwhConstants.PropertyApi;
        }
    }
}