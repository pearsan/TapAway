using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
    private GameObject[,,] cubes;
    private void Start()
    {
        /*GenerateCubes();*/
        /*
        GenerateLevel();
    */
    }

    public void GenerateCubes()
    {
        cubes = new GameObject[width, height, depth];
        /*
        GameObject parentCube = new GameObject("Cube");
        */
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
                    
                    cubes[x, y, z] = cube;
                }
            }
        }

        transform.position = new Vector3(0, 0, 0);
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
        ClearCube();

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateCube(x, y);
            }
        }
        transform.position = new Vector3(0, 0, 0);
    }

    private void GenerateCube(int x, int y)
    {
        float centerOffsetX = (map.width - 1) * (1 + spacing) / 2f;
        float centerOffsetZ = (map.height - 1) * (1 + spacing) / 2f;
        
        Color pixelColor = map.GetPixel(x, y);
        if (_color.Equals(pixelColor))
        {
            GameObject cube = Instantiate(_cube);
            cube.transform.localScale = new Vector3(100, 100, 100);
            cube.transform.SetParent(transform);
            
            float offsetX = x * (1 + spacing);
            float offsetZ = y * (1 + spacing);

            cube.transform.position = new Vector3(offsetX - centerOffsetX, offsetZ - centerOffsetZ, 0 );
            cube.transform.localScale = Vector3.one;
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