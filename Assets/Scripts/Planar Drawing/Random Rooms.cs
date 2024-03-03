using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class RandomRooms : MonoBehaviour
{
    [SerializeField] private int worldSize = 10;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    [SerializeField] private float minSpace = 1f;
    private Hashtable location = new Hashtable();
    private Hashtable map = new Hashtable();
    private Vector3 startSize = new Vector3(2, 1, 2);
    private Vector3 startPos = new Vector3(0, 1, 0);

    void Start()
    {
        PlaceRooms();
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Contract");
            Contract();
        }
    }
    bool collides(Vector3 pos, Vector3 roomSize)
    {
        foreach (DictionaryEntry area in location)
        {
            Vector3 areaPos = (Vector3)area.Key;
            Vector3 areaSize = (Vector3)area.Value;

            float minDistanceX = (roomSize.x * 0.5f) + (areaSize.x * 0.5f) + minSpace;
            float minDistanceZ = (roomSize.z * 0.5f) + (areaSize.z * 0.5f) + minSpace;

            if (Mathf.Abs(pos.x - areaPos.x) < minDistanceX &&
                Mathf.Abs(pos.z - areaPos.z) < minDistanceZ)
            {
                return true;
            }
        }
        return false;
    }
    void PlaceRooms()
    {
        for (int i = 0; i < Mathf.Round(worldSize * worldSize / math.sqrt(maxLenght * maxDepth)+1); i++)
        {
            if (i == 0)
            {
                GameObject start = Instantiate(prefab, startPos, Quaternion.identity);
                start.transform.SetParent(this.transform);

                start.transform.localScale = startSize;

                location.Add(startPos, startSize);
                map.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(Mathf.Round(UnityEngine.Random.Range(1,maxLenght)), Mathf.Round(UnityEngine.Random.Range(1,maxHeight)), Mathf.Round(UnityEngine.Random.Range(1,maxDepth)));
                Vector3 pos = new Vector3(Mathf.Round(UnityEngine.Random.Range(-worldSize + roomSize.x * 0.5f, worldSize - roomSize.x * 0.5f)), 1, Mathf.Round(UnityEngine.Random.Range(-worldSize + roomSize.z * 0.5f, worldSize - roomSize.z * 0.5f)));
                
                if(collides(pos, roomSize))
                {
                    i -= 1;
                    continue;
                }
                
                //need to pass on lenght, height and depth values
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);
                room.transform.SetParent(this.transform);

                room.transform.localScale = new Vector3(roomSize.x, 1, roomSize.z);

                location.Add(pos, roomSize);
                map.Add(pos, room);
            }
        }
    }
    void Contract()
    {
         // Sorting rooms based on their distance to the starting room
        List<Vector3> sortedRooms = new List<Vector3>(location.Keys.Cast<Vector3>());
        sortedRooms.Sort((a, b) => Vector3.Distance(startPos, a).CompareTo(Vector3.Distance(startPos, b)));

        foreach (Vector3 roomPos in sortedRooms)
        {
            // Skip the starting room
            if (roomPos == startPos)
                continue;

            Vector3 roomSize = (Vector3)location[roomPos];

            // Calculate force direction towards the starting room
            Vector3 forceDirection = Vector3.Normalize(startPos - roomPos);

            // Apply a force to move the room
            float forceMagnitude = 0.1f; // Adjust this value as needed
            Vector3 force = forceDirection * forceMagnitude;

            // Move the room and check for collisions
            Vector3 newPosition = roomPos + force;
            if (!collides(newPosition, roomSize))
            {
                // Room doesn't collide, update its position
                location.Remove(roomPos);
                location.Add(newPosition, roomSize);
                map[newPosition] = map[roomPos];
                map.Remove(roomPos);
            }
        }
    }
}
