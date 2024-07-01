using System;

namespace shared
{
    /**
     * Send the CLIENTS who won
     */
    public class PlayerWonGame : ASerializable
    {
        public int winner;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(winner);
        }

        public override void Deserialize(Packet pPacket)
        {
            winner = pPacket.ReadInt();
        }
    }
}