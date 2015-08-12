using System;
using System.IO.Ports;
using System.Linq;

namespace EversolarTest
{
    public class EversolarPmu
    {
        private SerialPort _serialPort;
        private byte _address;

        public EversolarPmu(SerialPort serialPort, byte address)
        {
            _serialPort = serialPort;
            _address = address;

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public InverterPacket Register()
        {
            WritePacket(0x00, ControlCodes.Register, RegisterFunctions.OfflineQuery);

            return ReadPacket();
        }

        public InverterPacket SendRegisterAddress(InverterPacket packet)
        {
            WritePacket(packet.SourceAddress, ControlCodes.Register, RegisterFunctions.SendRegisterAddress);

            return ReadPacket();
        }


        private InverterPacket ReadPacket()
        {
            byte[] buffer = new byte[9];
            _serialPort.Read(buffer, 0, buffer.Length);

            byte[] remainder = new byte[buffer[8] + 2];
            _serialPort.Read(remainder, 0, remainder.Length);

            return new InverterPacket(buffer.Concat(remainder).ToArray());
        }

        private void WritePacket(byte destAddress, ControlCodes controlCode, byte funcCode, byte[] data = null)
        {
            data = data ?? new byte[0];

            var packet = new byte[11 + data.Length];

            // Header
            packet[0] = 0xAA;
            packet[1] = 0x55;

            // Source address
            packet[2] = _address;
            packet[3] = 0x00;

            // Destination address
            packet[4] = 0x00;
            packet[5] = destAddress;

            // Operation
            packet[6] = (byte)controlCode;
            packet[7] = funcCode;

            // Data
            packet[8] = (byte)data.Length;
            Array.Copy(data, 0, packet, 9, data.Length);

            // Checksum
            var checksum = (ushort)packet.Sum(b => b);

            packet[packet.Length - 2] = (byte)checksum;
            packet[packet.Length - 1] = (byte)((checksum & 0xFF00) >> 8);

            _serialPort.DiscardInBuffer();
            _serialPort.Write(packet, 0, packet.Length);
        }
    }
}
