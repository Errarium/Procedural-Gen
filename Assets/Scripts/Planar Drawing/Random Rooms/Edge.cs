using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
public class Edge
{
    public Vector3 a { get; private set; }
    public Vector3 b { get; private set; }
    public float weight{ get; private set; }
    public Edge(Vector3 vertexA, Vector3 vertexB)
    {
        a = vertexA;
        b = vertexB;
        weight = Vector3.Distance(a, b);
    }
    public bool IntersectsRoom(Vector3 roomMin, Vector3 roomMax)
    {
        // Check if the edge intersects with the given room

        float edgeMinX = Mathf.Min(a.x, b.x);
        float edgeMaxX = Mathf.Max(a.x, b.x);
        float edgeMinZ = Mathf.Min(a.z, b.z);
        float edgeMaxZ = Mathf.Max(a.z, b.z);

        // Check for no overlap
        // if (edgeMaxX < roomMin.x || edgeMinX > roomMax.x || edgeMaxZ < roomMin.z || edgeMinZ > roomMax.z)
        // {
        //     return false;
        // }
        //
        // return true;

        // Check for no overlap
        if (edgeMaxX < roomMin.x || edgeMinX > roomMax.x || edgeMaxZ < roomMin.z || edgeMinZ > roomMax.z)
        {
            return false;
        }

        // Check if the edge is entirely inside the room
        if (edgeMinX >= roomMin.x && edgeMaxX <= roomMax.x && edgeMinZ >= roomMin.z && edgeMaxZ <= roomMax.z)
        {
            return true;
        }

        // Check for partial overlap
        float overlapX = Mathf.Max(0, Mathf.Min(edgeMaxX, roomMax.x) - Mathf.Max(edgeMinX, roomMin.x));
        float overlapZ = Mathf.Max(0, Mathf.Min(edgeMaxZ, roomMax.z) - Mathf.Max(edgeMinZ, roomMin.z));

        // Check if the overlapping area is significant
        float overlapPercentage = overlapX * overlapZ / (edgeMaxX - edgeMinX) * (edgeMaxZ - edgeMinZ);

        return overlapPercentage > 0.9f;
    }

    // Method to get the other vertex of the edge given one vertex
    public Vector3 GetOtherVertex(Vector3 vertex)
    {
        // If the given vertex is equal to 'a', return 'b', and vice versa
        if (vertex == a)
        {
            return b;
        }
        else if (vertex == b)
        {
            return a;
        }
        else
        {
            Debug.LogError("Vertex not found in the edge.");
            return Vector3.zero; // Or any other appropriate default value
        }
    }
    // You may want to implement methods to check if a point lies on the edge.
}