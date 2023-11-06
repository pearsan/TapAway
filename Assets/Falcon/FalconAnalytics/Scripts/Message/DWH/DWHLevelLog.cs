using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhLevelLog : DwhMessage
    {

        public new int level;
        /// <summary>
        /// in second
        /// </summary>
        public int duration;
        public int wave;
        public string difficulty;
        public string status;
        
        [Preserve]
        [JsonConstructor]
        public DwhLevelLog(int level, int duration, int wave, string difficulty, LevelStatus status)
        {
            CheckIntNonNegative(level, nameof(level));
            CheckStringNotEmpty(difficulty, nameof(difficulty), 20);
            CheckIntNonNegative(wave, nameof(wave));
            CheckIntNonNegative(duration, nameof(duration));

            var levelKey = "HasPassedLevel" + level;
            if (FData.Instance.HasKey(levelKey))
            {
                throw new DwhMessageException("Falcon Log detect error : Already passed the level before");
            }

            if (LevelStatus.Pass.Equals(status))
            {
                FData.Instance.Save("HasPassedLevel" + level, FTime.CurrentTimeSec());
                PlayerParams.MaxPassedLevel = level;
            }
            
            this.level = level;
            this.duration = duration;
            this.wave = wave;
            this.difficulty = difficulty;
            this.status = status.ToString();
        }
        
        
        protected override string GetAPI()
        {
            return DwhConstants.LevelApi;
        }
        

    }
}

