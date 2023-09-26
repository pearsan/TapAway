using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectLevelController : MonoBehaviour
{
    public static SelectLevelController ins;

    public GameObject pagePrefab;
    public GameObject levelPrefab;

    public int numberOfLevelGenerate;
    public int numberOfLevelEachPage;

    [Header("ButtonController")]
    public GameObject leftButton;
    public GameObject rightButton;

    private const int LEVEL_EACH_COLUMN = 8;
    private const string TEXT_FORMAT = "Level ";

    private int page = 1;
    private int totalPage;
    private List<GameObject> pages;
    private List<GameObject> levels;


    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        CreateLevelBoard();

        leftButton.transform.SetAsLastSibling();
        rightButton.transform.SetAsLastSibling();

        page = 1;
    }

    private void Update()
    {
        SettingButtonAndBoardDependOnPageNumber();
    }

    private void CreateLevelBoard()
    {
        totalPage = numberOfLevelGenerate / numberOfLevelEachPage;
        if (totalPage * numberOfLevelEachPage < numberOfLevelGenerate)
            totalPage++;

        pages = new List<GameObject>();
        levels = new List<GameObject>();
        for (int i = 1; i <= totalPage; i++)
        {
            GameObject insPage = Instantiate<GameObject>(pagePrefab, transform);
            pages.Add(insPage);
            insPage.name = "Page " + i;
            insPage.SetActive(false);
            insPage.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            insPage.GetComponent<GridLayoutGroup>().constraintCount = LEVEL_EACH_COLUMN;
        }

        foreach (GameObject a in pages)
        {
            if (Convert.ToInt64(a.name.Substring(5)) < totalPage)
            {
                for (int i = (page - 1) * numberOfLevelEachPage + 1; i <= page * numberOfLevelEachPage; i++)
                {
                    GameObject insLevel = Instantiate<GameObject>(levelPrefab, a.transform);
                    insLevel.name = TEXT_FORMAT + i;

                    Text insText = insLevel.GetComponentInChildren<Text>();
                    insText.text = TEXT_FORMAT + i;
                }
                page++;
            }
            else
            {
                for (int i = (page - 1) * numberOfLevelEachPage + 1; i <= numberOfLevelGenerate; i++)
                {
                    GameObject insLevel = Instantiate<GameObject>(levelPrefab, a.transform);
                    insLevel.name = "Level " + i;
                    Text insText = insLevel.GetComponentInChildren<Text>();
                    insText.text = "Level " + i;
                }
            }
            levels.Add(a);
        }
    }    

    private void SettingButtonAndBoardDependOnPageNumber()
    { 
        if (page == 1)
        {
            foreach (GameObject a in pages)
                a.SetActive(false);
            pages[0].SetActive(true);

            rightButton.SetActive(true);
            leftButton.SetActive(false);
        }
        else if (page == totalPage)
        {
            foreach (GameObject a in pages)
                a.SetActive(false);
            pages[totalPage - 1].SetActive(true);

            rightButton.SetActive(false);
            leftButton.SetActive(true);
        }
        else
        {
            foreach (GameObject a in pages)
                a.SetActive(false);
            pages[page-1].SetActive(true);

            rightButton.SetActive(true);
            leftButton.SetActive(true);
        }
    }

    public void LeftButton()
    {
        page--;
    }   
    
    public void RightButton()
    {
        page++;
    }    
}
