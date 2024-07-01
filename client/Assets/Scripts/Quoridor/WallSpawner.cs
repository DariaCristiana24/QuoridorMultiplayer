using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallSpawner : MonoBehaviour
{
    [SerializeField]
    Wall wallPrefab;

    bool maxWalls;

    void Update()
    {
        if (transform.childCount == 0 && !maxWalls)
        {
            Instantiate(wallPrefab, transform);
        }
    }

    public void StopSpawning()
    {
        maxWalls = true;

        Wall wall = GetComponentInChildren<Wall>();
        Destroy(wall.gameObject);
    }

    public void BlockPickingUp()
    {
        if(GetComponentInChildren<Wall>().image) GetComponentInChildren<Wall>().image.raycastTarget = false;
    }
    public void UnBlockPickingUp()
    {
        if(GetComponentInChildren<Wall>().image) GetComponentInChildren<Wall>().image.raycastTarget = true; 
    }
}
