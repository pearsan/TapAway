using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Falcon.FalconCore.Scripts.Utils.Singletons;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.Utils.Data
{
    public class FData : Singleton<FData>, IFEssential
    {
        public static readonly string DataFile = Path.Combine("Sdk", "Data");
        
        private readonly FFile file = new FFile(DataFile);

        private readonly ConcurrentDictionary<String, String> cache;
        
        [Preserve]
        public FData()
        {
            Dictionary<String, String> fileData = file.Load<Dictionary<String, String>>() ?? new Dictionary<string, string>();
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
            catch (Exception e)
            {
                CoreLogger.Instance.Warning(e);
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
            catch (Exception e)
            {
                CoreLogger.Instance.Warning(e);
                Save(key, valueIfNotExist);
                return valueIfNotExist;
            }
        }

        public bool HasKey(String key)
        {

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
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
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
                    catch (Exception e)
                    {
                        CoreLogger.Instance.Warning(e);
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
            file.Save(Instance.cache);
            CoreLogger.Instance.Info("Save Data finished");
        }

        public void OnPreContinue()
        {
        }

        public void OnPostStop()
        {
            Instance.SaveData();
        }
    }
}