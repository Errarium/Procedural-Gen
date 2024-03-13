using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
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
    private Hashtable rooms = new Hashtable();

    private Vector3 startSize;
    private Vector3 startPos = new Vector3(0, 1, 0);
    // private HashSet<Edge> edges = new HashSet<Edge>();
    void Start()
    {
        PlaceRooms();   
        Contract();
        
        // Debug.Log("Number of elements in location: " + location.Count);
        
    }
    void Update()
    {
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

                location.Add(startPos, startSize);

                map.Add(startPos, startSize);
                rooms.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(
                    Mathf.Round(UnityEngine.Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxLenght + 1) * worldScale) / 2)) * 2, 
                    Mathf.Round(UnityEngine.Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxHeight + 1) * worldScale) / 2)) * 2,  
                    Mathf.Round(UnityEngine.Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxDepth + 1) * worldScale) / 2)) * 2
                );
                Vector3 pos = new Vector3(
                    Mathf.Round(UnityEngine.Random.Range(-worldSize * worldScale + roomSize.x * 0.5f, worldSize * worldScale - roomSize.x * 0.5f)), 
                    1, 
                    Mathf.Round(UnityEngine.Random.Range(-worldSize * worldScale + roomSize.z * 0.5f, worldSize * worldScale - roomSize.z * 0.5f))
                    );
                
                if(Collides(pos, roomSize, location))
                {
                    i -= 1;
                    continue;
                }
                
                //need to pass on lenght, height and depth values
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);
                room.transform.SetParent(this.transform);

                room.transform.localScale = new Vector3(roomSize.x, 1, roomSize.z);

                location.Add(pos, roomSize);
                rooms.Add(pos, room);
            }
        }
    }
    void Contract()
    {
        // Step 0: Remove starting room from locations
        location.Remove(startPos);

        // Step 4: Repeat for every room in locations
        while (location.Count > 0)
        {
            // Step 1: Find the room with the position closest to any position of a room in map
            Vector3 locRoom = new Vector3(worldSize * worldScale, worldSize * worldScale, worldSize * worldScale);
            Vector3 mapRoom = new Vector3(-worldSize * worldScale, -worldSize * worldScale, -worldSize * worldScale);

            foreach (Vector3 mapPos in map.Keys.Cast<Vector3>())
            {
                foreach (Vector3 locPos in location.Keys.Cast<Vector3>())
                {
                    if (Vector3.Distance(locPos, mapPos) < Vector3.Distance(locRoom, mapRoom))
                    {
                        locRoom = locPos;
                        mapRoom = mapPos;
                    }
                }
            } 

            // Step 2: Move the room to the closest position to the nearest room in map without overlapping
            Vector3 deltaPos = mapRoom - locRoom;

            Vector3 locSize = (Vector3)location[locRoom];
            Vector3 mapSize = (Vector3)map[mapRoom];
            Vector3 margin = new Vector3(mapSize.x / 2 + locSize.x / 2 + minSpace * worldScale, 0, mapSize.z / 2 + locSize.z / 2 + minSpace * worldScale);
            
            Vector3 adjust = new Vector3(deltaPos.x + margin.x * Mathf.Sign(deltaPos.x) * -1, 0, deltaPos.z + margin.z * Mathf.Sign(deltaPos.z) * -1);

            if (Math.Abs(locRoom.x) - Math.Abs(mapRoom.x) < Math.Abs(locRoom.z) - Math.Abs(mapRoom.z))
            {
                adjust.x = mapRoom.x - locRoom.x;
            } else{
                adjust.z = mapRoom.z - locRoom.z;
            }

            // Step 3: Remove room from locations and add to map
            GameObject room = (GameObject)rooms[locRoom];
            room.transform.position += adjust;

            rooms.Remove(locRoom);
            location.Remove(locRoom);   

            if (Collides(room.transform.position, locSize, map))
            {
                Destroy(room);
            }else{
                rooms.Add(room.transform.position, room);
                map.Add(room.transform.position, locSize); 

                Debug.Log($"old position: {locRoom} | new position: {locRoom + adjust}");
            }
        }
    }
    void Connections()
    {
        // Implement Bowyer Watson algorithm to get a delaunay triangulation over all rooms

        // Get points from the location hashtable
        List<Vector3> points = new List<Vector3>(map.Keys.Cast<Vector3>());        

        // Create an instance of DelaunayTriangulation
        DelaunayTriangulation Triangulate = new DelaunayTriangulation();
        
        // Triangulate using the instance
        List<Triangle> triangulation = Triangulate.BowyerWatson(points, new List<Triangle>());

        // Remove edges that are obstructed by a room depending on its size
        // List<Edge> validEdges = DeleteIntersections(triangulation);

        // Implement Prim's algorithm to get a minimum spanning tree over the triangulation
        // MinimumSpanningTree FindTree = new MinimumSpanningTree();
        // List<Edge> MST = FindTree.Prims(triangulation);

        // Visualize edges
        foreach (Triangle triangle in triangulation)
        {
            foreach (Edge edge in triangle.GetEdges())
            {
                Debug.DrawLine(edge.a, edge.b, Color.red, 5f);
            }   
        }
        // foreach (Edge edge in MST)
        // {
        //     Debug.DrawLine(edge.a, edge.b, Color.yellow, 60f);
        // } 
    }

    // private List<Edge> DeleteIntersections(List<Triangle> triangulation)
    // {
    //     // Remove edges that intersect more than two rooms
    //     List<Edge> validEdges = new List<Edge>();

    //     foreach (Triangle triangle in triangulation)
    //     {
    //         foreach (Edge edge in triangle.GetEdges())
    //         {
    //             // Check if the edge intersects more than two rooms
    //             int intersectedRooms = 0;

    //             foreach (Vector3 roomPos in map.Keys.Cast<Vector3>())
    //             {
    //                 Vector3 roomSize = (Vector3)map[roomPos];
    //                 Vector3 roomMin = roomPos - roomSize * 0.5f;
    //                 Vector3 roomMax = roomPos + roomSize * 0.5f;

    //                 if (edge.IntersectsRoom(roomMin, roomMax))
    //                 {
    //                     intersectedRooms++;

    //                     if (intersectedRooms > 2)
    //                         break; // No need to check further
    //                 }
    //             }

    //             if (intersectedRooms == 2)
    //             {
    //                 // The edge intersects exactly two rooms, consider it as a valid edge
    //                 validEdges.Add(edge);
    //             }
    //         }
    //     }
    //     return validEdges;
    // }
}
