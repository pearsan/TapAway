using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhResourceLog : DwhMessage
    {
        public long amount;
        public string currency;
        public string flowType;
        public string itemId;
        public string itemType;

        public DwhResourceLog(FlowType flowType, string itemType, string itemId, string currency, long amount)
        {
            CheckLongNonNegative(amount, nameof(amount));
            CheckStringNotEmpty(itemType, nameof(itemType), 50);
            CheckStringNotEmpty(itemId, nameof(itemId), 50);
            CheckStringNotEmpty(currency, nameof(currency), 50);

            this.flowType = flowType.ToString();
            this.itemType = itemType;
            this.itemId = itemId;
            this.currency = currency;
            this.amount = amount;
        }

        [Preserve]
        [JsonConstructor]
        public DwhResourceLog(string flowType, string itemType, string itemId, string currency, long amount) : this(
            (FlowType)System.Enum.Parse(typeof(FlowType), flowType), itemType, itemId, currency, amount)
        {
        }

        protected override string GetAPI()
        {
            return DwhConstants.ResourceApi;
        }
    }
}