using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Force : MonoBehaviour
{
    [SerializeField] private int worldSize = 10;
    [SerializeField] private int worldScale = 1;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    [SerializeField] private float minSpace = 1f;
    private Hashtable location = new Hashtable();
    private Hashtable map = new Hashtable();
    private Hashtable rooms = new Hashtable();
    private Vector3 startSize = new Vector3(2, 1, 2);
    private Vector3 startPos = new Vector3(0, 1, 0);

    void Start()
    {
        PlaceRooms();

        // Freeze the starting room
        GameObject startRoom = (GameObject)rooms[startPos];
        Rigidbody startRoomRigidbody = startRoom.GetComponent<Rigidbody>();
        startRoomRigidbody.isKinematic = true;

    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Contract");
            Contract();
        }
    }
    bool Collides(Vector3 pos, Vector3 roomSize, Hashtable collection)
    {
        foreach (DictionaryEntry area in collection)
        {
            Vector3 areaPos = (Vector3)area.Key;
            Vector3 areaSize = (Vector3)area.Value;

            float minDistanceX = (roomSize.x * 0.5f) + (areaSize.x * 0.5f) + minSpace * worldScale;
            float minDistanceZ = (roomSize.z * 0.5f) + (areaSize.z * 0.5f) + minSpace * worldScale;

            if (Mathf.Abs(pos.x - areaPos.x) < minDistanceX && Mathf.Abs(pos.z - areaPos.z) < minDistanceZ)
            {
                return true;
            }
        }
        return false;
    }
    void PlaceRooms()
    {
        for (int i = 0; i < Mathf.Round(worldSize * worldSize / math.sqrt((maxLenght+1) * (maxDepth+1))+1); i++)
        // for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                GameObject start = Instantiate(prefab, startPos, Quaternion.identity);
                start.transform.SetParent(this.transform);
                startSize = new Vector3(2 * worldScale, 1, 2 * worldScale);

                start.transform.localScale = startSize;

                location.Add(startPos, startSize);

                map.Add(startPos, startSize);
                rooms.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxLenght + 1) * worldScale) / 2)) * 2, 
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxHeight + 1) * worldScale) / 2)) * 2,  
                    Mathf.Round(Random.Range(Mathf.Round(worldScale / 2),Mathf.Round((maxDepth + 1) * worldScale) / 2)) * 2
                );
                Vector3 pos = new Vector3(
                    Mathf.Round(Random.Range(-worldSize * worldScale + roomSize.x * 0.5f, worldSize * worldScale - roomSize.x * 0.5f)), 
                    1, 
                    Mathf.Round(Random.Range(-worldSize * worldScale + roomSize.z * 0.5f, worldSize * worldScale - roomSize.z * 0.5f))
                    );
                
                if(Collides(pos, roomSize, location))
                {
                    i -= 1;
                    continue;
                }
                
                //need to pass on lenght, height and depth values
                GameObject room = Instantiate(prefab, pos, Quaternion.identity);
                room.transform.SetParent(this.transform);

                room.transform.localScale = new Vector3(roomSize.x, 1, roomSize.z);

                location.Add(pos, roomSize);
                rooms.Add(pos, room);
            }
        }
    }
    void Contract()
    {
        // Step 0: Remove starting room from locations
        location.Remove(startPos);

        // Step 4: Repeat for every room in locations
        while (location.Count > 0)
        {
            // Step 1: Find the room with the position closest to any position of a room in map
            Vector3 locRoom = new Vector3(worldSize * worldScale, worldSize * worldScale, worldSize * worldScale);
            Vector3 mapRoom = new Vector3(-worldSize * worldScale, -worldSize * worldScale, -worldSize * worldScale);
            Rigidbody roomRigidbody;

            foreach (Vector3 mapPos in map.Keys.Cast<Vector3>())
            {
                foreach (Vector3 locPos in location.Keys.Cast<Vector3>())
                {
                    if (Vector3.Distance(locPos, mapPos) < Vector3.Distance(locRoom, mapRoom))
                    {
                        locRoom = locPos;
                        mapRoom = mapPos;
                    }
                }
            } 
            Vector3 locSize = (Vector3)location[locRoom];

            foreach (Vector3 entry in rooms.Keys)
            {
                if(entry == locRoom)
                    Debug.Log("Match found");
            }
           
            GameObject room = (GameObject)rooms[locRoom];

            if (room != null)
            {
                Debug.LogError("Room exists");
            }else{
                Debug.LogError("Room does not exist");
            }
            roomRigidbody = room.GetComponent<Rigidbody>(); // this is line 132

            if (roomRigidbody == null)
            {
                Debug.LogError("Room prefab must have a Rigidbody component.");
                return;
            }

            // Calculate force direction towards the starting room
            Vector3 forceDirection = new Vector3(-locRoom.x, 0, -locRoom.z);

            // Apply a force to move the room
            float forceMagnitude = 2f; // Adjust this value as needed
            Vector3 force = forceDirection * forceMagnitude;
            Debug.Log(force);

            // Apply the force to the room's Rigidbody
            roomRigidbody.AddForce(force, ForceMode.Force);

            // Check for collisions after applying force
            Vector3 newPosition = new Vector3(Mathf.Round(locRoom.x + force.x), Mathf.Round(locRoom.y + force.y), Mathf.Round(locRoom.z + force.z));
            if (!Collides(newPosition, locSize, map))
            {
                // Room doesn't collide, update its position
                location.Remove(locRoom);
                location.Add(newPosition, locSize);
                map.Remove(locRoom);
                map.Add(locRoom, locSize);
            }else{
                // If there's a collision, stop the room's Rigidbody
                location.Remove(locRoom);
                map.Remove(locRoom);
                map.Add(newPosition, locSize);

                roomRigidbody.isKinematic = true;
                room.transform.SetLocalPositionAndRotation(new Vector3(Mathf.Round(locRoom.x), locRoom.y, Mathf.Round(locRoom.z)), quaternion.identity);
            }
        }
    }
}
