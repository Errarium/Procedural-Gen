using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
public class Random2D : MonoBehaviour
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
    // private HashSet<Edge> edges = new HashSet<Edge>();
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
        // Implement Bowyer Watson algorithm to get a delaunay triangulation over all rooms

        // Get points from the location hashtable
        List<Vector3> points = new List<Vector3>(location.Keys.Cast<Vector3>());        

        // Create an instance of DelaunayTriangulation
        DelaunayTriangulation BowyerWatson = new DelaunayTriangulation();
        
        // Triangulate using the instance
        List<Triangle> triangulation = BowyerWatson.Triangulate(points, new List<Triangle>());

        // Visualize edges
        foreach (Triangle triangle in triangulation)
        {
            foreach (Edge edge in triangle.GetEdges())
            {
                Debug.DrawLine(edge.a, edge.b, Color.yellow, 60f);
            }   
        }

        // Remove edges that are obstructed by a room depending on its size

        // Implement Prim's algoritm to get a minimum spanning tree over the triangulation
    }   
}
