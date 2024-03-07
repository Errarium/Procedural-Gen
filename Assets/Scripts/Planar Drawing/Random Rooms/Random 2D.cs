using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// public class Triangle
// {
//     public Vector3 a { get; private set; }
//     public Vector3 b { get; private set; }
//     public Vector3 c { get; private set; }

//     public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
//     {
//         a = p1;
//         b = p2;
//         c = p3;
//     }
//     public List<Edge> GetEdges()
//     {
//         List<Edge> edges = new List<Edge>
//         {
//             new Edge(a, b),
//             new Edge(b, c),
//             new Edge(c, a)
//         };
//         return edges;
//     }
//     public bool HasEdge(Edge edge)
//     {
//         return (edge.a == a && edge.b == b) ||
//                (edge.a == b && edge.b == c) ||
//                (edge.a == c && edge.b == a) ||
//                (edge.b == a && edge.a == b) ||
//                (edge.b == b && edge.a == c) ||
//                (edge.b == c && edge.a == a);
//     }

//     // You may want to implement methods to check if a point lies inside the circumcircle, 
// }

// public class Edge
// {
//     public Vector3 a { get; private set; }
//     public Vector3 b { get; private set; }

//     public Edge(Vector3 p1, Vector3 p2)
//     {
//         a = p1;
//         b = p2;
//     }

//     // You may want to implement methods to check if a point lies on the edge.
// }

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
    // private  HashSet<Tuple<Vector3, Vector3>> edges = new HashSet<Tuple<Vector3, Vector3>>();
    private HashSet<Edge> edges = new HashSet<Edge>();


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
        List<Triangle> triangulation = new List<Triangle>();
        // List<Vector3> exPoints = new List<Vector3>();
 
        Triangulate(points, triangulation);

        // Visualize edges
        foreach (Triangle triangle in triangulation)
        {
            foreach (Edge edge in triangle.GetEdges())
            {
                Debug.DrawLine(edge.a, edge.b, Color.yellow, 60f);
            }   
        }
    }
    // void StartingCircle()
    // {
    //     List<Vector3> points = new List<Vector3>(location.Keys.Cast<Vector3>());

    //     // Convert Vector3 points to Point objects
    //     List<Point> pointObjects = points.Select(v => new Point(v.x, v.z)).ToList();

    //     Circle startingCircle = SmallestEnclosingCircle.MakeCircle(pointObjects);

    //     if (startingCircle.r > 0)
    //     {
    //         // Calculate the vertices of the triangle that circumscribes the starting circle
    //         Vector3 p1 = new Vector3((float)startingCircle.c.x - (float)startingCircle.r, 0, (float)startingCircle.c.y - (float)startingCircle.r);
    //         Vector3 p2 = new Vector3((float)startingCircle.c.x, 0, (float)startingCircle.c.y + (float)startingCircle.r);
    //         Vector3 p3 = new Vector3((float)startingCircle.c.x + (float)startingCircle.r, 0, (float)startingCircle.c.y - (float)startingCircle.r);

    //         // Display the starting circle and triangle
    //         Debug.DrawLine(2*p1, 2*p2, Color.red, 60f);
    //         Debug.DrawLine(2*p2, 2*p3, Color.red, 60f);
    //         Debug.DrawLine(2*p3, 2*p1, Color.red, 60f);
    //     }
    // }
    public bool IsPointInsideCircumcircle(Vector3 point, Triangle triangle)
    {
        // Get the vertices of the triangle
        Vector3 p1 = triangle.a;
        Vector3 p2 = triangle.b;
        Vector3 p3 = triangle.c;

        // Calculate the circumcircle center and radius
        float D = 2 * (p1.x * (p2.z - p3.z) + p2.x * (p3.z - p1.z) + p3.x * (p1.z - p2.z));
        float Ux = ((p1.x * p1.x + p1.z * p1.z) * (p2.z - p3.z) + (p2.x * p2.x + p2.z * p2.z) * (p3.z - p1.z) + (p3.x * p3.x + p3.z * p3.z) * (p1.z - p2.z)) / D;
        float Uz = ((p1.x * p1.x + p1.z * p1.z) * (p3.x - p2.x) + (p2.x * p2.x + p2.z * p2.z) * (p1.x - p3.x) + (p3.x * p3.x + p3.z * p3.z) * (p2.x - p1.x)) / D;

        Vector3 circumcenter = new Vector3(Ux, 1, Uz);
        float circumradius = Mathf.Sqrt((p1.x - circumcenter.x) * (p1.x - circumcenter.x) + (p1.z - circumcenter.z) * (p1.z - circumcenter.z));

        // Check if the point is inside the circumcircle
        float distanceToCircumcenter = Mathf.Sqrt((point.x - circumcenter.x) * (point.x - circumcenter.x) + (point.z - circumcenter.z) * (point.z - circumcenter.z));
        return distanceToCircumcenter <= circumradius;
    }

    void Triangulate(List<Vector3> points, List<Triangle> triangulation)
    {
        // Step 1: Find minimum and maximum coordinates
        float minX = points.Min(p => p.x);
        float minY = points.Min(p => p.z);
        float maxX = points.Max(p => p.x);
        float maxY = points.Max(p => p.z);

        // Step 2: Create super-triangle using extreme coordinates
        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy);
        float midX = (minX + maxX) / 2f;
        float midY = (minY + maxY) / 2f;

        // Step 3: Add super-triangle to triangulation
        Vector3 p1 = new Vector3(midX - 1.5f * deltaMax, 1, midY - deltaMax);
        Vector3 p2 = new Vector3(midX, 1, midY + deltaMax * 1.5f);
        Vector3 p3 = new Vector3(midX + 1.5f * deltaMax, 1, midY - deltaMax);
        triangulation.Add(new Triangle(p1, p2, p3));

        foreach (Vector3 point in points)
        {
            List<Triangle> badTriangles = triangulation
                .Where(triangle => IsPointInsideCircumcircle(point, triangle))
                .ToList();

            HashSet<Edge> polygon = new HashSet<Edge>();
            foreach (Triangle badTriangle in badTriangles)
            {
                foreach (Edge edge in badTriangle.GetEdges())
                {
                    if (badTriangles.Count(t => t.HasEdge(edge)) == 1)
                    {
                        polygon.Add(edge);
                    }
                }
            }

            foreach (Triangle badTriangle in badTriangles)
            {
                triangulation.Remove(badTriangle);
            }

            foreach (Edge edge in polygon)
            {
                triangulation.Add(new Triangle(edge.a, edge.b, point));
            }
        }
        // Step 10: Clean-Up, remove all egdes with verticies belonging to the super-triangle
        triangulation.RemoveAll(triangle => triangle.ContainsVertex(p1) || triangle.ContainsVertex(p2) || triangle.ContainsVertex(p3));
    }
}
