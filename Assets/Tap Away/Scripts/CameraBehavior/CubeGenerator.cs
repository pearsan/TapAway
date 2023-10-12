using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
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

    [SerializeField] private GameObject cubePrefabs;

    public GameObject shapePrefab;
    private List<TapCube> _cubes;

    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private string levelName;
    [SerializeField] private PurchaseSkinSO currentSkinSo;
    [SerializeField] private PurchaseSkinSO defaultSkinSo;
    [SerializeField] private bool isHollow;
    // ReSharper disable Unity.PerformanceAnalysis

    #region Level Editor

    public void Create3DShapeLevel()
    {
        Generate3DShapeLevel();
    }
    
    public void Create3DGridLevel()
    {
        GenerateGridLevel();
    }
    
        // ReSharper disable Unity.PerformanceAnalysis
    private void GenerateGridLevel()
    {
        SetSkin(cubePrefabs);
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
                    GameObject cube = Instantiate(cubePrefabs);
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
        SetSkin(cubePrefabs);
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
                        if (!isHollow)
                        {
                            if (IsInsideMeshCollider(meshCollider, point))
                            {
                                points.Add(point);
                            }                            
                        }
                        else
                        {
                            if (IsOnMeshCollider(meshCollider, point))
                            {
                                points.Add(point);
                            }
                        }

                    } 
                }
            }

            foreach (var point in points)
            {
                GameObject cube = Instantiate(this.cubePrefabs);
                cube.transform.SetParent(transform);
                cube.transform.localRotation = RandomRotation();
                cube.transform.localPosition = point;

                _cubes.Add(cube.gameObject.GetComponent<TapCube>());
            }
            shapePrefab.SetActive(false);
        }

        StartCoroutine(SolveGame());
    }

    #endregion
    
    
    public void ResetGame()
    {
        StartCoroutine(SetupLevel());
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
    
    bool IsOnMeshCollider(Collider collider, Vector3 point)
    {
        // Create a tiny box at the point's position
        Vector3 boxSize = new Vector3(1.5f, 1.5f, 1.5f); // Can adjust size as needed

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

        // If the box doesn't collide with the collider, the point is outside the collider
        return false;
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

#if  UNITY_EDITOR
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
        string path = Path.Combine("Assets", "Tap Away", "Resources", levelName + ".json");
        File.WriteAllText(path, jsonString);
        AssetDatabase.Refresh();
        Debug.Log("saved");
    }
#endif

    #region Setup Level

    public IEnumerator SetupLevel()
    {
        LoadJson();
        yield return new WaitForSeconds(1f);
        Autoplay();

        transform.position = new Vector3(0, 0, 0);
        
        // Get the total bounds of all the children objects
        Bounds bounds = CalculateTotalBounds();

        // Calculate the center point
        Vector3 centerPoint = bounds.center;

        // Calculate the offset
        Vector3 offset = transform.position - centerPoint;

        // Move children objects to have the parent in the center
        MoveChildrenToCenterPoint(offset);
        foreach (var cube in _cubes)
        {
            MoveChild(cube.transform);
        }

        foreach (var cube in _cubes)
        {
            MoveAnimChild(cube.transform);
        }
    }

    public void LoadJson()
    {
        bool hasCubes = false;
        /*
        SetSkin(cubePrefabs);
        */
        _cubes = new List<TapCube>();
        ClearCube();
        List<Vector3> positions = JsonConvert.DeserializeObject<List<Vector3>>(jsonFile.text, new Vector3Converter());
        int i = 0;
        foreach (Vector3 position in positions)
        {
            GameObject cube = Instantiate(this.cubePrefabs);
            cube.name = "" + i;
            _cubes.Add(cube.gameObject.GetComponent<TapCube>());
            cube.transform.SetParent(transform);
            cube.transform.localRotation = RandomRotation();
            cube.transform.localPosition = position;
            i++;
        }
        Debug.Log("current cubes: " + i);
        hasCubes = true;
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
        foreach (var cube in _cubes)
        {
            MoveChild(cube.transform);
        }
        foreach (var cube in _cubes)
        {
            MoveAnimChild(cube.transform);
        }
    }
    
    private IEnumerator SolveGame()
    {
        yield return new WaitForSeconds(0.05f);
        Autoplay();
    }

    // Test and make the puzzle solvable
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

            foreach (var cube in _cubes)
            {
                if (!cube.IsHidden())
                {
                    if (!cube.IsBlock())
                    {
                        playable = false;
                        rePlay = true;
                        
                        cube.HiddenCube();
                    }
                }
            }
        }

        return playable;
    }

    public void Reshuffle()
    {
        foreach (var cube in _cubes)
        {             
            if (!cube.IsHidden())
            {
                if (cube.IsBlock())
                {
                    cube.transform.localRotation = RandomRotation();
                    if (!cube.IsBlock())
                    {
                        cube.HiddenCube();
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
    
    private void SetSkin(GameObject cube)
    {
        if (currentSkinSo == null)
        {
            cube.GetComponentInChildren<MeshFilter>().sharedMesh = defaultSkinSo.ShopItemPrefab.GetComponent<MeshFilter>().sharedMesh;
            cube.GetComponentInChildren<MeshRenderer>().sharedMaterial =
                defaultSkinSo.ShopItemPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
        else
        {
            cube.GetComponentInChildren<MeshFilter>().sharedMesh = currentSkinSo.ShopItemPrefab.GetComponent<MeshFilter>().sharedMesh;
            cube.GetComponentInChildren<MeshRenderer>().sharedMaterial =
                currentSkinSo.ShopItemPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
    
    private void ShowCubes()
    {
        foreach (var cube in _cubes)
        {
            cube.ShowCube();
            SetSkin(cube.gameObject);
        }
    }
    
    #endregion
    
    
    
    
    private void MoveChild(Transform cube)
    {
        float distance = 10.0f; // Define how far you want to move the child
        // Get the direction from the parent to the child
        Vector3 direction = cube.position;
        direction.Normalize(); // Make the direction a unit vector

        // Move the child a certain distance along this line
        cube.GetChild(0).position = cube.position + direction * distance;
    }
    
    private void MoveAnimChild(Transform cube)
    {
        Transform cubeMesh = cube.GetChild(0);
        cube.gameObject.GetComponent<TapCube>().SetCanDoMove(false);
        cubeMesh.DOLocalMove(new Vector3(0, 0, 0), 1).SetEase(Ease.InOutSine).OnComplete((() => cube.gameObject.GetComponent<TapCube>().SetCanDoMove(true)));
    }

    private Bounds CalculateTotalBounds()
    {
        Bounds bounds = new Bounds();

        foreach (Transform child in transform)
        {
            Renderer renderer = child.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                if (bounds.size == Vector3.zero)
                {
                    bounds = renderer.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;
    }

    void MoveChildrenToCenterPoint(Vector3 offset)
    {
        foreach (Transform child in transform)
        {
            child.position += offset; // Move the child object by applying the offset
        }
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

    public void SetLevel(TextAsset level)
    {
        jsonFile = level;
    }
}