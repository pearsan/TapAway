using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils
{
     
    /// <summary>
    /// Saves, loads and deletes all data in the game
    /// </summary>
    public class DataSaveLoad
    {
        static DataSaveLoad ()
        {
            PersistentDataPath = Application.persistentDataPath;
            StreamingAssetsPath = Application.streamingAssetsPath;
        }
        public static string PersistentDataPath { get; private set; }
        public static string StreamingAssetsPath { get; private set; }
        
        /// <summary>
        /// Save data to a file (overwrite completely)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        public static void Save(object data, string folder, string file)
        {
            // get the data path of this save data
            string dataPath = GetFilePath(folder, file);

            string jsonData = JsonConvert.SerializeObject(data);
            var byteData = Encoding.ASCII.GetBytes(jsonData);
            // attempt to save here data
            try
            {
                // create the file in the path if it doesn't exist
                // if the file path or name does not exist, return the default SO
                if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
                {
                    var path = Path.GetDirectoryName(dataPath);
                    if (path == null)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                    }

                }
                
                // save data here
                File.WriteAllBytes(dataPath, byteData);
                FalconLogUtils.Info("Save data to: " + dataPath, "#ecbd77");
            }
            catch (System.Exception e)
            {
                // write out error here
                FalconLogUtils.Warning("Failed to save data to: " + dataPath, "#ecbd77");
                FalconLogUtils.Error(e, "#ecbd77");
            }
        }
        
        /// <summary>
        /// Load all data at a specified file and folder location
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static T Load<T>(string folder, string file)
        {
            // get the data path of this save data
            string dataPath = GetFilePath(folder, file);

            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                FalconLogUtils.Warning("File or path does not exist! " + dataPath, "#ecbd77");
                return default(T);
            }

            // load in the save data as byte array

            try
            {
                var jsonDataAsBytes = File.ReadAllBytes(dataPath);
                FalconLogUtils.Info("Loading all data from: " + dataPath, "#ecbd77");
                
                // convert the byte array to json

                var jsonData =
                    // convert the byte array to json
                    Encoding.ASCII.GetString(jsonDataAsBytes);

                // convert to the specified object type
                T returnedData = JsonConvert.DeserializeObject<T>(jsonData);

                // return the casted json object to use
                return (T)Convert.ChangeType(returnedData, typeof(T));
            }
            catch (System.Exception e)
            {
                FalconLogUtils.Warning("Failed to load data from: " + dataPath, "#ecbd77");
                FalconLogUtils.Warning(e, "#ecbd77");
                return default(T);
            }
        }
        
        /// <summary>
        /// Create file path for where a file is stored on the specific platform given a folder name and file name
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFilePath(string folderName, string fileName = "")
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // mac
            var filePath = Path.Combine(StreamingAssetsPath, ("data/" + folderName));

            if (fileName != "")
                filePath = Path.Combine(filePath, (fileName + ".txt"));
    #elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            // windows
            var filePath = Path.Combine(PersistentDataPath, ("data/" + folderName));

            if(fileName != "")
                filePath = Path.Combine(filePath, (fileName + ".txt"));
    #elif UNITY_ANDROID
            // android
            var filePath = Path.Combine(PersistentDataPath, ("data/" + folderName));

            if(fileName != "")
                filePath = Path.Combine(filePath, (fileName + ".txt"));
    #elif UNITY_IOS
            // ios
            var filePath = Path.Combine(PersistentDataPath, ("data/" + folderName));

            if(fileName != "")
                filePath = Path.Combine(filePath, (fileName + ".txt"));
    #endif
            return filePath;
        }
    }   
}