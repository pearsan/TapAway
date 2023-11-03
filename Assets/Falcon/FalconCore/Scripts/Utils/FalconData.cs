using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.Utils
{
    public class FalconData : IFalconInit
    {

        public const string DataFolder = "Sdk";
        public const string DataFile = "Data";

        private static FalconData _instance;

        public static FalconData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FalconData();
                }

                return _instance;
            }
        }

        private readonly ConcurrentDictionary<String, String> cache;
        
        [Preserve]
        public FalconData()
        {
            Dictionary<String, String> fileData;
            try
            {
                fileData =
                    DataSaveLoad.Load<Dictionary<String, String>>(DataFolder, DataFile);
            }
            catch (System.Exception e)
            {
                fileData = new Dictionary<string, string>();
                FalconLogUtils.Warning(e.ToString(), "#ecbd77");
            }
            if (fileData == null)
            {
                fileData = new Dictionary<string, string>();
            }
            cache = new ConcurrentDictionary<string, string>(fileData);
        }

        public T GetOrDefault<T>(String key, T defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(cache[key]);
                return obj;
            }
            catch (System.Exception e)
            {
                FalconLogUtils.Warning(e, "#ecbd77");
                Delete(key);
                return defaultValue;
            }
        }

        public T GetOrSet<T>(String key, T valueIfNotExist)
        {
            if (!HasKey(key))
            {
                Save(key, valueIfNotExist);
                return valueIfNotExist;
            }

            try
            {
                T obj = JsonConvert.DeserializeObject<T>(cache[key]);
                return obj;
            }
            catch (System.Exception e)
            {
                FalconLogUtils.Warning(e, "#ecbd77");
                Save(key, valueIfNotExist);
                return valueIfNotExist;
            }
        }

        public bool HasKey(String key)
        {
            // if (!cache.ContainsKey(key))
            // {
            //     FalconGameObjectUtils.Instance.TriggerSync(() =>
            //     {
            //         String data = PlayerPrefs.GetString(FalconPropertiesPrefix + key);
            //         if (data == "") data = null;
            //         cache[key] = data;
            //     });
            // }

            if (!cache.ContainsKey(key) || cache[key] == null || cache[key].Trim() == "")
                return false;
            return true;
        }

        public void ComputeIfPresent<T>(String key, Action<T> computation)
        {
            if (HasKey(key) && computation != null)
            {
                T obj;
                try
                {
                    obj = JsonConvert.DeserializeObject<T>(cache[key]);
                }
                catch (System.Exception e)
                {
                    FalconLogUtils.Warning(e, "#ecbd77");
                    Delete(key);
                    return;
                }

                computation.Invoke(obj);
            }
        }

        public void ComputeIfAbsent(String key, Action computation)
        {
            if (!HasKey(key) && computation != null) computation.Invoke();
        }

        public void Compute<T>(String key, Action<T> ifExist, Action ifAbsent)
        {
            if (HasKey(key))
            {
                if (ifExist != null)
                {
                    T obj;
                    try
                    {
                        obj = JsonConvert.DeserializeObject<T>(cache[key]);
                    }
                    catch (System.Exception e)
                    {
                        FalconLogUtils.Warning(e, "#ecbd77");
                        Delete(key);
                        if (ifAbsent != null) ifAbsent.Invoke();

                        return;
                    }

                    ifExist.Invoke(obj);
                }
            }
            else
            {
                if (ifAbsent != null) ifAbsent.Invoke();
            }
        }

        public void Save<T>(String key, T value)
        {
            String json = JsonConvert.SerializeObject(value);
            cache[key] = json;
        }

        public void Delete(String key)
        {
            cache[key] = null;
        }

        private void SaveData()
        {
            new Thread(() => {
                DataSaveLoad.Save(Instance.cache, DataFolder, DataFile);
                FalconLogUtils.Info("Save Data finished", "#ecbd77");
            }).Start();
            
        }

        public void Init()
        {
            if (Instance == null)
            {
                _instance = Instance;
            }
            FalconGameObjectUtils.Instance.OnGameStop(99, Instance.SaveData);
        }

        public int GetPriority()
        {
            return 0;
        }

        public bool InitInMainThread()
        {
            return false;
        }
    }
}