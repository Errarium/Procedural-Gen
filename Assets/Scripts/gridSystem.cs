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
    public float amp=5f;
    public float freq=10f;

    void Start()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                Vector3 pos = new Vector3(x - worldSize/2 , Mathf.PerlinNoise(x/freq, z/freq) * amp, z-worldSize/2);
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);

                room.transform.SetParent(this.transform);
            }
        }
    }
}
