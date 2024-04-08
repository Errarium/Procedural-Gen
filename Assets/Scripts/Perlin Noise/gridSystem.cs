using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int worldSize = 25;
    [SerializeField] private GameObject prefab;
    public float amp = 5f;
    public float freq = 10f;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 playerStart;
    private Hashtable map = new Hashtable();

    public int playerPosX
    {
        get
        {
            return (int)(player.transform.position.x - playerStart.x);
        }
    }
    public int playerPosZ
    {
        get
        {
            return (int)(player.transform.position.z - playerStart.z);
        }
    }
    public int playerlocX
    {
        get
        {
            return (int)MathF.Floor(player.transform.position.x);
        }
    }
    public int playerLocZ
    {
        get
        {
            return (int)Mathf.Floor(player.transform.position.z);
        }
    }
    void Start()
    {
        for (int x = -worldSize; x < worldSize; x++)
        {
            for (int z = -worldSize; z < worldSize; z++)
            {
                Vector3 pos = new Vector3(x + playerStart.x, Mathf.PerlinNoise(x / freq, z / freq) * amp, z + playerStart.z);
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);

                room.transform.SetParent(this.transform);
                map.Add(pos, room);
            }
        }
    }

    void Update()
    {
        if (MathF.Abs(playerPosX) >= 1 || Mathf.Abs(playerPosZ) >= 1)
        {
            for (int x = -worldSize; x < worldSize; x++)
            {
                for (int z = -worldSize; z < worldSize; z++)
                {
                    Vector3 pos = new Vector3(x + playerlocX, Mathf.PerlinNoise((x + playerlocX) / freq, (z + playerLocZ) / freq) * amp, z + playerLocZ);

                    if (!map.ContainsKey(pos))
                    {
                        GameObject room = Instantiate(prefab, pos, Quaternion.identity);

                        room.transform.SetParent(this.transform);
                        map.Add(pos, room);
                    }
                }
            }
        }
    }
}
