using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivePlane : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;
    [SerializeField] private int amp = 5;
    [SerializeField] private int freq = 10;
    [SerializeField] private int radius = 5;
    private int planeOffset = 10;
    private Vector3 startPos = Vector3.zero;
    private int pMoveX => (int)(player.transform.position.x - startPos.x);
    private int pMoveZ => (int)(player.transform.position.z - startPos.z);
    private int pLocX => (int)MathF.Floor(player.transform.position.x / planeOffset) * planeOffset;
    private int pLocZ => (int)MathF.Floor(player.transform.position.z / planeOffset) * planeOffset;
    private Hashtable map = new Hashtable();

    private void Update()
    {
        if (startPos == Vector3.zero)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3 pos = new Vector3(x * planeOffset, 0, z * planeOffset);
                    GeneratePlains(pos);
                }
            }
        }
        if (HasPMoved())
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3 pos = new Vector3(x * planeOffset + pLocX, 0, z * planeOffset + pLocZ);
                    ActivatePlanes();
                    GeneratePlains(pos);
                }
            }
        }
    }

    private void GeneratePlains(Vector3 pos)
    {
        if (!map.Contains(pos))
        {
            GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
            _plane.transform.SetParent(this.transform);

            // Deform the plane using Perlin noise for heightmap
            MeshFilter meshFilter = _plane.GetComponent<MeshFilter>();
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

            map.Add(pos, _plane);
        }
    }

    private void ActivatePlanes()
    {
        foreach (GameObject _plane in map.Values)
        {
            bool beyondRadius = Mathf.Abs(_plane.transform.position.x - pLocX) > radius * planeOffset || Mathf.Abs(_plane.transform.position.z - pLocZ) > radius * planeOffset;
            _plane.SetActive(!beyondRadius);
        }
    }

    private bool HasPMoved()
    {
        if (Mathf.Abs(pMoveX) >= planeOffset || Mathf.Abs(pMoveZ) >= planeOffset)
        {
            return true;
        }
        return false;
    }
}
