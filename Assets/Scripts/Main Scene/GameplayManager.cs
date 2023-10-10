using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    [SerializeField] private GameObject cubeGenerator;
    private CameraBehaviour cameraBehaviour;
    
    private Transform _currentPuzzel;
    
    private void Awake()
    {
        Instance = this;
        cameraBehaviour = gameObject.GetComponent<CameraBehaviour>();
    }

    public void HandlePlayButton()
    {
        GameObject level = GameObject.Instantiate(cubeGenerator);
        level.transform.position = Vector3.zero;
        _currentPuzzel = level.transform;
        cameraBehaviour.SetTargert(_currentPuzzel);
    }
}
