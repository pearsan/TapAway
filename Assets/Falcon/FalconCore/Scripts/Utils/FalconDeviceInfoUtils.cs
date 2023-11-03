// using System;
// using System.Globalization;
// using Falcon;
// using UnityEngine;
// using Newtonsoft.Json;
// #if UNITY_IOS
// using UnityEngine.iOS;
// #endif
//
// namespace Falcon.FalconCore.Scripts.Utils
// {
//     public static class FalconDeviceInfoUtils
//     {
//         public static string GetDeviceId()
//         {
//
// #if UNITY_EDITOR || UNITY_ANDROID
//             return SystemInfo.deviceUniqueIdentifier;
// #elif UNITY_IOS
//         string key = "falcon_" + Application.identifier + "_uuid";
//         string uuid = UUIDiOS.GetKeyChainValue(key);
//
//         if (!string.IsNullOrEmpty(uuid))
//         {
//             return uuid;
//         }
//         else
//         {
//             uuid = Guid.NewGuid().ToString();
//             UUIDiOS.SaveKeyChainValue(key, uuid);
//             return uuid;
//         }
// #endif
//         }
//
//         // public static String DeviceIP { get; private set; }
//         // public static String DeviceCountryFullName{ get; private set; }
//         // public static String DeviceCountryIso { get; private set; }
//         //
//         // public void Init()
//         // {
//         //     try
//         //     {
//         //         DeviceIP = FalconNetUtils.DoRequest(HttpMethod.Get, "https://api.ipify.org");
//         //
//         //         String jsonIpInfo = FalconNetUtils.DoRequest(HttpMethod.Get, "https://ipinfo.io/" + DeviceIP);
//         //
//         //         IpInfo ipInfo = JsonConvert.DeserializeObject<IpInfo>(jsonIpInfo);
//         //         RegionInfo myRi1 = new RegionInfo(ipInfo.Country);
//         //         ipInfo.Country = myRi1.EnglishName;
//         //         DeviceCountryFullName = ipInfo.Country;
//         //         var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
//         //             .Select(x => new RegionInfo(x.LCID));
//         //         if (DeviceCountryFullName != null)
//         //         {
//         //             var englishRegion =
//         //                 regions.FirstOrDefault(region => region.EnglishName.Contains(DeviceCountryFullName));
//         //             if (englishRegion != null)
//         //             {
//         //                 DeviceCountryIso = englishRegion.TwoLetterISORegionName.ToLower();
//         //             }
//         //         }
//         //     }
//         //     catch (System.Exception e)
//         //     {
//         //         DeviceIP = DeviceIP == null? "unknown" : DeviceIP;
//         //         DeviceCountryFullName = "unknown";
//         //         DeviceCountryIso = "un";
//         //         Debug.LogWarning(e);
//         //     }
//         // }
//         //
//         // public int GetPriority()
//         // {
//         //     return -1;
//         // }
//         // public void UseDeviceIp(Action<String> action)
//         // {
//         //     if(DeviceIP != null) {action.Invoke(DeviceIP);}
//         //     else
//         //     {
//         //         while (DeviceIP == null)
//         //         {
//         //             Thread.Sleep(100);
//         //         }
//         //         
//         //         FalconGameObjectUtils.Trigger(() => action.Invoke(DeviceIP), Debug.Log);
//         //     }
//         // }
//         //
//         // public void UseDeviceCountryName(Action<String> action)
//         // {
//         //     if(DeviceCountryFullName != null) {action.Invoke(DeviceCountryFullName);}
//         //     else
//         //     {
//         //         while (DeviceCountryFullName == null)
//         //         {
//         //             Thread.Sleep(100);
//         //         }
//         //         
//         //         FalconGameObjectUtils.Trigger(() => action.Invoke(DeviceCountryFullName), Debug.Log);
//         //     }
//         // }
//         //
//         // public void UseDeviceCountryIso(Action<String> action)
//         // {
//         //     if(DeviceCountryIso != null) {action.Invoke(DeviceCountryIso);}
//         //     else
//         //     {
//         //         while (DeviceCountryIso == null)
//         //         {
//         //             Thread.Sleep(100);
//         //         }
//         //         
//         //         FalconGameObjectUtils.Trigger(() => action.Invoke(DeviceCountryIso), Debug.Log);
//         //     }
//         // }
//
//         // class IpInfo
//         // {
//         //     [JsonProperty("ip")] public string Ip { get; set; }
//         //
//         //     [JsonProperty("hostname")] public string Hostname { get; set; }
//         //
//         //     [JsonProperty("city")] public string City { get; set; }
//         //
//         //     [JsonProperty("region")] public string Region { get; set; }
//         //
//         //     [JsonProperty("country")] public string Country { get; set; }
//         //
//         //     [JsonProperty("loc")] public string Loc { get; set; }
//         //
//         //     [JsonProperty("org")] public string Org { get; set; }
//         //
//         //     [JsonProperty("postal")] public string Postal { get; set; }
//         // }
//     }
// }
