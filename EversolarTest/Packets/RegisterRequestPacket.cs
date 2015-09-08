using System.Linq;
using System.Text;

namespace EversolarTest.Packets
{
    public class RegisterRequestPacket : InverterPacket
    {
        public string SerialNumber { get; set; }
        public byte[] SerialNumberBytes { get; set; }

        public byte[] RegistrationCode { get; set; }


        public override void Fill(byte[] data)
        {
            base.Fill(data);

            SerialNumberBytes = Payload.Take(16).ToArray();
            SerialNumber = Encoding.ASCII.GetString(SerialNumberBytes);

            RegistrationCode = Payload.Skip(16).Take(2).ToArray();
        }
    }
}
