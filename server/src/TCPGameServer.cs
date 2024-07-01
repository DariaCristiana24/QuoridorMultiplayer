using System;
using System.Net.Sockets;
using System.Net;
using shared;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace server {

	/**
	 * Basic TCPGameServer that runs our game.
	 * 
	 * Server is made up out of different rooms that can hold different members.
	 */
	class TCPGameServer
	{

		public static void Main(string[] args)
		{
			TCPGameServer tcpGameServer = new TCPGameServer();
			tcpGameServer.run();
		}

		private LoginRoom _loginRoom;	//this is the room every new user joins
		private LobbyRoom _lobbyRoom;   //this is the room a user moves to after a successful 'login'
        private List<GameRoom> _gameRooms = new List<GameRoom>(); //this are the rooms a user moves to when a game is succesfully started

        //stores additional info for a player
        private Dictionary<TcpMessageChannel, PlayerInfo> _playerInfo = new Dictionary<TcpMessageChannel, PlayerInfo>();

		private TCPGameServer()
		{
			//we have only one instance of each room, this is especially limiting for the game room (since this means you can only have one game at a time).
			_loginRoom = new LoginRoom(this);
			_lobbyRoom = new LobbyRoom(this);
		}

		private void run()
		{
			Log.LogInfo("Starting server on port 55555", this, ConsoleColor.Gray);

			//start listening for incoming connections (with max 50 in the queue)
			//we allow for a lot of incoming connections, so we can handle them
			//and tell them whether we will accept them or not instead of bluntly declining them
			TcpListener listener = new TcpListener(IPAddress.Any, 55555);
			listener.Start(50);

			while (true)
			{
				//check for new members	
				if (listener.Pending())
				{
					//get the waiting client
					Log.LogInfo("Accepting new client...", this, ConsoleColor.White);
					TcpClient client = listener.AcceptTcpClient();
					//and wrap the client in an easier to use communication channel
					TcpMessageChannel channel = new TcpMessageChannel(client);
					//and add it to the login room for further 'processing'
					_loginRoom.AddMember(channel);
				}

				//now update every single room
				_loginRoom.Update();
				_lobbyRoom.Update();
				for (int i = 0; i < _gameRooms.Count; i++ ) 
				{
					_gameRooms[i].Update();
                }
				

				Thread.Sleep(100);
			}

		}
		
		//provide access to the different rooms on the server 
		public LoginRoom GetLoginRoom() { return _loginRoom; }
		public LobbyRoom GetLobbyRoom() { return _lobbyRoom; }

		public GameRoom GetLastGameRoom()
		{
			if (_gameRooms.Count > 0)
			{

				return _gameRooms[_gameRooms.Count - 1];
			}
			else
			{
				return null;
			}
		}

		public void CreateNewGameRoom() 
		{ 
			_gameRooms.Add(new GameRoom(this));
            Log.LogInfo("New Game Room created ID: " + _gameRooms.Count, this, ConsoleColor.Green);
        }

        public void RemoveGameRoom(GameRoom gameRoom) 
		{
			_gameRooms.Remove(gameRoom);
            Log.LogInfo("Game Room deleted " , this, ConsoleColor.Green);
        }

		public void RemovePlayerInfo (TcpMessageChannel pClient)
		{
			_playerInfo.Remove(pClient);
		}

	}

}


