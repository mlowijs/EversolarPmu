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


        public virtual void Parse(byte[] data)
        {
            Data = data;

            Header = data.Take(2).ToArray();

            SourceAddress = data[2];
            DestinationAddress = data[5];

            ControlCode = (ControlCodes)data[6];
            FunctionCode = data[7];

            Payload = new byte[data[8]];

            if (Payload.Length > 0)
                Array.Copy(data, 9, Payload, 0, Payload.Length);
        }
    }
}
