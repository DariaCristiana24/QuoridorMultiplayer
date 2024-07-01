using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WallSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    bool vertical = false;

    Image image;

    bool wallPlaced = false;

    WallManager wallManager;


    private void Start()
    {
        image = GetComponent<Image>();
        wallManager = FindObjectOfType<WallManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (!wallPlaced)
        {
            GameObject dropped = eventData.pointerDrag;
            Wall wall = dropped.GetComponent<Wall>();

            if (wall.vertical == vertical)
            {
                wall.parentAfterDrag = transform;
                wall.image.raycastTarget = false;
                image.raycastTarget = false;

                wallPlaced = true;
                wall.CheckCollisions = true;

                //tell manager a wall was placed
                if (vertical)
                {
                    wallManager.PlaceWall(transform.GetSiblingIndex());
                }
                else
                {
                    wallManager.PlaceWall(transform.GetSiblingIndex() + 63);
                }

            }
            else
            {
                wall.image.raycastTarget = true;
            }
        }
    }


    public void SetWallPlaced(bool pWallPlaced)
    {
        wallPlaced = pWallPlaced;
    }


}
