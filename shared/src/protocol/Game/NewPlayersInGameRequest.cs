using System;

namespace shared
{
    /**
     * Send from CLIENT to SERVER to request joining the server.
     */
    public class NewPlayersInGameRequest : ASerializable
    {
        public string namePlayer1; 
        public string namePlayer2;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(namePlayer1);
            pPacket.Write(namePlayer2);
        }

        public override void Deserialize(Packet pPacket)
        {
            namePlayer1 = pPacket.ReadString();
            namePlayer2 = pPacket.ReadString();
        }
    }
}