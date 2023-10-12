using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    private CameraBehaviour cameraBehaviour;
    private int _currentStage = 0;
    private Transform _currentPuzzle;
    private TextAsset _levelInProgress;
    
    [SerializeField] private GameObject cubeGenerator;
    [SerializeField] private TextAsset[] jsonFile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        
        cameraBehaviour = gameObject.GetComponent<CameraBehaviour>();
    }

    public void HandlePlayButton()
    {
        if (_currentPuzzle != null)
        {
            Destroy(_currentPuzzle.gameObject);
        }
        GameObject level = GameObject.Instantiate(cubeGenerator);
        level.transform.position = Vector3.zero;
        _currentPuzzle = level.transform;
        _currentPuzzle.GetComponent<CubeGenerator>().SetLevel(jsonFile[_currentStage]);
        _currentPuzzle.GetComponent<CubeGenerator>().StartCoroutine(level.GetComponent<CubeGenerator>().SetupLevel());
        cameraBehaviour.SetTargert(_currentPuzzle);
        Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));

    }
    
    void Start()
    {
        string folderPath = "Assets/Tap Away/Resources/CurrentLevel";
        string jsonFilePath = Path.Combine(folderPath, "current.json");

        if (File.Exists(jsonFilePath))
        {
            
            GameObject level = GameObject.Instantiate(cubeGenerator);
            level.transform.position = Vector3.zero;
            _currentPuzzle = level.transform;
            string json = System.IO.File.ReadAllText(jsonFilePath);
            _levelInProgress = new TextAsset(json);
            LoadedData loadedData = JsonConvert.DeserializeObject<LoadedData>(_levelInProgress.text);
            _currentStage = loadedData.level;
            _currentPuzzle.GetComponent<CubeGenerator>().LoadCurrentLevel(_levelInProgress);
            
            cameraBehaviour.SetTargert(_currentPuzzle);
            
            Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));
        }
        else
        {

        }
    }

    private void Update()
    {
        if (_currentPuzzle != null && _currentPuzzle.childCount == 0)
        {
            _currentStage++;
            HandlePlayButton();
        }
    }

    public void ExportCurrentLevel()
    {
        if (_currentPuzzle == null || _currentPuzzle.childCount == 0)
            return;
        List<TransformData> transformDataList = new List<TransformData>();
        // Add the position and rotation of each game object to the list.
        foreach (Transform child in _currentPuzzle.transform)
        {
            TransformData transformData = new TransformData();
            // Setting the values
            transformData.position = new SerializableVector3(child.localPosition);
            transformData.rotation = new SerializableVector3(child.localRotation.eulerAngles);
            transformDataList.Add(transformData);
        }
        // Create a dictionary to hold the level and transform data
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        jsonData["level"] = _currentStage;
        jsonData["transforms"] = transformDataList;

        // Convert the list of transforms to JSON.
        string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
        
        string path = Path.Combine("Assets", "Tap Away", "Resources", "CurrentLevel","current" + ".json");
        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
    }
    
    [System.Serializable]
    public class TransformData
    {
        public SerializableVector3 position;
        public SerializableVector3 rotation;
    }

    
    public class LoadedData
    {
        public int level;
        public List<TransformData> transforms;
    }
    
    [Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        // Convert Vector3 to SerializableVector3
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        // Convert SerializableVector3 to Vector3
        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }
    void OnApplicationQuit()
    {
        ExportCurrentLevel();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ExportCurrentLevel();
        }
    }

}
