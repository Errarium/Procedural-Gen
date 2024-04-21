using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSystem : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;
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

                    if (!map.Contains(pos))
                    {
                        GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
                        _plane.transform.SetParent(this.transform);
                        map.Add(pos, _plane);
                    }
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

                    if (!map.Contains(pos))
                    {
                        GameObject _plane = Instantiate(plane, pos, Quaternion.identity);
                        _plane.transform.SetParent(this.transform);
                        map.Add(pos, _plane);
                    }
                }
            }
            // Remove planes outside the updated player's movement range
            List<Vector3> planesToRemove = new List<Vector3>();
            foreach (Vector3 pos in map.Keys)
            {
                if (Mathf.Abs(pos.x - pLocX) > radius * planeOffset || Mathf.Abs(pos.z - pLocZ) > radius * planeOffset)
                {
                    planesToRemove.Add(pos);
                }
            }

            foreach (Vector3 pos in planesToRemove)
            {
                GameObject planeToRemove = (GameObject)map[pos];
                Destroy(planeToRemove);
                map.Remove(pos);
            }
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
