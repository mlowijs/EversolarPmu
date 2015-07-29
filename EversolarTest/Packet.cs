using System;
using System.Linq;

namespace EversolarTest
{
    public class Packet
    {
        public byte[] Header { get; private set; }
        public byte[] SourceAddress { get; private set; }
        public byte[] DestinationAddress { get; private set; }
        public ControlCode ControlCode { get; private set; }
        public byte FunctionCode { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] Checksum { get; private set; }

        public Packet(byte[] data)
        {
            Header = data.Take(2).ToArray();
            SourceAddress = data.Skip(2).Take(2).ToArray();
            DestinationAddress = data.Skip(4).Take(2).ToArray();
            ControlCode = (ControlCode)data[6];
            FunctionCode = data[7];

            if (data[8] > 0)
            {
                Data = new byte[data[8]];

                Array.Copy(data, 9, Data, 0, Data.Length);
            }

            Array.Copy(data, data.Length - 2, Checksum, 0, 2);
        }
    }
}
