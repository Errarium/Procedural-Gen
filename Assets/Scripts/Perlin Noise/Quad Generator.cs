using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadGenerator : MonoBehaviour
{
    public GameObject quad;
    public GameObject player;
    [SerializeField] private int amp = 5;
    [SerializeField] private int freq = 10;
    [SerializeField] private int radius = 5;
    private int quadOffset = 10;
    private Vector3 startPos = Vector3.zero;
    private int pMoveX => (int)(player.transform.position.x - startPos.x);
    private int pMoveZ => (int)(player.transform.position.z - startPos.z);
    private int pLocX => (int)Mathf.Floor(player.transform.position.x / quadOffset) * quadOffset;
    private int pLocZ => (int)Mathf.Floor(player.transform.position.z / quadOffset) * quadOffset;
    private Hashtable map = new Hashtable();

    private void Update()
    {
        if (startPos == Vector3.zero)
        {
            GenerateQuadsAroundPlayer();
        }
        else if (HasPlayerMoved())
        {
            GenerateQuadsAroundPlayer();
        }
    }

    private void GenerateQuadsAroundPlayer()
    {
        ClearInactiveQuads();
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector3 pos = new Vector3(x * quadOffset + pLocX, 0, z * quadOffset + pLocZ);
                GenerateQuad(pos);
            }
        }
        startPos = player.transform.position;
    }

    private void GenerateQuad(Vector3 pos)
    {
        if (!map.Contains(pos))
        {
            GameObject _quad = Instantiate(quad, pos, Quaternion.identity);
            _quad.transform.SetParent(this.transform);

            // Deform the quad using Perlin noise for heightmap
            MeshFilter meshFilter = _quad.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;

                for (int i = 0; i < vertices.Length; i++)
                {
                    float rise = Mathf.PerlinNoise((pos.x + vertices[i].x) / freq, (pos.z + vertices[i].z) / freq) * amp;
                    vertices[i].y = rise;
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }

            // Deform the collider
            MeshCollider meshCollider = _quad.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = meshFilter.mesh;
            }

            map.Add(pos, _quad);
        }
    }

    private void ClearInactiveQuads()
    {
        List<Vector3> positionsToRemove = new List<Vector3>();
        foreach (Vector3 pos in map.Keys)
        {
            GameObject _quad = (GameObject)map[pos];
            bool beyondRadius = Mathf.Abs(_quad.transform.position.x - pLocX) > radius * quadOffset || Mathf.Abs(_quad.transform.position.z - pLocZ) > radius * quadOffset;
            if (beyondRadius)
            {
                Destroy(_quad);
                positionsToRemove.Add(pos);
            }
        }
        foreach (Vector3 pos in positionsToRemove)
        {
            map.Remove(pos);
        }
    }

    private bool HasPlayerMoved()
    {
        return Mathf.Abs(pMoveX) >= quadOffset || Mathf.Abs(pMoveZ) >= quadOffset;
    }
}
