using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayGenerater : CubeGenerator
{
    
    #region Setup Level

    
    // ReSharper disable Unity.PerformanceAnalysis
    

    public IEnumerator SetupLevel(GameObject tapCube)
    {
        yield return null;
        yield return LoadJson(tapCube);
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(Autoplay());

        IntroAnimation();
        
        GameplayManager.Instance.SetDefaultMoveAttemps();
        yield return new WaitForSeconds(1f);
    }

    private void IntroAnimation()
    {
        ShowCubes();
        
        transform.position = new Vector3(0, 0, 0);

        Bounds bounds = CalculateTotalBounds();

        // Calculate the center point
        Vector3 centerPoint = bounds.center;

        // Calculate the offset
        Vector3 offset = transform.position - centerPoint;

        // Move children objects to have the parent in the center
        MoveChildrenToCenterPoint(offset);
        

        foreach (Transform cube in transform)
        {
            MoveChild(cube);
        }

        foreach (Transform cube in transform)
        {
            MoveAnimChild(cube);
        }
    }

    private IEnumerator LoadJson(GameObject tapCube)
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

        yield return null;
    }

    public override void LoadCurrentLevel(TextAsset _levelInProgress)
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


    private IEnumerator Autoplay()
    {
        bool playable = AutoCheck();
        while (!playable)
        {
            yield return StartCoroutine(Reshuffle());
            playable = AutoCheck();
        }

    }

    private bool AutoCheck()
    {
        yield return null;
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

    public IEnumerator Reshuffle()
    {

        for (int i = _cubes.Count - 1; i >= 0; i--)
        {
            var cube = _cubes[i];

            if (cube.IsBlock())
            {
                cube.transform.localRotation = RandomRotation();
            }
            /*yield return null;
            
            if (!cube.IsBlock())
            {
                _cubes.RemoveAt(i);
                cube.HiddenCube();
            }*/
        }
        yield return null;

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

        }
    }
    
    #endregion
}
