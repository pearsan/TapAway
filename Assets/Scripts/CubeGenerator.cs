using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class CubeGenerator : MonoBehaviour
{

    [Range(0, 10)]
    public int width = 5;
    [Range(0, 10)]
    public int height = 5;
    [Range(0, 10)]
    public int depth = 5;
    
    public float spacing = 0.1f;

    [SerializeField] private GameObject _cube;
    [SerializeField] private Texture2D map;
    [SerializeField] private Color _color;
    private TapCube[,,] cubes;
    private bool[,,] canMove;


    private void Start()
    {
        StartCoroutine(GenerateCubes());
    }

    private IEnumerator GenerateCubes()
    {
        /*
        GameObject parentCube = new GameObject("Cube");
        */
        GenerateLevel();
        yield return new WaitForSeconds(0.05f);

        Autoplay();
        transform.position = new Vector3(0, 0, 0);

    }

    public void ResetGame()
    {
        StartCoroutine(GenerateCubes());
    }

    public void Autoplay()
    {
        bool canPlay = true;
        bool playable = true;
        while (canPlay)
        {
            canPlay = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (canMove[x, y, z] == false)
                        {
                            if (!cubes[x, y, z].IsBlock())
                            {
                                playable = false;
                                canPlay = true;
                                cubes[x, y, z].HiddenCube();
                                canMove[x, y, z] = true;
                            }
                        }
                    }
                }
            }
        }
        
        if (!playable)
        {
            Debug.Log("cant be play");
            Reshuffle();
            Autoplay();
        }
        else
        {
            Debug.Log("playable");
            ShowCubes();
        }
        
    }

    public void Reshuffle()
    {
        bool isPlayable = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (canMove[x, y, z] == false)
                    {
                        cubes[x, y, z].transform.rotation = RandomRotation();
                    }
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
    
    public void GenerateLevel()
    {
        cubes = new TapCube[width, height, depth];
        canMove = new bool[width, height, depth];
        ClearCube();
        float centerOffsetX = (width - 1) * (1 + spacing) / 2f;
        float centerOffsetY = (height - 1) * (1 + spacing) / 2f;
        float centerOffsetZ = (depth - 1) * (1 + spacing) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject cube = Instantiate(_cube);
                    cube.name ="" + x + ',' + y + ',' + z;
                    cube.transform.SetParent(transform);

                    float offsetX = x * (1 + spacing);
                    float offsetY = y * (1 + spacing);
                    float offsetZ = z * (1 + spacing);

                    Quaternion randomRotation = RandomRotation();

                    cube.transform.rotation = randomRotation;
                    
                    cube.transform.position = new Vector3(offsetX - centerOffsetX, offsetY - centerOffsetY, offsetZ - centerOffsetZ);
                    canMove[x, y, z] = false;
                    cubes[x, y, z] = cube.GetComponent<TapCube>();
                }
            }
        }

    }

    public void ShowCubes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    canMove[x, y, z] = false;
                    cubes[x, y, z].ShowCube();
                }
            }
        }
    }
    
    private void ClearCube()
    {
        for (int i = 0; i < cubes.GetLength(0); i++)
        {
            for (int j = 0; j < cubes.GetLength(1); j++)
            {
                for (int k = 0; k < cubes.GetLength(2); k++)
                {
                    cubes[i, j, k] = null;
                }
            }
        }
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }
}