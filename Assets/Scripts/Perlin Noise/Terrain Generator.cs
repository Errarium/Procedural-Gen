using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public GameObject player;
    [SerializeField] private int amp = 5;
    [SerializeField] private int freq = 10;
    [SerializeField] private int radius = 5;
    private int terrainSize = 0;
    private Vector3 startPos = Vector3.zero;
    private int pMoveX => (int)(player.transform.position.x - startPos.x);
    private int pMoveZ => (int)(player.transform.position.z - startPos.z);
    private int pLocX => (int)Mathf.Floor(player.transform.position.x / terrainSize) * terrainSize;
    private int pLocZ => (int)Mathf.Floor(player.transform.position.z / terrainSize) * terrainSize;

    private void Start()
    {
        terrainSize = (int)terrain.terrainData.size.x;
        GenerateTerrainAroundPlayer();
    }

    private void Update()
    {
        if (HasPlayerMoved())
        {
            GenerateTerrainAroundPlayer();
        }
    }

    private void GenerateTerrainAroundPlayer()
    {
        int terrainOffset = terrainSize * radius;
        int minX = pLocX - terrainOffset;
        int maxX = pLocX + terrainOffset;
        int minZ = pLocZ - terrainOffset;
        int maxZ = pLocZ + terrainOffset;

        TerrainData terrainData = terrain.terrainData;

        for (int x = minX; x <= maxX; x += terrainSize)
        {
            for (int z = minZ; z <= maxZ; z += terrainSize)
            {
                Vector3 pos = new Vector3(x, 0, z);
                float[,] heights = terrainData.GetHeights(x, z, terrainSize, terrainSize);

                for (int i = 0; i < terrainSize; i++)
                {
                    for (int j = 0; j < terrainSize; j++)
                    {
                        float rise = Mathf.PerlinNoise((pos.x + i) / freq, (pos.z + j) / freq) * amp;
                        heights[i, j] = rise;
                    }
                }

                terrainData.SetHeights(x, z, heights);
            }
        }

        startPos = player.transform.position;
    }

    private bool HasPlayerMoved()
    {
        return Mathf.Abs(pMoveX) >= terrainSize || Mathf.Abs(pMoveZ) >= terrainSize;
    }
}
