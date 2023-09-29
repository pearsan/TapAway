using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
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

    [SerializeField] private GameObject cube;

    public GameObject shapePrefab;
    private List<TapCube> _cubes;

    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private string levelName;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void Create3DShapeLevel()
    {
        Generate3DShapeLevel();
    }
    
    public void Create3DGridLevel()
    {
        GenerateGridLevel();
    }
    
    public void ResetGame()
    {
        LoadJson();
    }
    
    //Call the function to make the puzzle Sovable
    private IEnumerator SolveGame()
    {
        yield return new WaitForSeconds(0.05f);
        Autoplay();
    }

    // Test and make the puzzle solvable
    private void Autoplay()
    {
        bool canPlay = true;
        bool playable = true;
        while (canPlay)
        {
            canPlay = false;

            foreach (var cube in _cubes)
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
            /*
            Debug.Log("cant be play");
            */
            Reshuffle();
            Autoplay();
        }
        else
        {
            /*
            Debug.Log("playable");
            */
            ShowCubes();
        }
        
    }

    public void Reshuffle()
    {
        foreach (var cube in _cubes)
        {
            if (!cube.IsHidden())
            {
                cube.transform.localRotation = RandomRotation();
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
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void GenerateGridLevel()
    {
        _cubes = new List<TapCube>();
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
                    GameObject cube = Instantiate(this.cube);
                    cube.name ="" + x + ',' + y + ',' + z;
                    cube.transform.SetParent(transform);

                    float offsetX = x * (1 + spacing);
                    float offsetY = y * (1 + spacing);
                    float offsetZ = z * (1 + spacing);

                    Quaternion randomRotation = RandomRotation();

                    cube.transform.localRotation = randomRotation;
                    
                    cube.transform.localPosition = new Vector3(offsetX - centerOffsetX, offsetY - centerOffsetY, offsetZ - centerOffsetZ);
                    
                    _cubes.Add(cube.gameObject.GetComponent<TapCube>());
                }
            }
        }
        
        StartCoroutine(SolveGame());
    }
    
    public void Generate3DShapeLevel()
    {
        _cubes = new List<TapCube>();
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
                        if (IsInsideMeshCollider(meshCollider, point))
                        {
                            points.Add(point);
                        }
                    } 
                }
            }

            foreach (var point in points)
            {
                GameObject cube = Instantiate(this.cube);
                cube.transform.SetParent(transform);
                cube.transform.localRotation = RandomRotation();
                cube.transform.localPosition = point;

                _cubes.Add(cube.gameObject.GetComponent<TapCube>());
            }
            shapePrefab.SetActive(false);
        }

        StartCoroutine(SolveGame());
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
        foreach (var cube in _cubes)
        {
            cube.ShowCube();
        }
    }
    
    public void ClearCube()
    {
        _cubes.Clear();
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }

    public void ExportObject()
    {
        // Create a new list to hold the positions.
        List<Vector3> positions = new List<Vector3>();

        // Add the position of each game object to the list.

        foreach (Transform child in transform)
        {
            // Now you can do something with each child GameObject
            /*Debug.Log(child.name);*/
            positions.Add(child.localPosition);
        }

        // Convert the list of positions to JSON.
        string jsonString = JsonConvert.SerializeObject(positions, Formatting.Indented, new Vector3Converter());

        // Write the JSON string to a file.
        string path = Path.Combine("Assets", "Resources", levelName + ".json");
        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
        Debug.Log("saved");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void LoadJson()
    {
        _cubes = new List<TapCube>();
        ClearCube();
        List<Vector3> positions = JsonConvert.DeserializeObject<List<Vector3>>(jsonFile.text, new Vector3Converter());
        int i = 0;
        foreach (Vector3 position in positions)
        {
            GameObject cube = Instantiate(this.cube);
            cube.name = "" + i;
            _cubes.Add(cube.gameObject.GetComponent<TapCube>());
            cube.transform.SetParent(transform);
            cube.transform.localRotation = RandomRotation();
            cube.transform.localPosition = position;
            i++;
        }
        Debug.Log("current cubes: " + i);
        StartCoroutine(SolveGame());
        transform.position = new Vector3(0, 0, 0);
    }
    
    public class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 vector = (Vector3)value;
            serializer.Serialize(writer, new float[] { vector.x, vector.y, vector.z });
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return new Vector3(array[0].Value<float>(), array[1].Value<float>(), array[2].Value<float>());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
}