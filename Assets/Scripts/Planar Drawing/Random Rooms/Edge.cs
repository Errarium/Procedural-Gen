using UnityEngine;
public class Edge
{
    public Vector3 a { get; private set; }
    public Vector3 b { get; private set; }

    public Edge(Vector3 p1, Vector3 p2)
    {
        a = p1;
        b = p2;
    }

    // You may want to implement methods to check if a point lies on the edge.
}