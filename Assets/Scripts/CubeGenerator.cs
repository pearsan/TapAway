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
    [SerializeField] private bool generateShape = false;
    public GameObject shapePrefab;
    private List<TapCube> cubes;


    private void Start()
    {
        StartCoroutine(GenerateCubes());
    }

    private IEnumerator GenerateCubes()
    {
        /*
        GameObject parentCube = new GameObject("Cube");
        */
        if (!generateShape)
        {
            GenerateCubeLevel();
            yield return new WaitForSeconds(0.05f);            
        }
        else
        {
            GenerateShapeLevel();
            yield return new WaitForSeconds(0.05f);
        }


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

            foreach (var cube in cubes)
            {
                if (!cube.IsHidden())
                {
                    if (!cube.IsBlock())
                    {
                        playable = false;
                        canPlay = true;
                        cube.HiddenCube();
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
        foreach (var cube in cubes)
        {
            if (!cube.IsHidden())
            {
                cube.transform.rotation = RandomRotation();
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
    
    public void GenerateCubeLevel()
    {
        cubes = new List<TapCube>();
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
                    
                    cubes.Add(cube.gameObject.GetComponent<TapCube>());
                }
            }
        }
    }
    
    public void GenerateShapeLevel()
    {
        cubes = new List<TapCube>();
        ClearCube();
        shapePrefab.SetActive(true);

        MeshCollider meshCollider = shapePrefab.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            Debug.Log(true);
            Bounds bounds = meshCollider.bounds;
            float gridSize = 1f; // size of each cube

            List<Vector3> points = new List<Vector3>();

            for (float x = bounds.min.x; x < bounds.max.x; x += gridSize + spacing)
            {
                for (float y = bounds.min.y; y < bounds.max.y; y += gridSize + spacing)
                {
                    for (float z = bounds.min.z; z < bounds.max.z; z += gridSize + spacing)
                    {
                        Vector3 point =  new Vector3(x, y, z);
                        /*if (IsPointInsideCollider(point, meshCollider))
                        { 
                            GameObject cube = Instantiate(_cube, point, RandomRotation()); 
                            cube.transform.SetParent(transform);

                            cubes.Add(cube.gameObject.GetComponent<TapCube>());
                        }*/
                        /*if (IsPointInMesh(point))
                        {
                            points.Add(point);
                        }*/

                        if (IsInsideMeshCollider(meshCollider, point))
                        {
                            points.Add(point);
                        }
                    } 
                }
            }

            foreach (var point in points)
            {
                GameObject cube = Instantiate(_cube, point, RandomRotation()); 
                cube.transform.SetParent(transform);

                cubes.Add(cube.gameObject.GetComponent<TapCube>());
            }
            shapePrefab.SetActive(false);
        }
    }
    
    bool IsPointInsideCollider(Vector3 point, Collider collider)
    {
        // Create a tiny box at the point's position
        float size = 1.2f;
        Vector3 boxSize = new Vector3(size, size, size); // Can adjust size as needed

        // Check if the box collides with the collider
        Collider[] hitColliders = Physics.OverlapBox(point, boxSize / 2);
        
        // If the box collides with the collider, the point is inside the collider
        foreach (Collider hitCollider in hitColliders)
        {

            if (hitCollider == collider)
            {
                return true;
            }
        }
        
        return false;
    }
    
    bool IsInsideMeshCollider(MeshCollider col, Vector3 point)
    {
        var temp = Physics.queriesHitBackfaces;
        Ray ray = new Ray(point, Vector3.back);

        bool hitFrontFace = false;
        RaycastHit hit = default;

        Physics.queriesHitBackfaces = true;
        bool hitFrontOrBackFace = col.Raycast(ray, out RaycastHit hit2, 100f);
        if (hitFrontOrBackFace)
        {
            Physics.queriesHitBackfaces = false;
            hitFrontFace = col.Raycast(ray, out hit, 100f);
        }
        Physics.queriesHitBackfaces = temp;

        if (!hitFrontOrBackFace)
        {
            return false;
        }
        else if (!hitFrontFace)
        {
            return true;
        }
        else 
        {
            // This can happen when, for instance, the point is inside the torso but there's a part of the mesh (like the tail) that can still be hit on the front
            if (hit.distance > hit2.distance)
            {
                return true;
            }
            else
                return false;
        }

    }

    public void ShowCubes()
    {
        foreach (var cube in cubes)
        {
            cube.ShowCube();
        }
    }
    
    public void ClearCube()
    {
        cubes.Clear();
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }
}