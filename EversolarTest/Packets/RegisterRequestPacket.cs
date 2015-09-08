using System.Linq;
using System.Text;

namespace EversolarTest.Packets
{
    public class RegisterRequestPacket : InverterPacket
    {
        public string SerialNumber { get; private set; }
        public byte[] SerialNumberBytes { get; private set; }

        public byte[] RegistrationCode { get; private set; }


        public override void Parse(byte[] data)
        {
            base.Parse(data);

            SerialNumberBytes = Payload.Take(16).ToArray();
            SerialNumber = Encoding.ASCII.GetString(SerialNumberBytes);

            RegistrationCode = Payload.Skip(16).Take(2).ToArray();
        }
    }
}
