using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    
    [SerializeField] private int moveAttemps = 10;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject cubeGenerator;
    [SerializeField] private TextAsset[] jsonFile;
    [SerializeField] private GameObject cubePrefabs;
    [SerializeField] private GameObject goldCube;
    [SerializeField] private Camera camera;
    
    [Range(0, 100)] [SerializeField] private float initialRewardRate;

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
            moveAttemps = loadedData.move;

            if (_currentStage > 2)
            {
                cameraBehaviour.SetEnable();
            }
            string loadedState = loadedData.state;
            

            switch (loadedState)
            {
                case PLAYING_STATE:
                    
                    if (loadedData.transforms.Count < 0 || _currentStage < 3)
                    {
                        HandlePlayButton();
                    }
                    else if (_currentStage >= 3)
                    {
                        _currentPuzzle.GetComponent<GameplayGenerater>().LoadCurrentLevel(_levelInProgress);
                        InitiateRewardCube();
                        cameraBehaviour.SetTargert(_currentPuzzle);  
                    }
                    if (Camera.main != null)
                    {
                        /*Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
                        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));*/
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
        StartCoroutine(GenerateLevel());
    }

    private IEnumerator GenerateLevel()
    {
        if (_currentPuzzle != null)
        {
            Destroy(_currentPuzzle.gameObject);
        }
        
        _gameState = PLAYING_STATE;
        GameObject level = GameObject.Instantiate(cubeGenerator);
        level.transform.position = Vector3.zero;
        _currentPuzzle = level.transform;

        _currentPuzzle.GetComponent<GameplayGenerater>().SetLevel(jsonFile[_currentStage]);
        if (_currentStage < 3)
        {
            _currentPuzzle.GetComponent<GameplayGenerater>().LoadCurrentLevel(jsonFile[_currentStage]);
        }
        else
        {
            if (!cameraBehaviour.CameraIsOn())
                cameraBehaviour.SetEnable();
            yield return StartCoroutine(_currentPuzzle.GetComponent<GameplayGenerater>().SetupLevel(cubePrefabs));
            InitiateRewardCube();
        }
        TutorialManager.Instance.SetTutorial(_currentStage);

        cameraBehaviour.SetTargert(_currentPuzzle);
        if (Camera.main != null)
        {
            /*Camera.main.transform.position = new Vector3(12.35f, 1, -12.33f);
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));*/
        }
    }

    private void InitiateRewardCube()
    {
        Transform parent = _currentPuzzle; // Assuming this script is attached to the parent
        int childCount = parent.childCount;
        int convertedChildCount = Mathf.CeilToInt(childCount * initialRewardRate * 0.01f); // Calculate 2%
        
        List<int> childIndices = new List<int>(); // List to store child indices
        for (int i = 0; i < childCount; i++)
        {
            childIndices.Add(i); // Populate the list with indices
        }
        Debug.Log(childCount);

        for (int i = 0; i < convertedChildCount; i++)
        {
            int randomIndex = Random.Range(0, childIndices.Count); // Get random index
            Transform child = parent.GetChild(childIndices[randomIndex]);
            GameObject newObject = Instantiate(goldCube, child.position, child.rotation, parent); // Create new GameObject1
            Destroy(child.gameObject); // Destroy original child
            childIndices.RemoveAt(randomIndex); // Remove selected index from list
        }
    }

    public void SpawnRewardCube()
    {
        int chance = Random.Range(0, 99);
        if (_currentPuzzle.childCount > 0 && _currentStage > 2)
            if (chance < initialRewardRate)
            {
                int randomIndex = Random.Range(0, _currentPuzzle.childCount);
                Transform child = _currentPuzzle.GetChild(randomIndex);
                GameObject newObject = Instantiate(goldCube, child.position, child.rotation, _currentPuzzle); // Create new GameObject1
                Destroy(child.gameObject); // Destroy original child
            }
    }

    #region DataHandle

    public void ChangeCurrentSkin(GameObject skin)
    {
        if (_currentPuzzle != null)
            foreach (Transform cube in _currentPuzzle.transform)
            {
                if (cube.GetComponent(typeof(TapCube)) != null)
                {
                    cube.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh =
                        skin.GetComponentInChildren<MeshFilter>().sharedMesh;
                    cube.GetComponentInChildren<MeshRenderer>().sharedMaterial =
                        skin.GetComponentInChildren<MeshRenderer>().sharedMaterial;                    
                }

            }
        cubePrefabs.transform.GetChild(0).gameObject.SetActive(true);
        cubePrefabs.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh =
            skin.GetComponentInChildren<MeshFilter>().sharedMesh;
        cubePrefabs.GetComponentInChildren<MeshRenderer>().sharedMaterial =
            skin.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        cubePrefabs.transform.GetChild(0).gameObject.SetActive(false);
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
        jsonData["move"] = moveAttemps;
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
        return moveAttemps;
    }

    public void MinusMoveAttemps()
    {
        moveAttemps--;
    }

    public void SetBonusMovesAttemps()
    {
        _gameState = PLAYING_STATE;
        var childCount = (float)JsonConvert.DeserializeObject<List<Vector3>>(jsonFile[_currentStage].text, new CubeGenerator.Vector3Converter()).Count;
        moveAttemps = Mathf.CeilToInt(childCount * 10 / 100);
    }
    
    public int GetBonusMovesAttemps()
    {
        var childCount = (float)JsonConvert.DeserializeObject<List<Vector3>>(jsonFile[_currentStage].text, new CubeGenerator.Vector3Converter()).Count;
        return Mathf.CeilToInt(childCount * 10 / 100);;
    }
    
    public void SetDefaultMoveAttemps()
    {
        Debug.Log("setted");
        var childCount = (float)_currentPuzzle.childCount;
        moveAttemps = Mathf.CeilToInt(childCount + childCount * 10 / 100);
    }

    #region GAME STATE
    
    public bool CheckIfLose()
    {
        if (moveAttemps == 0 && _currentPuzzle.childCount > 0 && _currentStage > 2)
        {
            _gameState = LOSE_STATE;
            ExportCurrentLevel();
            return true;
        }

        return false;
    }
    
    public bool CheckIfWin()
    {
        if (moveAttemps >= 0 && _currentPuzzle.childCount == 0 && _currentPuzzle != null)
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
        TutorialManager.Instance.DisableTutorial(_currentStage);
        ChangeBackGroundColor();
        StartCoroutine(GameUIManager.Instance.OnTriggerEnterWinPanel());
    }

    public void OnTriggerLose()
    {
        _gameState = LOSE_STATE;
        ChangeBackGroundColor();
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

    public void OnLoadNextStage()
    {
        _currentStage++;    
    }

    public void OnShowNextStage()
    {
        HandlePlayButton();
    }    
    #endregion

    #region Camera

    private void ChangeBackGroundColor()
    {
        float saturation = 0.53f;
        float value = 0.80f;
        camera.clearFlags = CameraClearFlags.SolidColor;

        float hue = Random.Range(0f, 1f);
        camera.backgroundColor = Color.HSVToRGB(hue, saturation, value);
    }
    
    public void Pause()
    {
        TutorialManager.Instance.DisableTutorial(_currentStage);
        cameraBehaviour.OnPause();
        OnPauseEvent.Invoke();
        /*
        cameraBehaviour.SetDisable();
    */
    }

    public void Resume()
    {
        TutorialManager.Instance.SetTutorial(_currentStage);
        cameraBehaviour.OnPlay();

        OnResumeEvent.Invoke();
    }

    public void EnableTarget()
    {
        _currentPuzzle.gameObject.SetActive(true);
    }

    public void DisableTarget()
    {
        TutorialManager.Instance.DisableTutorial(_currentStage);
        _currentPuzzle.gameObject.SetActive(false);
    }

    public int CubesLeft()
    {
        return _currentPuzzle.childCount;
    }

    public Transform CurrentPuzzle()
    {
        return _currentPuzzle.transform;
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
