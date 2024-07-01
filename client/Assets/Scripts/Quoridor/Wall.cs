using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wall : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;


    public bool vertical = false;

    public bool CheckCollisions;

    public Image image { private set;  get; }
    private void Start()
    {
        image = GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        image.raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        if (vertical) { transform.Translate(0, -20, 0); }
        else { transform.Translate(20, 0, 0); }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        transform.position = parentAfterDrag.position;
        if (parentAfterDrag.GetComponent<WallSpawner>())
        {
            image.raycastTarget = true;
        }
    }

    int colliders =0;


    //block impossible positions
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CheckCollisions && collision.transform.GetComponent<WallSlot>() && colliders < 3)
        {
            collision.transform.GetComponent<WallSlot>().SetWallPlaced(true);
            colliders++;
        }
    }

}
