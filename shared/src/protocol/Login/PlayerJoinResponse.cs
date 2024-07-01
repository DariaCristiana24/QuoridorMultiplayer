namespace shared
{
    /**
     * Send from SERVER to CLIENT to let the client know whether it was allowed to join or not.
     */
    public class PlayerJoinResponse : ASerializable
    {
        public enum RequestResult { ACCEPTED, DECLINED }; 
        public RequestResult result;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write((int)result);
        }

        public override void Deserialize(Packet pPacket)
        {
            result = (RequestResult)pPacket.ReadInt();
        }
    }
}
