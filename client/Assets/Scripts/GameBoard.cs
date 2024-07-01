using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using shared;

/**
 * Simple checkerbased gameboard that could be reused for other 1 piece based 2 player games.
 */
public class GameBoard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private List<Sprite> _cellStateSprites = new List<Sprite>();

    public WallManager wallManager;

    private List<Image> _cells = new List<Image>();

    private List<int> adjacentCells = new List<int>();

    //called when we click on a cell
    public event Action<int> OnCellClicked = delegate { };

    public event Action<int> OnWallPlaced = delegate { };

    private int playerNo;

    private Player player;

    int playerCell;

    [SerializeField]
    Player playerPrefab;

    private void Awake()
    {
        foreach (Transform child in transform) _cells.Add(child.GetComponent<Image>());
        Debug.Log(_cells.Count + " cells on the board found.");

        wallManager.GetGameBoard(this);

    }

    /**
     * Updates the whole board view to reflect the given board data.
     */
    public void SetBoardData (TicTacToeBoardData pBoardData)
    {
        adjacentCells.Clear();
        //pass the whole board to our view
        int[] boardData = pBoardData.board;

        int cellsToSet = Mathf.Min(boardData.Length, _cells.Count);

        for (int i = 0; i < cellsToSet; i++)
        {
            _cells[i].sprite = _cellStateSprites[boardData[i]];

            //add correctly colored adjacents
            if (boardData[i] == 3 && playerNo ==1)
            {
                adjacentCells.Add(i);
            }
            if (boardData[i] == 4 && playerNo == 2) 
            {
                adjacentCells.Add(i);
            }

            //only show the players adjacents
            if (boardData[i] == 3 && playerNo == 2)
            {
                _cells[i].sprite = _cellStateSprites[0];
            }
            if (boardData[i] == 4 && playerNo == 1)
            {
                _cells[i].sprite = _cellStateSprites[0];
            }

            if (boardData[i] == playerNo) 
            {
                if (!player)
                {
                    player = Instantiate(playerPrefab, _cells[i].transform.position, Quaternion.identity);
                    player.GetGameBoard(this);
                }
                else
                {
                    player.transform.position = _cells[i].transform.position;
                }
                playerCell = i;
            }
        }
        calculateCompromizedCells();
        wallManager.SetWalls(pBoardData);

    }

    /**
     * Automatically called by the Unity UI system since we have implemented the IPointerClickHandler interface
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        //check whether we clicked on a cell
        int clickedCellIndex = _cells.IndexOf(eventData.pointerPressRaycast.gameObject.GetComponent<Image>());
        Debug.Log("Clicked cell index:" + clickedCellIndex);
        //and if we actually clicked on a cell, trigger our event
        if (clickedCellIndex > -1 && adjacentCells.Contains(clickedCellIndex))
        {
            OnCellClicked(clickedCellIndex);

        }
    }

    public void SetPlayer(int pPlayer)
    {
        playerNo = pPlayer;
    }

    public void PlaceWall(int pWallIndex)
    {
        OnWallPlaced(pWallIndex);
    }
    List<int> compromizedDirections = new List<int>();
    void calculateCompromizedCells()
    {
        if (player) compromizedDirections = player.GetCompromizedDirectionList();

        if(compromizedDirections.Count>0)
        Debug.Log("Comperomized dir" + compromizedDirections[0]);

        for (int i =0; i< compromizedDirections.Count; i++)
        {
            int cell = 0;
            if (compromizedDirections[i] == 0)
            {
                cell = playerCell - 9;
            }
            else if (compromizedDirections[i] == 1)
            {
                cell = playerCell + 1;
            }
            else if (compromizedDirections[i] == 2) 
            {
                cell = playerCell + 9;
            }
            else if (compromizedDirections[i] == 3)
            {
                cell = playerCell - 1;
            }
            adjacentCells.Remove(cell);
            _cells[cell].sprite = _cellStateSprites[0];

            Debug.Log("cell index: " + cell);
            
        }
    
    }

    public void CalculateCompromizedCell(int dir, bool isCompro)
    {
        if (player) compromizedDirections = player.GetCompromizedDirectionList();

        int cell = 0;
        if (dir == 0)
        {
            cell = playerCell - 9;
        }
        else if (dir == 1)
        {
            cell = playerCell + 1;
        }
        else if (dir == 2)
        {
            cell = playerCell + 9;
        }
        else if (dir == 3)
        {
            cell = playerCell - 1;
        }


        if (isCompro)
        {
            adjacentCells.Remove(cell);
            _cells[cell].sprite = _cellStateSprites[0];
        }
        else{
            if (cell > 0 && cell < 81)
            {
                adjacentCells.Add(cell);
            }
        }
       

    }




}
