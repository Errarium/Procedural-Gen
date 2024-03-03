using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] private int worldSize = 10;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int minRooms = 10;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    private Hashtable map = new Hashtable();

    void Start()
    {
        PlaceRooms();
        Graphing();
    }

    void PlaceRooms()
    {
        for (int i = 0; i < Random.Range(minRooms,maxRooms); i++)
        {
            int roomLenght = Random.Range(1,maxLenght);
            int roomHeight = Random.Range(1,maxHeight);
            int roomDepth = Random.Range(1,maxDepth);

            //need to check valid position
            Vector3 pos = new Vector3(Random.Range(-worldSize + roomLenght * 0.5f, worldSize - roomLenght * 0.5f), 1, Random.Range(-worldSize + roomDepth * 0.5f, worldSize - roomDepth * 0.5f));
            //need to pass on lenght, height and depth values
            GameObject room = Instantiate(prefab, pos, Quaternion.identity);

            room.transform.localScale = new Vector3(roomLenght, 1, roomDepth);

            room.transform.SetParent(this.transform);
            map.Add(pos, room);
            
        }
    }
    private void Graphing()
    {
        throw new System.NotImplementedException();
    }
}
