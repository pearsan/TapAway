using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    [SerializeField] private GameObject cubeGenerator;
    private CameraBehaviour cameraBehaviour;
    private int _currentStage = 0;
    private Transform _currentPuzzel;
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
        if (_currentPuzzel != null)
        {
            Destroy(_currentPuzzel.gameObject);
        }
        GameObject level = GameObject.Instantiate(cubeGenerator);
        level.transform.position = Vector3.zero;
        _currentPuzzel = level.transform;
        level.GetComponent<CubeGenerator>().SetLevel(jsonFile[_currentStage]);
        level.GetComponent<CubeGenerator>().LoadJson();
        cameraBehaviour.SetTargert(_currentPuzzel);
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
        if (_currentPuzzel != null && _currentPuzzel.childCount == 0)
        {
            GameUIManager.Instance.OnTriggerEnterWinPanel();
        }
    }

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
}
