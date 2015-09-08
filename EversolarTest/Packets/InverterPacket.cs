using System;
using System.Linq;

namespace EversolarTest.Packets
{
    public class InverterPacket
    {
        public byte[] Data { get; private set; }
        public byte[] Header { get; private set; }
        public byte SourceAddress { get; private set; }
        public byte DestinationAddress { get; private set; }
        public ControlCodes ControlCode { get; private set; }
        public byte FunctionCode { get; private set; }
        public byte[] Payload { get; private set; }
        public byte[] Checksum { get; private set; }

        public virtual void Fill(byte[] data)
        {
            Data = data;

            Header = data.Take(2).ToArray();

            SourceAddress = data[2];
            DestinationAddress = data[5];

            ControlCode = (ControlCodes)data[6];
            FunctionCode = data[7];

            if (data[8] > 0)
            {
                Payload = new byte[data[8]];

                Array.Copy(data, 9, Payload, 0, Payload.Length);
            }

            Checksum = new byte[2];
            Array.Copy(data, data.Length - 2, Checksum, 0, 2);
        }
    }
}
