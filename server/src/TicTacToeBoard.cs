using shared;
using System;
using System.Collections.Generic;

namespace server
{
/// <summary>
/// The actual board we play on
/// </summary>
    public class TicTacToeBoard
    {
        private TicTacToeBoardData _board = new TicTacToeBoardData();

        int[] playerPos = new int[2] { -1,-1};

        List<int> adjacents1 = new List<int>();
        List<int> adjacents2 = new List<int>();

        public void MakeMove (int pMove, int pPlayer)
        {
            if (playerPos[pPlayer - 1] > -1)
            {
                //remove playre from old pos
                _board.board[playerPos[pPlayer - 1]] = 0;
                //remove old adjacent
                removeAdjacents(pPlayer);
            }

            playerPos[pPlayer-1] = pMove;
            _board.board[pMove] = pPlayer;

            int columns = 9;
            int row = pMove / columns;
            int column = pMove % columns;
            Log.LogInfo($"Player {pPlayer} made a move in cell ({column},{row})", this);

            checkWin();
        }

        public void PlaceWall(int pWall, int pPlayer)
        {
            removeAdjacents(pPlayer);

            if (pWall <= 63) //vertical wall
            {
                _board.verticalWalls[pWall] = 1;
            }
            else  // horizontal
            {
                _board.horizontalWalls[pWall-63] = 1;
            }
            Log.LogInfo($"Player {pPlayer} placed a wall in cell ({pWall})", this);

            
        }

        public void FindAdjacents(int pPlayer)
        {
            adjacents1.Clear();
            adjacents2.Clear();
            addAdjacentCell(playerPos[pPlayer - 1] - 1, pPlayer);
            addAdjacentCell(playerPos[pPlayer - 1] + 1, pPlayer);
            addAdjacentCell(playerPos[pPlayer - 1] - 9, pPlayer);
            addAdjacentCell(playerPos[pPlayer - 1] + 9, pPlayer);
        }

        void addAdjacentCell(int i, int pPlayer)
        {
            if (i > -1 && i < 81)
            {
                if (_board.board[i] == 0)
                {
                    _board.board[i] = pPlayer + 2;

                    if (pPlayer == 1)
                    {
                        adjacents1.Add(i);
                    }
                    else
                    {
                        adjacents2.Add(i);
                    }
                }

               
            }
        }

        void removeAdjacents(int pPlayer)
        {
            if (pPlayer == 1)
            {
                foreach (int i in adjacents1)
                {
                    _board.board[i] = 0;
                }
            }
            else
            {
                foreach (int i in adjacents2)
                {
                    _board.board[i] = 0;
                }
            }
        }

        private void checkWin()
        {
            if (playerPos[1] >=0 && playerPos[1] < 9) 
            {
                _board.SetWinner(1);
            }
            if (playerPos[0] >= 72 && playerPos[0] < 81)
            {
                _board.SetWinner(0);
            }


        }

        /**
         * Return the inner board data state so we can send it to a client.
         */
        public TicTacToeBoardData GetBoardData()
        {
            return _board;
        }
    }
}
