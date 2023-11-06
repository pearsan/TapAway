using System;
using System.Linq;
using System.Net.Http;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.Wrapper
{
    public class DataWrapper
    {
        public readonly String MessageType;
        public readonly string URL;
        [JsonProperty(PropertyName = "data")] public string Data;

        private int failCount;

        [Preserve]
        [JsonConstructor]
        public DataWrapper(String messageType, string url, string data, int failCount)
        {
            MessageType = messageType;
            URL = url;
            Data = data;
            this.failCount = failCount;
        }

        [Preserve]
        public DataWrapper(DwhMessage message, string url)
        {
            MessageType = message.GetType().Name;
            Data = JsonConvert.SerializeObject(message);
            URL = url;
            failCount = 0;
        }


        public virtual void Send()
        {
            var response = new string(new HttpRequest
                        {
                            RequestType = HttpMethod.Post,
                            URL = URL,
                            JsonBody = JsonConvert.SerializeObject(this),
                            Timeout = TimeSpan.FromSeconds(60)
                        }.InvokeAndGet().Where(c => !char.IsControl(c)).ToArray());
            
                        if (string.Equals(response, "SS", StringComparison.Ordinal) ||
                            string.Equals(response, "SS\n", StringComparison.Ordinal))
                            AnalyticLogger.Instance.Info(MessageType.Substring(3) + " has been sent successfully");
                        else
                            AnalyticLogger.Instance.Warning(MessageType.Substring(3) +
                                                            " has been sent failed with the response of: " + response);
        }

        public void OnSendFail()
        {
            failCount++;
        }

        public bool CanRetry()
        {
            if (MessageType == nameof(DwhRetentionLog) || MessageType == nameof(DwhSessionLog)) return true;
            return failCount < 2;
        }
    }
}