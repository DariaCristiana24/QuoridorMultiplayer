using shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WallManager : MonoBehaviour
{
    [SerializeField]
    Transform verticalWallHolder; 
    [SerializeField]
    Transform horizontalWallHolder;
    [SerializeField]
    GameObject wallPrefabVertical; 
    [SerializeField]
    GameObject wallPrefabHorizontal;

    List<Transform> verticalWalls = new List<Transform>();
    List<Transform> horizontalWalls = new List<Transform>();

    GameBoard board;

    [SerializeField]
    List<WallSpawner> wallSpawners = new List<WallSpawner>();

    [SerializeField]
    int maxWalls = 10;

    int currentWalls;

    private void Awake()
    {
        foreach (Transform child in verticalWallHolder) verticalWalls.Add(child.GetComponent<Transform>());
        foreach (Transform child in horizontalWallHolder) horizontalWalls.Add(child.GetComponent<Transform>());
    }

    public void SetWalls(TicTacToeBoardData pBoardData)
    {

        int[] vWalls = pBoardData.verticalWalls; 
        int[] hWalls = pBoardData.horizontalWalls;
        for (int i = 0; i < verticalWalls.Count; i++)
        {
            
            if (vWalls[i] != 0 && verticalWalls[i].childCount == 0)
            {
                GameObject wall = Instantiate(wallPrefabVertical, verticalWalls[i]);
                wall.GetComponent<Image>().raycastTarget = false; // dont allow moving
            }
            
        }
        for (int i = 0; i < horizontalWalls.Count; i++)
        {
            if (hWalls[i] != 0 && horizontalWalls[i].childCount ==0)
            {
                GameObject wall = Instantiate(wallPrefabHorizontal, horizontalWalls[i]);
                wall.GetComponent<Image>().raycastTarget = false; // dont allow moving
            }
            


        }
    }

    public void GetGameBoard(GameBoard gameBoard)
    {
        board = gameBoard;
    }

    public void PlaceWall(int pWallIndex)
    {
        board.PlaceWall(pWallIndex);

        //check for max walls
        currentWalls++;

        if(currentWalls >= maxWalls)
        {
            foreach (WallSpawner wallSpawner in wallSpawners)
            {
                wallSpawner.StopSpawning();
            }
        }
    }

    public void BlockWallMovement()
    {
        foreach (WallSpawner wallSpawner in wallSpawners)
        {
            wallSpawner.UnBlockPickingUp();
        }
    }

    public void UnblockWallMovement()
    {
        foreach (WallSpawner wallSpawner in wallSpawners)
        {
            wallSpawner.BlockPickingUp();
        }
    }


}
