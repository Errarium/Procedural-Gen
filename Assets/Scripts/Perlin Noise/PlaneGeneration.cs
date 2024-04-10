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
    private float seedAmp;
    private float seedFreq;
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
    private void Start()
    {
        Random.InitState(seed); // Seed the random number generator

        seedAmp = Random.Range(2, 4);
        seedFreq = Random.Range(2 * seedAmp, 4 * seedAmp);

        minHeight = -(amp1 + amp2 + amp3 + seedAmp) / 2;
        maxHeight = (amp1 + amp2 + amp3 + seedAmp) / 2;
    }
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
    private float NoiseMap(float x, float y)
    {
        float height = Mathf.PerlinNoise(x / freq1, y / freq1) * amp1;
        height += Mathf.PerlinNoise(x / freq2, y / freq2) * amp2;
        height += Mathf.PerlinNoise(x / freq3, y / freq3) * amp3;
        height += Mathf.PerlinNoise(x / seedFreq, y / seedFreq) * seedAmp;

        height += minHeight;

        return height;
    }
    private void GeneratePlane(Vector3 pos)
    {
        if (!map.Contains(pos))
        {
            pos.y = NoiseMap(pos.x, pos.z);
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
                    float height = NoiseMap(pos.x + vertices[i].x, pos.z + vertices[i].z);

                    vertices[i].y = height - pos.y;
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();

                // Assign color based on height relative to global minimum and maximum heights using gradient
                Color[] colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = heightGradient.Evaluate(Mathf.InverseLerp(minHeight / 2, maxHeight / 2, vertices[i].y)); // adjust colors
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

            map.Add(new Vector3(pos.x, 0, pos.z), _plane);
        }
    }

    private void GenerateObjectsWithinPlane(Vector3 planePos, Vector3[] vertices)
    {
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

                // Instantiate the object at the selected position with the same Y position as the plane
                GameObject _obj = Instantiate(obj, objPos, Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                _obj.transform.SetParent(this.transform);

                locDensity += 4;
            }
            else
            {
                GameObject obj = Random.value < 0.5f ? bigObjs[Random.Range(0, bigObjs.Length)] : smObjs[Random.Range(0, smObjs.Length)];

                // Instantiate the object at the selected position with the same Y position as the plane
                GameObject _obj = Instantiate(obj, objPos, Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                _obj.transform.SetParent(this.transform);

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
