using shared;
using System.Collections.Generic;
using UnityEngine;

/**
 * Starting state where you can connect to the server.
 */
public class LoginState : ApplicationStateWithView<LoginView>
{
    [SerializeField]    private string _serverIP = null;
    [SerializeField]    private int _serverPort = 0;
    [Tooltip("To avoid long iteration times, set this to true while testing.")]


    public override void EnterState()
    {
        base.EnterState();

        //listen to a connect click from our view
        view.ButtonConnect.onClick.AddListener(Connect);

    }

    public override void ExitState ()
    {
        base.ExitState();

        //stop listening to button clicks
        view.ButtonConnect.onClick.RemoveAllListeners();
    }

    /**
     * Connect to the server (with some client side validation)
     */
    private void Connect()
    {
        if (view.userName == "")
        {
            view.TextConnectResults = "Please enter a name first";
            return;
        }

        //connect to the server and on success try to join the lobby
        if (fsm.channel.Connect(_serverIP, _serverPort))
        {
            tryToJoinLobby();
        } else
        {
            view.TextConnectResults = "Oops, couldn't connect:"+string.Join("\n", fsm.channel.GetErrors());
        }
    }

    private void tryToJoinLobby()
    {
        PlayerJoinRequest playerJoinRequest = new PlayerJoinRequest();
        playerJoinRequest.name = view.userName;
        fsm.channel.SendMessage(playerJoinRequest);
    }

    /// //////////////////////////////////////////////////////////////////
    ///                     NETWORK MESSAGE PROCESSING
    /// //////////////////////////////////////////////////////////////////

    private void Update()
    {
        if (fsm.channel.Connected) receiveAndProcessNetworkMessages();
    }

    
    protected override void handleNetworkMessage(ASerializable pMessage)
    {
        if (pMessage is PlayerJoinResponse) handlePlayerJoinResponse (pMessage as PlayerJoinResponse);
        else if (pMessage is RoomJoinedEvent) handleRoomJoinedEvent (pMessage as RoomJoinedEvent);
    }
    

    private void handlePlayerJoinResponse(PlayerJoinResponse pMessage)
    {       
        if (pMessage.result == PlayerJoinResponse.RequestResult.ACCEPTED)
        {
            view.TextConnectResults = "Success!";
        }
        else
        {
            view.TextConnectResults = "Name already in use";
        }
    }

    private void handleRoomJoinedEvent (RoomJoinedEvent pMessage)
    {
        if (pMessage.room == RoomJoinedEvent.Room.LOBBY_ROOM)
        {
            fsm.ChangeState<LobbyState>();
        } 
    }

}

