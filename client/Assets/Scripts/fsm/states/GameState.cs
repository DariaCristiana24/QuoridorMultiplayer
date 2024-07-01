using shared;
using System.Diagnostics;
using UnityEngine;

/**
 * This is where we 'play' a game.
 */
public class GameState : ApplicationStateWithView<GameView>
{

    private string player1Name = "Player 1";
    private string player2Name = "Player 2";

    private int player;

    public override void EnterState()
    {
        base.EnterState();
        
        view.gameBoard.OnCellClicked += _onCellClicked;
        view.gameBoard.OnWallPlaced += _placeAWall;

        ChangeTurnTo(1);
    }

    private void _onCellClicked(int pCellIndex)
    {
        MakeMoveRequest makeMoveRequest = new MakeMoveRequest();
        makeMoveRequest.move = pCellIndex;

        fsm.channel.SendMessage(makeMoveRequest);
       
    }

    private void _placeAWall(int pWallCellIndex)
    {
        PlaceAWallRequest placeAWallRequest = new PlaceAWallRequest();
        placeAWallRequest.wall = pWallCellIndex;

        fsm.channel.SendMessage(placeAWallRequest);
    }

    public override void ExitState()
    {
        base.ExitState();
        view.gameBoard.OnCellClicked -= _onCellClicked;
    }

    private void Update()
    {
        receiveAndProcessNetworkMessages();
    }

    protected override void handleNetworkMessage(ASerializable pMessage)
    {
        if (pMessage is MakeMoveResult)
        {
            handleMakeMoveResult(pMessage as MakeMoveResult);
        }
        else if (pMessage is NewPlayersInGameRequest)
        {
            handleNewPlayersInfo(pMessage as NewPlayersInGameRequest);
        }
        else if (pMessage is PlayerWonGame)
        {
            handleWinner(pMessage as PlayerWonGame);
        }
        else if(pMessage is RoomJoinedEvent)
        {
            handleRoomJoinedEvent(pMessage as RoomJoinedEvent);
        }
        else if (pMessage is PlayerNo)
        {
            handlePlayerNo(pMessage as PlayerNo);
        }
        else if (pMessage is PlaceAWallResult)
        {
            handlePlaceAWallResult(pMessage as PlaceAWallResult);
        }
    }

    private void handleMakeMoveResult(MakeMoveResult pMakeMoveResult)
    {
        view.gameBoard.SetBoardData(pMakeMoveResult.boardData);

        //some label display
        if (pMakeMoveResult.whoMadeTheMove == 1)
        {
            view.playerLabel1.text = string.Format("{0}", player1Name);

            ChangeTurnTo(2);
        }
        if (pMakeMoveResult.whoMadeTheMove == 2)
        {
            view.playerLabel2.text = string.Format("{0}", player2Name);

            ChangeTurnTo(1);
        }

    }

    private void handlePlaceAWallResult(PlaceAWallResult placeAWallResult)
    {
        view.gameBoard.SetBoardData(placeAWallResult.boardData);

        //some label display
        if (placeAWallResult.whoMadeTheMove == 1)
        {
            view.playerLabel1.text = string.Format("{0}", player1Name);

            ChangeTurnTo(2);
        }
        if (placeAWallResult.whoMadeTheMove == 2)
        {
            view.playerLabel2.text = string.Format("{0}", player2Name);

            ChangeTurnTo(1);
        }
    }

    private void handleNewPlayersInfo(NewPlayersInGameRequest newPlayersInGameRequest)
    {
        view.playerLabel1.text = "P1: " + newPlayersInGameRequest.namePlayer1;
        view.playerLabel2.text = "P2: " + newPlayersInGameRequest.namePlayer2;   
        player1Name = newPlayersInGameRequest.namePlayer1;
        player2Name = newPlayersInGameRequest.namePlayer2;
    }
    private void handleWinner(PlayerWonGame playerWonGame)
    {
        UnityEngine.Debug.Log(playerWonGame.winner);
        
    }
    private void handleRoomJoinedEvent(RoomJoinedEvent pMessage)
    {
        if (pMessage.room == RoomJoinedEvent.Room.LOBBY_ROOM)
        {
            fsm.ChangeState<LobbyState>();
        }
    }

    private void handlePlayerNo(PlayerNo pMessage)
    {
        player = pMessage.player;
        view.gameBoard.SetPlayer(player);

        SetUIPlayer();
    }

    private void SetUIPlayer()
    {
        if (player == 1)
        {
            view.playerColor.color = Color.red;
        }
        else
        {
            view.playerColor.color = Color.blue;
        }
    }

    void ChangeTurnTo (int pPlayer)
    {
        if (pPlayer == 1) 
        {
            view.turnColor.color = Color.red;
        }
        else
        {
            view.turnColor.color = Color.blue;
        }


        if(player == pPlayer)
        {
            view.gameBoard.wallManager.BlockWallMovement();
        }
        else
        {
            view.gameBoard.wallManager.UnblockWallMovement();
        }
    }
}
