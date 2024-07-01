using shared;
using System;
using System.Runtime.Remoting.Messaging;

namespace server
{
	/**
	 * This room runs a single Game (at a time). 
	 * 
	 * The 'Game' is very simple at the moment:
	 *	- all client moves are broadcasted to all clients
	 *	
	 * The game has no end yet (that is up to you), in other words:
	 * all players that are added to this room, stay in here indefinitely.
	 */
	class GameRoom : Room
	{
		public bool IsGameInPlay { get; private set; }

		//wraps the board to play on...
		private TicTacToeBoard _board = new TicTacToeBoard();

		TcpMessageChannel player1;
		TcpMessageChannel player2;




        public GameRoom(TCPGameServer pOwner) : base(pOwner)
		{
		}

		public void StartGame (TcpMessageChannel pPlayer1, TcpMessageChannel pPlayer2)
		{
			if (IsGameInPlay) throw new Exception("This was not supposed to happen");

			IsGameInPlay = true;

			addMember(pPlayer1);
			addMember(pPlayer2);

            player1 = pPlayer1;
			player2 = pPlayer2;

			//tell client who they are
			sendPlayerNo(pPlayer1,1);
            sendPlayerNo(pPlayer2,2);

            sendPlayerInfo(pPlayer1, pPlayer2);

            sendStartPos();
        }

        private void sendPlayerNo(TcpMessageChannel pPlayer, int i)
        {
            PlayerNo playerNo = new PlayerNo();
            playerNo.player = i;
			sendToOne(playerNo, pPlayer);
			
        }
        private void sendStartPos()
		{
            _board.MakeMove(4, 1);
            MakeMoveResult makeMoveResult = new MakeMoveResult();
            makeMoveResult.whoMadeTheMove = 1;
            makeMoveResult.boardData = _board.GetBoardData();
            _board.FindAdjacents(1);
            sendToAll(makeMoveResult);

            _board.MakeMove(76, 2);	
            makeMoveResult = new MakeMoveResult();
            makeMoveResult.whoMadeTheMove = 2;
            makeMoveResult.boardData = _board.GetBoardData();
            sendToAll(makeMoveResult);

        }

		protected override void addMember(TcpMessageChannel pMember)
		{
			base.addMember(pMember);

			//notify client he has joined a game room 
            RoomJoinedEvent roomJoinedEvent = new RoomJoinedEvent();
            roomJoinedEvent.room = RoomJoinedEvent.Room.GAME_ROOM;
            pMember.SendMessage(roomJoinedEvent);
        }

		public override void Update()
		{
			int oldMemberCount = memberCount;
			base.Update();
			int newMemberCount = memberCount;

			if (oldMemberCount != newMemberCount)
			{
				Log.LogInfo("People left the game...", this);
			}
		}

		protected override void handleNetworkMessage(ASerializable pMessage, TcpMessageChannel pSender)
		{
			if (pMessage is MakeMoveRequest)
			{
                handleMakeMoveRequest(pMessage as MakeMoveRequest, pSender);
            }
            if (pMessage is PlaceAWallRequest)
            {
                handlePlaceAWallRequest(pMessage as PlaceAWallRequest, pSender);
            }
        }

		private void handleMakeMoveRequest(MakeMoveRequest pMessage, TcpMessageChannel pSender)
		{
			int playerID = indexOfMember(pSender) + 1;
            _board.MakeMove(pMessage.move, playerID);

			if (playerID == 1)
			{
				_board.FindAdjacents(2);
			}
			else
			{
                _board.FindAdjacents(1);
            }

			MakeMoveResult makeMoveResult = new MakeMoveResult();
			makeMoveResult.whoMadeTheMove = playerID;
			makeMoveResult.boardData = _board.GetBoardData();
			sendToAll(makeMoveResult);
			checkForWin();

        }

		private void handlePlaceAWallRequest(PlaceAWallRequest pMessage, TcpMessageChannel pSender)
		{
            int playerID = indexOfMember(pSender) + 1;

            _board.PlaceWall(pMessage.wall, playerID);

            if (playerID == 1)
            {
                _board.FindAdjacents(2);
            }
            else
            {
                _board.FindAdjacents(1);
            }

            PlaceAWallResult placeAWallResult = new PlaceAWallResult();
            placeAWallResult.whoMadeTheMove = playerID;
            placeAWallResult.boardData = _board.GetBoardData();
            sendToAll(placeAWallResult);

        }

        private void sendPlayerInfo(TcpMessageChannel pPlayer1, TcpMessageChannel pPlayer2)
		{
            //send clients name
            NewPlayersInGameRequest newPlayersInGameRequest = new NewPlayersInGameRequest();
            newPlayersInGameRequest.namePlayer1 = pPlayer1.playerInfo.Name;
            newPlayersInGameRequest.namePlayer2 = pPlayer2.playerInfo.Name;
            pPlayer1.SendMessage(newPlayersInGameRequest);
            pPlayer2.SendMessage(newPlayersInGameRequest);
        }

		private void checkForWin()
        {
            if (_board.GetBoardData().WhoHasWon() > -1)
            {
                PlayerWonGame playerWonGame = new PlayerWonGame();
                playerWonGame.winner = _board.GetBoardData().WhoHasWon();
                sendToAll(playerWonGame);
                gameWonProcedure(playerWonGame);
            }

        }

		private void gameWonProcedure(PlayerWonGame pPlayerWonGame)
		{
          	resetRoom(); 
            removeMember(player1);
            removeMember(player2);
            _server.GetLobbyRoom().AddMember(player1);
            _server.GetLobbyRoom().AddMember(player2);

            if (pPlayerWonGame.winner == 0)
            {
                _server.GetLobbyRoom().sendWinnginMessage(player1.playerInfo.Name + " (RED)");
            }
            else
            {
                _server.GetLobbyRoom().sendWinnginMessage(player2.playerInfo.Name + " (BLUE)");
            }


			_server.RemoveGameRoom(this);
        }

		private void resetRoom()
        {
            IsGameInPlay = false;
            _board = new TicTacToeBoard(); 
			MakeMoveResult makeMoveResult = new MakeMoveResult();
            makeMoveResult.boardData = _board.GetBoardData();
            sendToAll(makeMoveResult);
        }

	}
}
