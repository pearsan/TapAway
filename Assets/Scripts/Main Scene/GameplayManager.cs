using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    private CameraBehaviour cameraBehaviour;
    private int _currentStage = 0;
    private Transform _currentPuzzle;
    private TextAsset _levelInProgress;
    private string _gameState;

    public const string WIN_STATE = "WIN";
    public const string LOSE_STATE = "LOSE";
    public const string PLAYING_STATE = "PLAYING";
    
    [SerializeField] private int _moveAttemps = 10;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject cubeGenerator;
    [SerializeField] private TextAsset[] jsonFile;
    [SerializeField] private GameObject tutorialCursor;

    [Header("Events")]
    [SerializeField] private UnityEvent OnResumeEvent;
    [SerializeField] private UnityEvent OnPauseEvent;

    private int LevelPassedEachPlaySection; //Each time player play, this counter will count how many level passed each section
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
    
    private void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "Tap Away", "Resources", "CurrentLevel", "current.json");
#if  UNITY_EDITOR
        path = Path.Combine("Assets", "Tap Away", "Resources", "CurrentLevel","current" + ".json");
#endif
        if (File.Exists(path))
        {
            _gameState = PLAYING_STATE;
            playButton.SetActive(false);
            
            GameObject level = GameObject.Instantiate(cubeGenerator);
            level.transform.position = Vector3.zero;
            _currentPuzzle = level.transform;
            string json = System.IO.File.ReadAllText(path);
            _levelInProgress = new TextAsset(json);
            LoadedData loadedData = JsonConvert.DeserializeObject<LoadedData>(_levelInProgress.text);
            _currentStage = loadedData.level;
            _moveAttemps = loadedData.move;
            string loadedState = loadedData.state;
            

            switch (loadedState)
            {
                case PLAYING_STATE:
                    tutorialCursor.SetActive(false);

                    if (_currentStage == 0)
                        tutorialCursor.SetActive(true);

                    _currentPuzzle.GetComponent<CubeGenerator>().LoadCurrentLevel(_levelInProgress);
                    cameraBehaviour.SetTargert(_currentPuzzle);

                    if (Camera.main != null)
                    {
                        Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
                        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));
                    }
                    break;
                case LOSE_STATE:
                    HandlePlayButton();
                    break;
                case WIN_STATE:
                    _currentStage++;
                    HandlePlayButton();
                    break;
            }
        }
    }

    public void HandlePlayButton()
    {
        if (_currentPuzzle != null)
        {
            Destroy(_currentPuzzle.gameObject);
        }
        Debug.Log(_currentStage);
        if (_currentStage == 0)
        {
            tutorialCursor.SetActive(true);
        }
        else
        {
            tutorialCursor.SetActive(false);
        }

        _gameState = PLAYING_STATE;
        GameObject level = GameObject.Instantiate(cubeGenerator);
        level.transform.position = Vector3.zero;
        _currentPuzzle = level.transform;
        _currentPuzzle.GetComponent<CubeGenerator>().SetLevel(jsonFile[_currentStage]);
        _currentPuzzle.GetComponent<CubeGenerator>().StartCoroutine(level.GetComponent<CubeGenerator>().SetupLevel());
        cameraBehaviour.SetTargert(_currentPuzzle);
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));
        }

        SetDefaultMoveAttemps();
    }

    #region DataHandle

    public void ChangeCurrentSkin(GameObject skin)
    {
        foreach (var cube in _currentPuzzle)
        {
            
        }
    }
    
     public void ExportCurrentLevel()
    {
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
        jsonData["state"] = _gameState;
        jsonData["move"] = _moveAttemps;
        jsonData["transforms"] = transformDataList;

        // Convert the list of transforms to JSON.
        string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

        string path = Path.Combine(Application.persistentDataPath, "Tap Away", "Resources", "CurrentLevel", "current.json");
#if  UNITY_EDITOR
        path = Path.Combine("Assets", "Tap Away", "Resources", "CurrentLevel","current" + ".json");
#endif
        if (!File.Exists(path))
        {
            string directoryPath = Path.GetDirectoryName(path);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create the file
            using (File.Create(path)) { }
        }
        File.WriteAllText(path, jsonString);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
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
    
    [System.Serializable]
    public class TransformData
    {
        public SerializableVector3 position;
        public SerializableVector3 rotation;
    }

    
    public class LoadedData
    {
        public int level;
        public string state;
        public int move;
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

    #endregion
    
    public int GetMoveAttemps()
    {
        return _moveAttemps;
    }

    public void MinusMoveAttemps()
    {
        _moveAttemps--;
        tutorialCursor.SetActive(false);
    }

    public void SetBonusMovesAttemps()
    {
        _gameState = PLAYING_STATE;
        var childCount = (float)JsonConvert.DeserializeObject<List<Vector3>>(jsonFile[_currentStage].text, new CubeGenerator.Vector3Converter()).Count;
        _moveAttemps = Mathf.CeilToInt(childCount * 10 / 100);
    }
    
    public int GetBonusMovesAttemps()
    {
        var childCount = (float)JsonConvert.DeserializeObject<List<Vector3>>(jsonFile[_currentStage].text, new CubeGenerator.Vector3Converter()).Count;
        return Mathf.CeilToInt(childCount * 10 / 100);;
    }
    
    private void SetDefaultMoveAttemps()
    {
        var childCount = (float)_currentPuzzle.childCount;
        _moveAttemps = Mathf.CeilToInt(childCount + childCount * 10 / 100);
    }

    #region GAME STATE
    
    public bool CheckIfLose()
    {
        if (_moveAttemps == 0 && _currentPuzzle.childCount > 0)
        {
            _gameState = LOSE_STATE;
            ExportCurrentLevel();
            return true;
        }

        return false;
    }
    
    public bool CheckIfWin()
    {
        if (_moveAttemps >= 0 && _currentPuzzle.childCount == 0 && _currentPuzzle != null)
        {
            _gameState = WIN_STATE;
            ExportCurrentLevel();
            return true;
        }

        return false;
    }

    public void OnTriggerWin()
    {
        _gameState = WIN_STATE;
        GameUIManager.Instance.OnTriggerEnterWinPanel();
    }

    public void OnTriggerLose()
    {
        _gameState = LOSE_STATE;
        GameUIManager.Instance.OnTriggerEnterLosePanel();
    }

    public string GetGameState()
    {
        return _gameState;
    }
    
    #endregion

    #region Stage behaviours
    public int GetCurrentStage()
    {
        return _currentStage;
    }

    public void OnTriggerNextStage()
    {
        _currentStage++;
        HandlePlayButton();
    }    
    #endregion

    #region Camera

    public void Pause()
    {
        OnPauseEvent.Invoke();
        cameraBehaviour.SetDisable();
    }

    public void Resume()
    {
        OnResumeEvent.Invoke();
        cameraBehaviour.SetEnable();
    }

    public void EnableTarget()
    {
        _currentPuzzle.gameObject.SetActive(true);
        cameraBehaviour.SetTargert(_currentPuzzle);
    }

    public void DisableTarget()
    {
        _currentPuzzle.gameObject.SetActive(false);
    }

    #endregion

    #region Ads behaviours
    public bool OnValidateTriggerIntersitialAdsEvent()
    {
        LevelPassedEachPlaySection++;
        if (LevelPassedEachPlaySection % 2 == 0)
            return true;
        else
            return false;
    }    
    #endregion
}
