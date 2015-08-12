using System;
using System.Linq;

namespace EversolarTest
{
    public class InverterPacket
    {
        public byte[] Header { get; private set; }
        public byte SourceAddress { get; private set; }
        public byte DestinationAddress { get; private set; }
        public ControlCodes ControlCode { get; private set; }
        public byte FunctionCode { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] Checksum { get; private set; }

        public InverterPacket(byte[] data)
        {
            Header = data.Take(2).ToArray();

            SourceAddress = data[2];
            DestinationAddress = data[5];

            ControlCode = (ControlCodes)data[6];
            FunctionCode = data[7];

            if (data[8] > 0)
            {
                Data = new byte[data[8]];

                Array.Copy(data, 9, Data, 0, Data.Length);
            }

            Checksum = new byte[2];
            Array.Copy(data, data.Length - 2, Checksum, 0, 2);
            Checksum = Checksum.Reverse().ToArray();
        }
    }
}
