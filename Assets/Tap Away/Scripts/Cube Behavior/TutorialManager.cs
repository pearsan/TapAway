using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject[] tutorial2;
    [SerializeField] private List<GameObject> tutorial3;
    public List<GameObject> _current;
    private int index = 0;
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
    }
    public void SetTutorial(int stage)
    {
        index = 0;

        switch (stage)
        {
            case 0:
                tutorial1.SetActive(true);
                break;
            case 1:
                if (GameplayManager.Instance.CubesLeft() == 1)
                {
                    tutorial2[1].SetActive(true);
                }
                else
                {
                    tutorial2[0].SetActive(true);
                }
                break;
            case 2:
                _current.Clear();
                _current = new List<GameObject>();
                for (int i = 0; i < GameplayManager.Instance.CurrentPuzzle().childCount; i++)
                {
                    Vector3 position = tutorial3[i].transform.position;
                    Quaternion rotation = tutorial3[i].transform.rotation;

                    GameObject pointer = Instantiate(tutorial3[i], position,
                        rotation, GameplayManager.Instance.CurrentPuzzle().GetChild(i));
                    pointer.transform.localScale = tutorial3[i].transform.localScale * 0.01f;
                    _current.Add(pointer);
                }
                _current[0].SetActive(true);
                break;
        }
    }

    public void ChangeStep(int stage)
    {
        switch (stage)
        {
            case 0:
                tutorial1.SetActive(false);
                index = 0;
                break;
            case 1:
                
                tutorial2[index].SetActive(false);
                if (GameplayManager.Instance.CubesLeft() == 0)
                {
                }
                else
                {
                    index++;
                    tutorial2[index].SetActive(true);
                }
                break;
            case 2:
                _current[index].SetActive(false);
                if (GameplayManager.Instance.CubesLeft() == 0)
                {
                }
                else
                {
                    index++;
                    _current[index].SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public void DisableTutorial(int stage)
    {
        switch (stage)
        {
            case 0:
                tutorial1.SetActive(false);
                break;
            case 1:
                foreach (var gameObject in tutorial2)
                {
                    gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (var gameObject in tutorial3)
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
