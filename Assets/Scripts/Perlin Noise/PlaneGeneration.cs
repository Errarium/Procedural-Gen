using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaneGeneration : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;
    public GameObject[] bigObjs;
    public GameObject[] smObjs;
    public GameObject[] underwaterObjs;
    [SerializeField] public int amp1 = 5;
    [SerializeField] public int amp2 = 5;
    [SerializeField] public int amp3 = 5;
    [SerializeField] public int freq1 = 10;
    [SerializeField] public int freq2 = 5;
    [SerializeField] public int freq3 = 5;
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
    public float minHeight = float.MaxValue;
    public float maxHeight = float.MinValue;

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

                for (int i = 0; i < vertices.Length; i++)
                {
                    float height = Mathf.PerlinNoise((pos.x + vertices[i].x) / freq1, (pos.z + vertices[i].z) / freq1) * amp1 - math.sqrt(amp1);
                    height += Mathf.PerlinNoise((pos.x + vertices[i].x) / freq2, (pos.z + vertices[i].z) / freq2) * amp2 - 2* math.sqrt(amp2);
                    height += Mathf.PerlinNoise((pos.x + vertices[i].x) / freq3, (pos.z + vertices[i].z) / freq3) * amp3 - 4;
                    vertices[i].y = height;

                    if (height < minHeight)
                        minHeight = height;
                    if (height > maxHeight)
                        maxHeight = height;
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();

                // Assign color based on height relative to global minimum and maximum heights using gradient
                Color[] colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = heightGradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y));
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

            // Select a random object for each plane
            if (objPos.y < 1)
            {
                GameObject obj = underwaterObjs[Random.Range(0, underwaterObjs.Length)];
                Instantiate(obj, objPos, Quaternion.identity);
                locDensity += 4;
            }else{
                GameObject obj = Random.value < 0.5f ? bigObjs[Random.Range(0, bigObjs.Length)] : smObjs[Random.Range(0, smObjs.Length)];

                // Instantiate the object at the selected position with the same Y position as the plane
                Instantiate(obj, objPos, Quaternion.identity);

                objLocations.Add(objPos);

                if (bigObjs.Contains(obj))
                    locDensity += 6;
                else
                    locDensity += 2;
            }
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