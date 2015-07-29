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


        public void Register()
        {
            // Offline query
            WritePacket(0x00, ControlCode.Register, 0x00);

            var registerRequest = ReadPacket();


        }


        private Packet ReadPacket()
        {
            byte[] buffer = new byte[9];
            _serialPort.Read(buffer, 0, buffer.Length);

            byte[] remainder = new byte[buffer[8] + 2];
            _serialPort.Read(remainder, 0, remainder.Length);

            return new Packet(buffer.Concat(remainder).ToArray());
        }

        private void WritePacket(byte destAddress, ControlCode controlCode, byte funcCode, byte[] data = null)
        {
            var packet = new byte[11 + data.Length];

            // Header;
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
            data = data ?? new byte[0];

            packet[8] = (byte)data.Length;
            Array.Copy(data, 0, packet, 9, data.Length);

            // Checksum
            var checksum = (ushort)packet.Sum(b => b);

            packet[packet.Length - 2] = (byte)((checksum & 0xFF00) >> 8);
            packet[packet.Length - 1] = (byte)checksum;

            _serialPort.Write(data, 0, data.Length);
        }
    }
}
