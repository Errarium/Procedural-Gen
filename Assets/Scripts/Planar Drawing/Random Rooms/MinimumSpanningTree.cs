using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// ------------------------------- Work in progress -------------------------------
public class MinimumSpanningTree
{
    public HashSet<Edge> Prims(List<Triangle> triangulation)
    {
        HashSet<Edge> minimumSpanningTree = new HashSet<Edge>();
        HashSet<Vector3> visitedVertices = new HashSet<Vector3>();

        // Make a hashset of edges in triangulation
        HashSet<Edge> edges = new HashSet<Edge>();

        // Extract all edges from the triangulation
        foreach (Triangle triangle in triangulation)
        {
            edges.AddRange(triangle.GetEdges());
        }

        // Make a hashset of edges in triangulation
        List<Vector3> vertices = new List<Vector3>();

        // Extract all edges from the triangulation
        foreach (Edge edge in edges)
        {
            if(!vertices.Contains(edge.a))
                vertices.Add(edge.a);
            if(!vertices.Contains(edge.b))
                vertices.Add(edge.b);        
        }

        // Step 1: Add the edge with the smallest weight to the spanning tree and the vertices to visitedVertices
        
        Edge smallestEdge = edges.OrderBy(edge => edge.weight).First(); // Find the edge with the smallest weight
        
        minimumSpanningTree.Add(smallestEdge);
        edges.Remove(smallestEdge);
        visitedVertices.Add(smallestEdge.a);
        visitedVertices.Add(smallestEdge.b);

        float maxWeight = edges.Max(edge => edge.weight); // Find the edge with the largest weight

        // Step 3: Repeat step 2 for the number of vertices in triangulation -1 times

        for (int i = 0; i < (vertices.Count -1); i++)
        {
            // Step 2: Find the smallest weight edge among edges with one vertex not in visitedVertices and one in visitedVerticies add that edge to the spanning tree

            Edge nextEdge = null;
            float minWeight = maxWeight;

            // Finding smallest weighted edge
            foreach (Edge edge in edges)
            {
                if (visitedVertices.Contains(edge.a) && !visitedVertices.Contains(edge.b) ||
                    !visitedVertices.Contains(edge.a) && visitedVertices.Contains(edge.b))
                {
                    if (edge.weight < minWeight)
                    {
                        nextEdge = edge;
                        minWeight = edge.weight;
                    }
                }
            }

            // Adding edge to minimum spanning tree
            if (nextEdge != null)
            {
                minimumSpanningTree.Add(nextEdge);
                edges.Remove(nextEdge);

                if(!visitedVertices.Contains(nextEdge.a))
                    visitedVertices.Add(nextEdge.a);
                if(!visitedVertices.Contains(nextEdge.b))
                    visitedVertices.Add(nextEdge.b);

            } else{
                Debug.LogError("Index Error");
            }
        }

        return minimumSpanningTree;
    }
}
