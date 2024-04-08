using UnityEngine;

public class SimpleMeshGenerator : MonoBehaviour
{
    public int WorldX;
    public int WorldZ;
    [SerializeField] private Mesh mesh;
    private int[] triangles;
    private Vector3[] vertices;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        vertices = new Vector3[(WorldX + 1) * (WorldZ + 1)];
        triangles = new int[WorldX * WorldZ * 6];

        for (int i = 0, x = 0; x <= WorldX; x++)
        {
            for (int z = 0; z <= WorldZ; z++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        int tris = 0;
        int verts = 0;

        for (int x = 0; x < WorldX; x++)
        {
            for (int z = 0; z < WorldZ; z++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + WorldZ + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + WorldZ + 1;
                triangles[tris + 5] = verts + WorldZ + 2;

                tris += 6;
                verts++;
            }
        }
    }

    private void GenerateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
