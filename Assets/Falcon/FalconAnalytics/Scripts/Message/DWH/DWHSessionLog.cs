using System;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhSessionLog : DwhMessage
    {
        public string gameMode;
        public int sessionId;
        public int sessionTime;

        public DwhSessionLog(int sessionTime, string gameMode)
        {
            CheckInt(sessionTime, nameof(sessionTime), 5, 172800);


            this.sessionTime = sessionTime;
            this.gameMode = gameMode;
            sessionId = PlayerParams.SessionId;
        }

        [Preserve]
        [JsonConstructor]
        public DwhSessionLog(string gameMode, int sessionId, int sessionTime) : this(sessionId, gameMode)
        {
            this.sessionId = sessionId;
        }

        protected override string GetAPI()
        {
            return DwhConstants.SessionApi;
        }
    }
}