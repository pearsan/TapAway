// using System;
// using System.Collections.Concurrent;
// using System.Threading;
// using Newtonsoft.Json;
// using UnityEngine;
//
// namespace Falcon.FalconCore.Scripts.Utils
// {
//     public class FalconPlayerPrefUtils : MonoBehaviour, IFalconInit
//     {
//         private const string FalconPropertiesPrefix = "Falcon_SDK_";
//
//         private static readonly ConcurrentDictionary<String, String> Cache = new ConcurrentDictionary<string, String>();
//
//         private static readonly ConcurrentQueue<DataRequest> RequestQueue = new ConcurrentQueue<DataRequest>();
//         private static FalconPlayerPrefUtils _instance;
//         
//         private void Awake()
//         {
//             try
//             {
//                 _instance = this;
//                 DontDestroyOnLoad(this);
//                 FalconGameObjectUtils.OnGameStop(99, SavePref);
//             }
//             catch (System.Exception e)
//             {
//                 FalconLogUtils.Error(e,"#ecbd77");
//             }
//         }
//
//         private void Update()
//         {
//             try
//             {
//                 if (!Application.isPlaying)
//                 {
//                     if (_instance != this) DestroyImmediate(this);
//                 }
//                 else
//                 {
//                     if(_instance != this) Destroy(this);
//                 }
//                 SynchronizePrefAndCache();
//             }
//             catch (System.Exception e)
//             {
//                 FalconLogUtils.Error(e,"#ecbd77");
//             }
//         }
//
//         public static T GetOrDefault<T>(String key, T defaultValue)
//         {
//             if (!HasKey(key))
//             {
//                 return defaultValue;
//             }
//             else
//             {
//                 try
//                 {
//                     T obj = JsonConvert.DeserializeObject<T>(Cache[key]);
//                     return obj;
//                 }
//                 catch (System.Exception e)
//                 {
//                     FalconLogUtils.Error(e,"#ecbd77");
//                     Delete(key);
//                     return defaultValue;
//                 }
//             }
//         }
//
//         public static T GetOrSet<T>(String key, T valueIfNotExist)
//         {
//             if (!HasKey(key))
//             {
//                 Save(key, valueIfNotExist);
//                 return valueIfNotExist;
//             }
//             else
//             {
//                 try
//                 {
//                     T obj = JsonConvert.DeserializeObject<T>(Cache[key]);
//                     return obj;
//                 }
//                 catch (System.Exception e)
//                 {
//                     FalconLogUtils.Error(e,"#ecbd77");
//                     Save(key, valueIfNotExist);
//                     return valueIfNotExist;
//                 }
//             }
//
//         }
//
//         public static bool HasKey(String key)
//         {
//             if (!Cache.ContainsKey(key))
//             {
//                 if (FalconGameObjectUtils.IsMainThread())
//                 {
//                     GetAtMainThread(key);
//             
//                 }
//                 else
//                 {
//                     bool done = false;
//
//                     DataRequest dataRequest = new DataRequest(key, null, Request.Get, () => done = true);
//                     RequestQueue.Enqueue(dataRequest);
//                     FalconFreezePreventor.Wait(() => done, "Falcon Player Pref loading");
//                 }
//             }
//             
//             if (!Cache.ContainsKey(key) || Cache[key]== null || Cache[key].Trim() == "")
//             {
//                 return false;
//             }
//             else
//             {
//                 return true;
//             }
//         }
//
//         public static void ComputeIfPresent<T>(String key, Action<T> computation)
//         {
//             if (HasKey(key) && computation != null)
//             {
//                 T obj;
//                 try
//                 {
//                     obj = JsonConvert.DeserializeObject<T>(Cache[key]);
//                 }
//                 catch (System.Exception e)
//                 {
//                     FalconLogUtils.Error(e,"#ecbd77");
//                     Delete(key);
//                     return;
//                 }
//                 computation.Invoke(obj);
//             }
//         }
//
//         public static void ComputeIfAbsent(String key, Action computation)
//         {
//             if (!HasKey(key) && computation != null)
//             {
//                 computation.Invoke();
//             }
//         }
//
//         public static void Compute<T>(String key, Action<T> ifExist, Action ifAbsent)
//         {
//             if (HasKey(key))
//             {
//                 if (ifExist != null)
//                 {
//                     T obj;
//                     try
//                     {
//                         obj = JsonConvert.DeserializeObject<T>(Cache[key]);
//                     }
//                     catch (System.Exception e)
//                     {
//                         FalconLogUtils.Error(e,"#ecbd77");
//                         Delete(key);
//                         if (ifAbsent != null)
//                         {
//                             ifAbsent.Invoke();
//                         }
//                         return;
//                     }
//                     ifExist.Invoke(obj);
//                 }
//             } else
//             {
//                 if (ifAbsent != null)
//                 {
//                     ifAbsent.Invoke();
//                 }
//             }
//         }
//
//         public static void Save<T>(String key, T value)
//         {
//             String json = JsonConvert.SerializeObject(value);
//             if (FalconGameObjectUtils.IsMainThread())
//             {
//                 SaveAtMainThread(key, json);
//             }
//             else
//             {
//                 bool done = false;
//
//                 DataRequest dataRequest = new DataRequest(key, json, Request.Save, () => done = true);
//                 RequestQueue.Enqueue(dataRequest);
//                 FalconFreezePreventor.Wait(() => done, "Falcon Player Pref saving");
//             }
//         }
//
//         public static void Delete(String key)
//         {
//             if (FalconGameObjectUtils.IsMainThread())
//             {
//                 DeleteAtMainThread(key);
//             }
//             else
//             {
//                 bool done = false;
//
//                 DataRequest dataRequest =new DataRequest(key, null, Request.Delete, () => done = true);
//                 RequestQueue.Enqueue(dataRequest);
//                 FalconFreezePreventor.Wait(() => done, "Falcon Player Pref deleting");
//             }
//         }
//
//         private void SavePref()
//         {
//             Thread.Sleep(100);
//             SynchronizePrefAndCache();
//             
//             PlayerPrefs.Save();
//         }
//
//         private static void SynchronizePrefAndCache()
//         {
//             DataRequest request;
//             while (RequestQueue.TryDequeue(out request))
//             {
//                 // Debug.Log("Falcon Player Pref processing data");
//
//                 switch (request.Request)
//                 {
//                     case Request.Delete:
//                         DeleteAtMainThread(request.Key);
//                         break;
//                     case Request.Get:
//                         GetAtMainThread(request.Key);
//                         break;
//                     case Request.Save:
//                         SaveAtMainThread(request.Key, request.Value);
//                         break;
//                 }
//                 if (request.CallBack != null)
//                 {
//                     request.CallBack.Invoke();
//                 }
//             }
//         }
//
//         private static void GetAtMainThread(String key)
//         {
//             String data = PlayerPrefs.GetString(FalconPropertiesPrefix+key);
//             if (data == "")
//             {
//                 data = null;
//             }
//
//             Cache[key] = data;
//         }
//
//         private static void DeleteAtMainThread(String key)
//         {
//             PlayerPrefs.DeleteKey(FalconPropertiesPrefix + key);
//             string ignore;
//             Cache.TryRemove(key, out ignore);
//         }
//
//         private static void SaveAtMainThread(String key, String value)
//         {
//             PlayerPrefs.SetString(FalconPropertiesPrefix + key, value);
//             Cache[key] = value;
//         }
//
//         private enum Request
//         {
//             Save, Get, Delete
//         }
//
//         private class DataRequest
//         {
//             public readonly Guid Id = Guid.NewGuid();
//             public String Key { get; private set;}
//             public String Value { get; private set;}
//             public Request Request { get; private set;}
//             public Action CallBack { get; private set;}
//             public DataRequest(String key, String value, Request request, Action onCallBack)
//             {
//                 Key = key;
//                 Value = value;
//                 Request = request;
//                 CallBack = onCallBack;
//             }
//
//             public DataRequest()
//             {
//
//             }
//
//             public override bool Equals(object obj)
//             {
//                 DataRequest fooItem = obj as DataRequest;
//
//                 if (fooItem == null) 
//                 {
//                     return false;
//                 }
//
//                 return fooItem.Id == Id;
//             }
//
//             public override int GetHashCode()
//             {
//                 return Id.GetHashCode();
//             }
//         }
//         
//         public void Init()
//         {
//             FalconGameObjectUtils.AddMonoBehaviourIfNotExist<FalconPlayerPrefUtils>();
//         }
//
//         public int GetPriority()
//         {
//             return - 3;
//         }
//     }
// }