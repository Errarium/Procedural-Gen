// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class ActivePlane : MonoBehaviour
// {
//     public GameObject plane;
//     public GameObject player;
//     [SerializeField] private int amp = 5;
//     [SerializeField] private int freq = 10;
//     [SerializeField] private int radius = 5;
//     private int planeOffset = 10;
//     private Vector3 startPos = Vector3.zero;
//     private int pMoveX => (int)(player.transform.position.x - startPos.x);
//     private int pMoveZ => (int)(player.transform.position.z - startPos.z);
//     private int pLocX => (int)MathF.Floor(player.transform.position.x / planeOffset) * planeOffset;
//     private int pLocZ => (int)MathF.Floor(player.transform.position.z / planeOffset) * planeOffset;
//     private Hashtable map = new Hashtable();

//     private void Update()
//     {
//         if (startPos == Vector3.zero)
//         {
//             for (int x = -radius; x <= radius; x++)
//             {
//                 for (int z = -radius; z <= radius; z++)
//                 {
//                     Vector3 pos = new Vector3(x * planeOffset, 0, z * planeOffset);
//                     GeneratePlains(pos);
//                 }
//             }
//         }
//         if (HasPMoved())
//         {
//             for (int x = -radius; x <= radius; x++)
//             {
//                 for (int z = -radius; z <= radius; z++)
//                 {
//                     Vector3 pos = new Vector3(x * planeOffset + pLocX, 0, z * planeOffset + pLocZ);
//                     ActivatePlanes();
//                     GeneratePlains(pos);
//                 }
//             }
//         }
//     }

//     private void GeneratePlains(Vector3 pos)
//     {
//         if (!map.Contains(pos))
//         {
//             GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
//             _plane.transform.SetParent(this.transform);

//             // Deform the plane using Perlin noise for heightmap
//             MeshFilter meshFilter = _plane.GetComponent<MeshFilter>();
//             if (meshFilter != null)
//             {
//                 Mesh mesh = meshFilter.mesh;
//                 Vector3[] vertices = mesh.vertices;

//                 for (int i = 0; i < vertices.Length; i++)
//                 {
//                     float height = Mathf.PerlinNoise((pos.x + vertices[i].x) / freq, (pos.z + vertices[i].z) / freq) * amp;
//                     vertices[i].y = height;
//                 }
//                 mesh.vertices = vertices;
//                 mesh.RecalculateNormals();
//             }

//             map.Add(pos, _plane);
//         }
//     }

//     private void ActivatePlanes()
//     {
//         foreach (GameObject _plane in map.Values)
//         {
//             bool beyondRadius = Mathf.Abs(_plane.transform.position.x - pLocX) > radius * planeOffset || Mathf.Abs(_plane.transform.position.z - pLocZ) > radius * planeOffset;
//             _plane.SetActive(!beyondRadius);
//         }
//     }

//     private bool HasPMoved()
//     {
//         if (Mathf.Abs(pMoveX) >= planeOffset || Mathf.Abs(pMoveZ) >= planeOffset)
//         {
//             return true;
//         }
//         return false;
//     }
// }

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class PlaneGeneration : MonoBehaviour
// {
//     public GameObject plane;
//     public GameObject player;
//     [SerializeField] private int amp = 5;
//     [SerializeField] private int freq = 10;
//     [SerializeField] private int seed = 0;
//     [SerializeField] private int radius = 5;
//     private int planeOffset = 10;
//     Color[] colors;
//     public Gradient gradient;
//     private float minHeight;
//     private float maxHeight;

//     private Vector3 startPos = Vector3.zero;
//     private int pMoveX => (int)(player.transform.position.x - startPos.x);
//     private int pMoveZ => (int)(player.transform.position.z - startPos.z);
//     private int pLocX => (int)Mathf.Floor(player.transform.position.x / planeOffset) * planeOffset;
//     private int pLocZ => (int)Mathf.Floor(player.transform.position.z / planeOffset) * planeOffset;
//     private Hashtable map = new Hashtable();

//     private void Update()
//     {
//         if (startPos == Vector3.zero)
//         {
//             GenerateWorld();
//         }
//         else if (HasPlayerMoved())
//         {
//             GenerateWorld();
//         }
//     }

//     private void GenerateWorld()
//     {
//         ClearInactivePlanes();
//         for (int x = -radius; x <= radius; x++)
//         {
//             for (int z = -radius; z <= radius; z++)
//             {
//                 Vector3 pos = new Vector3(x * planeOffset + pLocX, 0, z * planeOffset + pLocZ);
//                 GeneratePlane(pos);
//             }
//         }
//         startPos = player.transform.position;
//     }

//     private void GeneratePlane(Vector3 pos)
//     {
//         if (!map.Contains(pos))
//         {
//             GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
//             _plane.transform.SetParent(this.transform);

//             // Deform the plane using Perlin noise for heightmap
//             MeshFilter meshFilter = _plane.GetComponent<MeshFilter>();
//             if (meshFilter != null)
//             {
//                 Mesh mesh = meshFilter.mesh;
//                 Vector3[] vertices = mesh.vertices;

//                 // Set seed for Perlin noise
//                 UnityEngine.Random.InitState(seed);

//                 for (int i = 0; i < vertices.Length; i++)
//                 {
//                     float height = Mathf.PerlinNoise((pos.x + vertices[i].x) / freq, (pos.z + vertices[i].z) / freq);
                    
//                     if (height < minHeight)
//                         minHeight = height;
//                     if (height > maxHeight)
//                         maxHeight = height;

//                     vertices[i].y = height * amp;
//                 }
//                 mesh.vertices = vertices;
//                 mesh.RecalculateNormals();

//                 colors = new Color[vertices.Length];
//                 for (int i = 0, x = 0; x < 10; x++)
//                 {
//                     for (int z = 0; z < 10; z++)
//                     {
//                         colors[i] = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y));
//                         i++;
//                     }
//                 }
//                 mesh.colors = colors;
//             }

//             // Deform the collider
//             MeshCollider meshCollider = _plane.GetComponent<MeshCollider>();
//             if (meshCollider != null)
//             {
//                 meshCollider.sharedMesh = meshFilter.mesh;
//             }

//             map.Add(pos, _plane);
//         }
//     }

//     private void ClearInactivePlanes()
//     {
//         List<Vector3> positionsToRemove = new List<Vector3>();
//         foreach (Vector3 pos in map.Keys)
//         {
//             GameObject _plane = (GameObject)map[pos];
//             bool beyondRadius = Mathf.Abs(_plane.transform.position.x - pLocX) > radius * planeOffset || Mathf.Abs(_plane.transform.position.z - pLocZ) > radius * planeOffset;
//             if (beyondRadius)
//             {
//                 Destroy(_plane);
//                 positionsToRemove.Add(pos);
//             }
//         }
//         foreach (Vector3 pos in positionsToRemove)
//         {
//             map.Remove(pos);
//         }
//     }

//     private bool HasPlayerMoved()
//     {
//         return Mathf.Abs(pMoveX) >= planeOffset || Mathf.Abs(pMoveZ) >= planeOffset;
//     }
// }

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaneGeneration : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;
    public GameObject[] plants;
    public GameObject[] rocks;
    [SerializeField] private int amp = 5;
    [SerializeField] private int freq = 10;
    [SerializeField] private int seed = 0;
    [SerializeField] private int radius = 5;
    [SerializeField] private int density = 10;
    [SerializeField] private Gradient heightGradient;
    private int planeOffset = 10;
    private Vector3 startPos = Vector3.zero;
    private int pMoveX => (int)(player.transform.position.x - startPos.x);
    private int pMoveZ => (int)(player.transform.position.z - startPos.z);
    private int pLocX => (int)Mathf.Floor(player.transform.position.x / planeOffset) * planeOffset;
    private int pLocZ => (int)Mathf.Floor(player.transform.position.z / planeOffset) * planeOffset;
    private Hashtable map = new Hashtable();

    private float globalMinHeight = float.MaxValue;
    private float globalMaxHeight = float.MinValue;

    private void Update()
    {
        if (startPos == Vector3.zero)
        {
            GenerateWorld();
        }
        else if (HasPlayerMoved())
        {
            GenerateWorld();
        }
    }

    private void GenerateWorld()
    {
        ClearInactivePlanes();
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector3 pos = new Vector3(x * planeOffset + pLocX, 0, z * planeOffset + pLocZ);
                GeneratePlane(pos);
            }
        }
        startPos = player.transform.position;
    }

    private void GeneratePlane(Vector3 pos)
    {
        if (!map.Contains(pos))
        {
            GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
            _plane.transform.SetParent(this.transform);

            // Deform the plane using Perlin noise for heightmap
            MeshFilter meshFilter = _plane.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;

                float minHeight = float.MaxValue;
                float maxHeight = float.MinValue;

                for (int i = 0; i < vertices.Length; i++)
                {
                    float height = Mathf.PerlinNoise((pos.x + vertices[i].x) / freq, (pos.z + vertices[i].z) / freq) * amp;
                    vertices[i].y = height;

                    if (height < minHeight)
                        minHeight = height;
                    if (height > maxHeight)
                        maxHeight = height;

                    if (height < globalMinHeight)
                        globalMinHeight = height;
                    if (height > globalMaxHeight)
                        globalMaxHeight = height;
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();

                // Assign color based on height relative to global minimum and maximum heights using gradient
                Color[] colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = heightGradient.Evaluate(Mathf.InverseLerp(globalMinHeight, globalMaxHeight, vertices[i].y));
                }
                mesh.colors = colors;

                // Generate objects within the plane
                GenerateObjectsWithinPlane(pos, vertices);
            }

            // Deform the collider
            MeshCollider meshCollider = _plane.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshFilter.mesh;
            }

            map.Add(pos, _plane);
        }
    }

    private void GenerateObjectsWithinPlane(Vector3 planePos, Vector3[] vertices)
    {
        //Random.InitState(seed); // Seed the random number generator

        int locDensity = 0;
        List<Vector3> objLocations = new List<Vector3>();

        while (locDensity < density)
        {
            // Randomly select a vertex within the mesh for each plane
            int vertexIndex = Random.Range(0, vertices.Length);
            Vector3 objPos = planePos + vertices[vertexIndex];

            if (objLocations.Contains(objPos))
                continue;

            // Select a random object (plant or rock) for each plane
            GameObject obj = Random.value < 0.5f ? plants[Random.Range(0, plants.Length)] : rocks[Random.Range(0, rocks.Length)];

            // Instantiate the object at the selected position with the same Y position as the plane
            Instantiate(obj, objPos, Quaternion.identity);

            objLocations.Add(objPos);

            locDensity += 5;
        }
    }


    private void ClearInactivePlanes()
    {
        List<Vector3> positionsToRemove = new List<Vector3>();
        foreach (Vector3 pos in map.Keys)
        {
            GameObject _plane = (GameObject)map[pos];
            bool beyondRadius = Mathf.Abs(_plane.transform.position.x - pLocX) > radius * planeOffset || Mathf.Abs(_plane.transform.position.z - pLocZ) > radius * planeOffset;
            if (beyondRadius)
            {
                Destroy(_plane);
                positionsToRemove.Add(pos);
            }
        }
        foreach (Vector3 pos in positionsToRemove)
        {
            map.Remove(pos);
        }
    }

    private bool HasPlayerMoved()
    {
        return Mathf.Abs(pMoveX) >= planeOffset || Mathf.Abs(pMoveZ) >= planeOffset;
    }
}
