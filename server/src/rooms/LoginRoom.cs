using shared;
using System.Collections.Generic;
using System.Diagnostics;

namespace server
{
	/**
	 * The LoginRoom is the first room clients 'enter' until the client identifies himself with a PlayerJoinRequest. 
	 * If the client sends the wrong type of request, it will be kicked.
	 *
	 * A connected client that never sends anything will be stuck in here for life,
	 * unless the client disconnects (that will be detected in due time).
	 */ 
	class LoginRoom : SimpleRoom
	{

		List<string> takenNames = new List<string>();

		public LoginRoom(TCPGameServer pOwner) : base(pOwner)
		{
		}

		protected override void addMember(TcpMessageChannel pMember)
		{
			base.addMember(pMember);

			//notify the client that (s)he is now in the login room, clients can wait for that before doing anything else
			RoomJoinedEvent roomJoinedEvent = new RoomJoinedEvent();
			roomJoinedEvent.room = RoomJoinedEvent.Room.LOGIN_ROOM;
			pMember.SendMessage(roomJoinedEvent);
		}

		protected override void handleNetworkMessage(ASerializable pMessage, TcpMessageChannel pSender)
		{
			if (pMessage is PlayerJoinRequest)
			{
				handlePlayerJoinRequest(pMessage as PlayerJoinRequest, pSender);

			}
			else //if member sends something else than a PlayerJoinRequest
			{
				Log.LogInfo("Declining client, auth request not understood", this);

				//don't provide info back to the member on what it is we expect, just close and remove
				removeAndCloseMember(pSender);
			}
		}

		/**
		 * Tell the client he is accepted and move the client to the lobby room.
		 */
		private void handlePlayerJoinRequest (PlayerJoinRequest pMessage, TcpMessageChannel pSender)
		{
            PlayerJoinResponse playerJoinResponse = new PlayerJoinResponse();
            switch (checkNameAvailability(pMessage.name))
			{
				case PlayerJoinResponse.RequestResult.ACCEPTED:

					takenNames.Add(pMessage.name);
                    Log.LogInfo("Moving new client to accepted...", this);

                    playerJoinResponse.result = PlayerJoinResponse.RequestResult.ACCEPTED;
					pSender.playerInfo.Name = pMessage.name;
                    
                    removeMember(pSender);
                    _server.GetLobbyRoom().AddMember(pSender);

                    break;
				case PlayerJoinResponse.RequestResult.DECLINED:
                    Log.LogInfo("Client was declined due to already in use name...", this);
                    playerJoinResponse.result = PlayerJoinResponse.RequestResult.DECLINED;
                    break;
			}
            pSender.SendMessage(playerJoinResponse);

        }

		/*
		 * Checks if the name is available
		 */

		private PlayerJoinResponse.RequestResult checkNameAvailability(string nameToCheck)
		{
			foreach(string name in takenNames)
			{
				if(name == nameToCheck)
				{
					return PlayerJoinResponse.RequestResult.DECLINED;
				}
			}
			return PlayerJoinResponse.RequestResult.ACCEPTED;
		}

	}
}
