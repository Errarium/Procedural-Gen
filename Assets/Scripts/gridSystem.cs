using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridSystem : MonoBehaviour
{
    [SerializeField] private int sizeX = 10;
    [SerializeField] private int sizeZ = 10;
    [SerializeField] private GameObject room;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                Instantiate(room, new Vector3(i * 2f, 0, j * 2f), Quaternion.identity);
            }
        }
    }

}
