using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("0 - up, 1 - right, 2 - down, 3 - left")]
    [SerializeField]
    List<PlayerArm> playerArms = new List<PlayerArm>();


    List<int>compromisedDirections = new List<int>();

    GameBoard board;

    private void Start()
    {
        playerArms = GetComponentsInChildren<PlayerArm>().ToList();
        CheckDirections();

    }

    public void CheckDirections()
    {
        compromisedDirections.Clear();

        for (int i =0; i< playerArms.Count; i++) 
        {
            if (playerArms[i].GetCompromisedState())
            {
                compromisedDirections.Add(i);
                Debug.Log("Added compromized " + i);
                //update the board
                board.CalculateCompromizedCell(i, true);
            }
        }
    }

    public void RemoveCompromized()
    {
        for (int i = 0; i < playerArms.Count; i++)
        {
            if (!playerArms[i].GetCompromisedState())
            {
                compromisedDirections.Remove(i);
                Debug.Log("Removed compromized " + i);
                board.CalculateCompromizedCell(i, false);
            }
        }
    }

    public List<int> GetCompromizedDirectionList()
    {
        return compromisedDirections;
    }

    public void GetGameBoard(GameBoard gameBoard)
    {
        board = gameBoard;
    }
}
