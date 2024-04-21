using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class Raymarching : MonoBehaviour
{
        [SerializeField] private int worldSize = 10;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxLenght = 3;
    [SerializeField] private int maxHeight = 3;
    [SerializeField] private int maxDepth = 3;
    [SerializeField] private float minSpace = 1f;
    [SerializeField] private int roomLayer = 9;
    private Hashtable location = new Hashtable();
    private Hashtable map = new Hashtable();
    private Vector3 startSize = new Vector3(2, 1, 2);
    private Vector3 startPos = new Vector3(0, 1, 0);

    void Start()
    {
        PlaceRooms();
        
        // Debug.Log("Number of elements in location: " + location.Count);
        
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Contract();
        }
    }
    bool Collides(Vector3 pos, Vector3 roomSize)
    {
        foreach (DictionaryEntry area in location)
        {
            Vector3 areaPos = (Vector3)area.Key;
            Vector3 areaSize = (Vector3)area.Value;

            float minDistanceX = (roomSize.x * 0.5f) + (areaSize.x * 0.5f) + minSpace;
            float minDistanceZ = (roomSize.z * 0.5f) + (areaSize.z * 0.5f) + minSpace;

            if (Mathf.Abs(pos.x - areaPos.x) < minDistanceX && Mathf.Abs(pos.z - areaPos.z) < minDistanceZ)
            {
                return true;
            }
        }
        return false;
    }
    void PlaceRooms()
    {
        // for (int i = 0; i < Mathf.Round(worldSize * worldSize / math.sqrt((maxLenght+1) * (maxDepth+1))+1); i++)
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                GameObject start = Instantiate(prefab, startPos, Quaternion.identity);
                start.transform.SetParent(this.transform);

                start.transform.localScale = startSize;
                start.layer = roomLayer;

                location.Add(startPos, startSize);
                map.Add(startPos, start);
            }else{
                Vector3 roomSize = new Vector3(Mathf.Round(UnityEngine.Random.Range(1,maxLenght+1)), Mathf.Round(UnityEngine.Random.Range(1,maxHeight)), Mathf.Round(UnityEngine.Random.Range(1,maxDepth+1)));
                Vector3 pos = new Vector3(Mathf.Round(UnityEngine.Random.Range(-worldSize + roomSize.x * 0.5f, worldSize - roomSize.x * 0.5f)), 1, Mathf.Round(UnityEngine.Random.Range(-worldSize + roomSize.z * 0.5f, worldSize - roomSize.z * 0.5f)));
                
                if(Collides(pos, roomSize))
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
    Vector3 Raymarch(Vector3 origin)
    {
        // 2. let the room raycast towards the starting room (startPos),
        // the raycast will hit a position ranging from the current room to the starting room.

        Vector3 direction = (startPos - origin).normalized;

        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, math.sqrt(worldSize * worldSize + worldSize * worldSize), roomLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(origin, hit.point.normalized, Color.green, 60.0f);
            Vector3 target = new Vector3(Mathf.Round(-hit.point.normalized.x), 1, Mathf.Round(-hit.point.normalized.z));

            // 3. calculate the closest position by finding the vector with 
            // the opposite direction from the raycast and the lowest magnitude, 
            // from the raycast hit to a position where the current room fits without colliding with any other room while being surrounded by minSpace.
            if(target.x > 0)
                target.x += minSpace;
            if(target.x < 0)
            target.x -= minSpace;

            if(target.z > 0)
            target.z += minSpace;
            if(target.z < 0)
            target.z -= minSpace;

            Debug.DrawLine(hit.point.normalized, target, Color.blue, 60.0f);

            return target;
        }else{
            Debug.DrawLine(origin, startPos, Color.red, 60.0f);
            Debug.LogWarning("Raycast " + origin + "did not hit anything.");
            return origin; // Return original position if no hit
        }
    }
    void Contract()
    {
        // 1. order all rooms in location (hashtable) by their proximity to the starting room (startPos)
        
        List<KeyValuePair<Vector3, Vector3>> sortedRooms = location.Cast<DictionaryEntry>().OrderBy(entry => Vector3.Distance((Vector3)entry.Key, startPos)).Select(entry => new KeyValuePair<Vector3, Vector3>((Vector3)entry.Key, (Vector3)entry.Value)).ToList();

        // 5. repeat for every other room in locations (hashtable)
        foreach (var domain in sortedRooms)
        {
            Vector3 origin = domain.Key;

            if(origin == startPos)
                continue;

            Vector3 target = Raymarch(origin);

            // 4. move the room, then, remove it from the list
            GameObject room = (GameObject)map[origin];
            room.transform.position = target;

            Debug.Log(origin + " hit tagret at " + target);

            // if(origin != target)
            //     sortedRooms.Remove(domain);

            // room.layer = roomLayer;
        }
    }
}
