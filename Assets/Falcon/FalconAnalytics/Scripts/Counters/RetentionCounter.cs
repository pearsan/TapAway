using System;
using System.Collections;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils.Data;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Counters
{
    public sealed class RetentionCounter : IFInit
    {
        private const string LatestLoginDateKey = "LATEST_DATE";

        [Preserve]
        public RetentionCounter()
        {
        }

        public static int Retention =>
            DateTime.Compare(DateTime.Now.Date, FirstLoginDate.Date) > 0
                ? (DateTime.Now.Date - FirstLoginDate.Date).Days
                : 0;

        public static bool RetentionChanged =>
            DateTime.Compare(DateTime.Now.Date,
                FData.Instance.GetOrDefault(LatestLoginDateKey, FirstLoginDate).Date) > 0;

        public static DateTime FirstLoginDate => PlayerParams.FirstLoginDate;

        public IEnumerator Init()
        {
            Refresh();
            FData.Instance.Save(LatestLoginDateKey, DateTime.Now);

            yield return null;
        }

        public static void Refresh()
        {
            FData.Instance.Compute<DateTime>(LatestLoginDateKey, latestLogin =>
            {
                if (RetentionChanged)
                    new WaitInit(() => MessageSender.Instance.Enqueue(new DwhRetentionLog(Retention, FirstLoginDate)))
                        .Schedule();
            }, () =>
            {
                FData.Instance.Save(LatestLoginDateKey, FirstLoginDate);
                new WaitInit(() => MessageSender.Instance.Enqueue(new DwhRetentionLog(Retention, FirstLoginDate)))
                    .Schedule();
            });
        }
    }
}