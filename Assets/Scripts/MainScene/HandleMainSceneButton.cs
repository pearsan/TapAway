using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleMainSceneButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeToGameplayScene()
    {
        SceneManager.LoadScene("GenerateScene");
    }

    public void ChangeToShopScene()
    {
        SceneManager.LoadScene("Shop Scene");
    }    
}
