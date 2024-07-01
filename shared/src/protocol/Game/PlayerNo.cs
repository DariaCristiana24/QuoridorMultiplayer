using System;

namespace shared
{
    /**
     * Tell the CLIENT what player they are.
     */
    public class PlayerNo : ASerializable
    {
        public int player;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(player);
        }

        public override void Deserialize(Packet pPacket)
        {
            player = pPacket.ReadInt();
        }
    }
}