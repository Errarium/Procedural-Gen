using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelaunayTriangulation
{
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

    public List<Triangle> Triangulate(List<Vector3> points, List<Triangle> triangulation)
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

        foreach (Vector3 point in points)
        {
            // Step 4: Identify bad triangles
            List<Triangle> badTriangles = triangulation.Where(triangle => IsPointInsideCircumcircle(point, triangle)).ToList();

            HashSet<Edge> polygon = new HashSet<Edge>();
            foreach (Triangle badTriangle in badTriangles)
            {
                // Step 5: Find boundary of the polygonal hole
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
                // Step 6: Remove bad triangles from triangulation
                triangulation.Remove(badTriangle);
            }

            foreach (Edge edge in polygon)
            {
                // Step 7: Re-triangulate the polygonal hole
                triangulation.Add(new Triangle(edge.a, edge.b, point));
            }
        }

        // Step 8: Clean-Up, remove all triangles containing super-triangle vertices
        triangulation.RemoveAll(triangle => triangle.ContainsVertex(p1) || triangle.ContainsVertex(p2) || triangle.ContainsVertex(p3));

        // Step 9: Return triangulation
        return triangulation;
    }
}
