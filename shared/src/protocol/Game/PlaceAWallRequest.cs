namespace shared
{
    /**
     * Send from CLIENT to SERVER to indicate the position the client would like to place a wall onto.
     */
    public class PlaceAWallRequest : ASerializable
    {
        public int wall;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(wall);
        }

        public override void Deserialize(Packet pPacket)
        {
            wall = pPacket.ReadInt();
        }
    }
}
