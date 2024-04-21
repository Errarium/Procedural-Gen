using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
public class Expand : MonoBehaviour
{
    [SerializeField] public int worldSize = 10;
    [SerializeField] private int worldScale = 5;
    [SerializeField] private int loopChance = 15;
    [SerializeField] public GameObject prefab;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    [SerializeField] private float minSpace = 1f;
    private HashSet<(Vector3,Vector3)> location = new HashSet<(Vector3, Vector3)>();
    private Hashtable map = new Hashtable();
    private Hashtable rooms = new Hashtable();
    private Vector3 startSize;
    private Vector3 startPos = new Vector3(0, 1, 0);
    void Start()
    {
        PlaceRooms();   
        

    }
    void Update()
    {
        
        Expansion();
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Trigger");
            Connections();
        }
    }
    bool Collides(Vector3 pos, Vector3 roomSize, Hashtable collection)
    {
        foreach (DictionaryEntry area in collection)
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
        // for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                GameObject start = Instantiate(prefab, startPos, Quaternion.identity);
                start.transform.SetParent(this.transform);
                startSize = new Vector3(2 * worldScale, 1, 2 * worldScale);

                start.transform.localScale = startSize;
                location.Add((startPos, startSize));

                map.Add(startPos, startSize);
                rooms.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxLenght + 1) * worldScale) / 2)) * 2, 
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxHeight + 1) * worldScale) / 2)) * 2,  
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxDepth + 1) * worldScale) / 2)) * 2
                );
                float v = Random.Range(-1.99f, 1.01f);
                Vector3 pos = new Vector3(
                    (float)Math.Cos(v * Math.PI),
                    1, 
                    (float)Math.Sin(v * math.PI)
                );
                
                if(location.Contains((pos, roomSize)))
                {
                    i -= 1;
                    continue;
                }
                
                //need to pass on lenght, height and depth values
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);
                room.transform.SetParent(this.transform);

                room.transform.localScale = new Vector3(roomSize.x, 1, roomSize.z);

                location.Add((pos, roomSize));
                rooms.Add(pos, room);
            }
        }
    }
    void Expansion()
    {
        // Step 0: Remove starting room from locations
        location.Remove((startPos, startSize));

        // Step 3: Repeat for every room in locations
        while(location.Count() > 0)
        {
            foreach ((Vector3 locRoom, Vector3 locSize) in location)
            {                
                // Step 1: Move the room away from the starting room until it does not overlap
                Vector3 deltaPos = startPos - locRoom;
                Vector3 adjust = Vector3.zero;

                while (Collides(locRoom + adjust, locSize, map))
                {
                    adjust += deltaPos;
                }

                GameObject room = (GameObject)rooms[locRoom];
                room.transform.position = new Vector3(Mathf.Round(adjust.x), Mathf.Round(adjust.y), Mathf.Round(adjust.z));

                // Step 2: Remove room from locations and add to map
                rooms.Remove(locRoom);
                location.Remove((locRoom, locSize));   

                rooms.Add(room.transform.position, room);
                map.Add(room.transform.position, locSize); 

            }
        }
    }
    void Connections()
    {
        // Implement Bowyer Watson algorithm to get a delaunay triangulation over all rooms

        // Get points from the location hashtable
        List<Vector3> points = new List<Vector3>(map.Keys.Cast<Vector3>());        

        // Use Boywer Watson's algorithm for DelaunayTriangulation
        DelaunayTriangulation Triangulate = new DelaunayTriangulation();
        List<Triangle> triangulation = Triangulate.BowyerWatson(points, new List<Triangle>());

        // Remove edges that are obstructed by a room depending on its size
        // List<Edge> validEdges = DeleteIntersections(triangulation);

        // Implement Prim's algorithm to get a minimum spanning tree over the triangulation
        MinimumSpanningTree FindTree = new MinimumSpanningTree();
        HashSet<Edge> tree = FindTree.Prims(triangulation);

        // Add loops (edges in the triangulation not in tree)

        HashSet<Edge> edges = new HashSet<Edge>();
        HashSet<Edge> loops = new HashSet<Edge>();

        // Extract all edges from the triangulation
        foreach (Triangle triangle in triangulation)
        {
            edges.AddRange(triangle.GetEdges());
        }

        float midWeight = (edges.Max(edge => edge.weight) + edges.Min(edge => edge.weight)) / 3;

        foreach (Edge edge in edges)
        {
            if(tree.Contains(edge)){
                continue;
            }

            if (edge.weight < midWeight)
            {
                int chance = Random.Range(1,100);
                if(chance <= loopChance)
                    loops.Add(edge);
            }
        }

        // Use A* algorithm to create paths between all edges in tree and loops


        // Visualize edges
        foreach (Edge edge in edges)
        {
            Debug.DrawLine(edge.a, edge.b, Color.red, 5f);
            
        }
        foreach (Edge edge in tree)
        {
            Debug.DrawLine(edge.a, edge.b, Color.yellow, 60f);
        } 
        foreach (Edge loop in loops)
        {
            Debug.DrawLine(loop.a, loop.b, Color.green, 15f);
        }
    }
}
