using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector3 a { get; private set; }
    public Vector3 b { get; private set; }
    public Vector3 c { get; private set; }

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        a = p1;
        b = p2;
        c = p3;
    }
    public List<Edge> GetEdges()
    {
        List<Edge> edges = new List<Edge>
        {
            new Edge(a, b),
            new Edge(b, c),
            new Edge(c, a)
        };
        return edges;
    }
    public bool HasEdge(Edge edge)
    {
        return (edge.a == a && edge.b == b) ||
               (edge.a == b && edge.b == c) ||
               (edge.a == c && edge.b == a) ||
               (edge.b == a && edge.a == b) ||
               (edge.b == b && edge.a == c) ||
               (edge.b == c && edge.a == a);
    }
    public bool ContainsVertex(Vector3 vertex)
    {
        return a == vertex || b == vertex || c == vertex;
    }
    // You may want to implement methods to check if a point lies inside the circumcircle, 
}