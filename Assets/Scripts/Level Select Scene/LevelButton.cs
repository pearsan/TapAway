using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour
{
    public void SelectLevelButton()
    {
        string levelSelected = EventSystem.current.currentSelectedGameObject.name.Substring(5);
        Debug.Log(levelSelected);
    }
}
