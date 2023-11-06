using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhAdLog : DwhMessage
    {
        public string type;

        public string adWhere;

        public string adPrecision;
        public string adCountry;
        public double adRev;
        public string adNetwork;
        public string adMediation;
        
        public DwhAdLog(AdType type, string adWhere)
        {
            CheckStringNotEmpty(adWhere, nameof(adWhere), 50);

            if (type.Equals(AdType.Interstitial))
            {
                PlayerParams.InterstitialAdCount++;
            }
            else
            {
                PlayerParams.RewardAdCount++;
            }
            
            this.type = type.ToString();
            this.adWhere = adWhere;
        }

        [Preserve]
        [JsonConstructor]
        public DwhAdLog(AdType type, string adWhere, string adPrecision, string adCountry, double adRev, string adNetwork, string adMediation)
        {
            CheckStringNotEmpty(adWhere, nameof(adWhere), 50);
            CheckStringNullable(adCountry, nameof(adCountry), 2);
            CheckStringNotEmpty(adPrecision, nameof(adPrecision), 20);
            CheckStringNotEmpty(adCountry, nameof(adCountry), 20);
            CheckStringNotEmpty(adNetwork, nameof(adNetwork), 20);
            CheckStringNotEmpty(adMediation, nameof(adMediation), 20);
            if (adRev < 0)
                throw new DwhMessageException(
                    string.Format(
                        "Dwh Log invalid field : the value of field {0} of {1} must be non-negative, input value '{2}'",
                        nameof(adRev), GetType().Name.Substring(3), adRev));
            
            this.type = type.ToString();
            this.adWhere = adWhere;
            this.adPrecision = adPrecision;
            this.adCountry = adCountry;
            this.adRev = adRev;
            this.adNetwork = adNetwork;
            this.adMediation = adMediation;
        }

        protected override string GetAPI()
        {
            return DwhConstants.AdApi;
        }

    }
}

