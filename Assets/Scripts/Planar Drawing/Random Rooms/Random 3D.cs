using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Random3D : MonoBehaviour
{
    [SerializeField] public int worldSize = 10;
    [SerializeField] private int worldScale = 5;
    [SerializeField] public GameObject prefab;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    [SerializeField] private float minSpace = 1f;
    private Hashtable location = new Hashtable();
    private Hashtable map = new Hashtable();
    private Vector3 startSize;
    private Vector3 startPos = new Vector3(0, 1, 0);

    void Start()
    {
        PlaceRooms();
        
        // Debug.Log("Number of elements in location: " + location.Count);
        
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Connections();
        }
    }
    bool Collides(Vector3 pos, Vector3 roomSize)
    {
        foreach (DictionaryEntry area in location)
        {
            Vector3 areaPos = (Vector3)area.Key;
            Vector3 areaSize = (Vector3)area.Value;

            float minDistanceX = (roomSize.x * 0.5f) + (areaSize.x * 0.5f) + minSpace * worldScale;
            float minDistanceZ = (roomSize.z * 0.5f) + (areaSize.z * 0.5f) + minSpace * worldScale;

            if (Mathf.Abs(pos.x - areaPos.x) < minDistanceX && Mathf.Abs(pos.z - areaPos.z) < minDistanceZ)
            {
                return true;
            }
        }
        return false;
    }
    void PlaceRooms()
    {
        for (int i = 0; i < Mathf.Round(worldSize * worldSize / math.sqrt((maxLenght+1) * (maxDepth+1))+1); i++)
        // for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                GameObject start = Instantiate(prefab, startPos, Quaternion.identity);
                start.transform.SetParent(this.transform);
                startSize = new Vector3(2 * worldScale, 1, 2 * worldScale);

                start.transform.localScale = startSize;

                location.Add(startPos, startSize);
                map.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(
                    Mathf.Round(UnityEngine.Random.Range(worldScale,(maxLenght + 1) * worldScale)), 
                    Mathf.Round(UnityEngine.Random.Range(worldScale,(maxHeight + 1) * worldScale)),     
                    Mathf.Round(UnityEngine.Random.Range(worldScale,(maxDepth + 1) * worldScale))
                );
                Vector3 pos = new Vector3(
                    Mathf.Round(UnityEngine.Random.Range(-worldSize * worldScale + roomSize.x * 0.5f, worldSize * worldScale - roomSize.x * 0.5f)), 
                    1, 
                    Mathf.Round(UnityEngine.Random.Range(-worldSize * worldScale + roomSize.z * 0.5f, worldSize * worldScale - roomSize.z * 0.5f))
                    );
                
                if(Collides(pos, roomSize))
                {
                    i -= 1;
                    continue;
                }
                
                //need to pass on lenght, height and depth values
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);
                room.transform.SetParent(this.transform);

                room.transform.localScale = new Vector3(roomSize.x, 1, roomSize.z);

                location.Add(pos, roomSize);
                map.Add(pos, room);
            }
        }
    }
    void Connections()
    {
        // implement Bowyer Watson algorithm to connect all rooms with eachother, then visualize this with yellow lines

        List<Vector3> points = new List<Vector3>(location.Keys.Cast<Vector3>());
        HashSet<Tuple<Vector3, Vector3>> edges = new HashSet<Tuple<Vector3, Vector3>>();

        // Step 1: Find minimum and maximum coordinates
        float minX = points.Min(p => p.x);
        float minY = points.Min(p => p.y);
        float minZ = points.Min(p => p.z);
        float maxX = points.Max(p => p.x);
        float maxY = points.Max(p => p.y);
        float maxZ = points.Max(p => p.z);

        // Step 2: Create supertriangle using extreme coordinates
        float dx = maxX - minX;
        float dy = maxY - minY;
        float dz = maxZ - minZ;
        float deltaMax = Mathf.Max(dx, Mathf.Max(dy, dz));
        float midX = (minX + maxX) / 2f;
        float midY = (minY + maxY) / 2f;
        float midZ = (minZ + maxZ) / 2f;

        Vector3 p1 = new Vector3(midX - 2 * deltaMax, midY - deltaMax * 1.5f, midZ - deltaMax * 1.5f);
        Vector3 p2 = new Vector3(midX + 2 * deltaMax, midY - deltaMax * 1.5f, midZ - deltaMax * 1.5f);
        Vector3 p3 = new Vector3(midX, midY + deltaMax * 1.5f, midZ - deltaMax * 1.5f);
        Vector3 p4 = new Vector3(midX, midY, midZ + deltaMax * 2.5f);

        // Clear points
        points.Clear();

        // Step 3: Add supertriangle vertices to the points list
        points.Add(p1);
        points.Add(p2);
        points.Add(p3);
        points.Add(p4);

        foreach (Vector3 point in points)
        {
            Debug.Log(point);
        }
        
        for (int c = 0; c < (points.Count() - 1); c++)
        {
            Vector3 currentPoint = points[c];

            for (int n = c + 1; n < points.Count(); n++)
            {
                Vector3 nextPoint = points[n];
                var edge = new Tuple<Vector3, Vector3>(currentPoint, nextPoint);

                if (edges.Contains(edge))
                continue;

                edges.Add(edge);   
            }
        }

        // Visualize edges
        foreach (var edge in edges)
        {
            Debug.DrawLine(edge.Item1, edge.Item2, Color.yellow, 60f);
        }
    }
}
