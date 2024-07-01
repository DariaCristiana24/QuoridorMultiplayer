using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerArm : MonoBehaviour
{

    bool directionCompromized;

    Player player;

    private void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;

        if (GetComponentInParent<Player>())
        {
            player = GetComponentInParent<Player>();
        }
        else
        {
            Debug.LogError("No player found by arm");
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Wall>() && !directionCompromized)
        {
            directionCompromized = true;
            player.CheckDirections();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Wall>() && directionCompromized)
        {
            directionCompromized = false;
            player.RemoveCompromized();
        }
    }


    public bool GetCompromisedState()
    {
        return directionCompromized;
    }
}
