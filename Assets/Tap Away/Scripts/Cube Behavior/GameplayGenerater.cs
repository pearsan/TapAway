using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayGenerater : CubeGenerator
{
 
    public delegate void PuzzleGeneratedHandler();
    public event PuzzleGeneratedHandler OnPuzzleGenerated;

    #region Setup Level


    public IEnumerator SetupLevel(GameObject tapCube)
    {
        yield return StartCoroutine(LoadJson(tapCube, Autoplay));


        transform.position = new Vector3(0, 0, 0);
        
        // Get the total bounds of all the children objects
        Bounds bounds = CalculateTotalBounds();

        // Calculate the center point
        Vector3 centerPoint = bounds.center;

        // Calculate the offset
        Vector3 offset = transform.position - centerPoint;

        // Move children objects to have the parent in the center
        MoveChildrenToCenterPoint(offset);
        foreach (Transform child in transform)
        {
            MoveChild(child);
        }

        foreach (Transform child in transform)
        {
            MoveAnimChild(child);
        }
    }

    public IEnumerator LoadJson(GameObject tapCube, Action callback)
    {
        if (tapCube == null)
            tapCube = cubePrefabs;
        _cubes = new List<TapCube>();
        ClearCube();
        List<Vector3> positions = JsonConvert.DeserializeObject<List<Vector3>>(jsonFile.text, new Vector3Converter());
        int i = 0;
        foreach (Vector3 position in positions)
        {
            GameObject cube = Instantiate(tapCube);
            cube.name = "" + i;
            cube.transform.SetParent(transform);
            cube.transform.localRotation = RandomRotation();
            cube.transform.localPosition = position;
            i++;
            
            _cubes.Add(cube.gameObject.GetComponent<TapCube>());
        }
        yield return new WaitForSeconds(0.1f);
        callback?.Invoke();
    }
    
    public void LoadCurrentLevel(TextAsset _levelInProgress)
    {
        _cubes = new List<TapCube>();
        ClearCube();
        
        GameplayManager.LoadedData loadedData = JsonConvert.DeserializeObject<GameplayManager.LoadedData>(_levelInProgress.text);

        // Apply the transform for each object.
        for (int i = 0; i < loadedData.transforms.Count; i++)
        {
            GameObject cube = Instantiate(this.cubePrefabs);
            cube.transform.SetParent(transform);
            // Getting the values
            cube.transform.localPosition = loadedData.transforms[i].position.ToVector();
            cube.transform.localRotation = Quaternion.Euler(loadedData.transforms[i].rotation.ToVector());
            _cubes.Add(cube.GetComponent<TapCube>());
        }
        ShowCubes();
        foreach (Transform child in transform)
        {
            MoveChild(child);
        }

        foreach (Transform child in transform)
        {
            MoveAnimChild(child);
        }
    }
    
    private IEnumerator SolveGame()
    {
        yield return new WaitForSeconds(0.05f);
        Autoplay();
    }

    // Test and make the puzzle solvable
    // ReSharper disable Unity.PerformanceAnalysis
    private void Autoplay()
    {
        
        bool playable = AutoCheck();

        if (!playable)
        {
            Reshuffle();
            Autoplay();
        }
        else
        {
            ShowCubes();
        }
    }

    private bool AutoCheck()
    {
        bool rePlay = true;
        bool playable = true;
        while (rePlay)
        {
            rePlay = false;

            for (int i = _cubes.Count - 1; i >= 0; i--)
            {
                var cube = _cubes[i];
                if (!cube.IsHidden() && !cube.IsBlock())
                {
                    playable = false;
                    rePlay = true;
                    _cubes.RemoveAt(i);
                    cube.HiddenCube();
                }
            }
        }

        return playable;
    }

    public void Reshuffle()
    {
        for (int i = _cubes.Count - 1; i >= 0; i--)
        {
            var cube = _cubes[i];

            if (cube.IsBlock())
            {
                cube.transform.localRotation = RandomRotation();
                if (!cube.IsBlock())
                {
                    _cubes.RemoveAt(i);
                    cube.HiddenCube();
                }
            }
        }
    }
        
    private Quaternion RandomRotation()
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f
        );
        return randomRotation;
    }
    

    private void ShowCubes()
    {
        foreach (Transform cube in transform)
        {
            cube.GetComponent<TapCube>().ShowCube();
            /*
            SetSkin(cube.gameObject);
        */
        }
    }
    
    #endregion
}
