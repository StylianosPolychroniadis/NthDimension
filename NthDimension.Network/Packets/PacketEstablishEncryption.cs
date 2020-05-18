namespace NthDimension.Network.Packets
{
    // Request Encryption
    public class PacketEstablishEncryption
    {
        public class Request
        {

        }

        public class Response
        {
            public readonly string KeyXml;

            public Response(string keyXml)
            {
                KeyXml = keyXml;
            }
        }
    }
}
