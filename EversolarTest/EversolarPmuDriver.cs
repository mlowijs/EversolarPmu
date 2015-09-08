using EversolarTest.Packets;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace EversolarTest
{
    public class EversolarPmuDriver : IDisposable
    {
        private SerialPort _serialPort;
        private byte _address;
        private byte[] _receivedPacketData = null;

        public EversolarPmuDriver(string portName, byte address)
        {
            _address = address;

            _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                ReadBufferSize = 256,
            };

            _serialPort.DataReceived += OnSerialDataReceived;

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Dispose()
        {
            _serialPort.Close();
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var leader = new byte[9];
            _serialPort.Read(leader, 0, leader.Length);

            var remainder = new byte[leader[8] + 2];
            _serialPort.Read(remainder, 0, remainder.Length);

            _receivedPacketData = leader.Concat(remainder).ToArray();

            if (!ValidatePacket(_receivedPacketData))
                _receivedPacketData = null;
        }


        public RegisterRequestPacket Register()
        {
            return WaitForPacket<RegisterRequestPacket>(0x00, ControlCodes.Register, RegisterFunctions.OfflineQuery);
        }

        public AddressConfirmPacket SendRegisterAddress(RegisterRequestPacket packet)
        {
            return WaitForPacket<AddressConfirmPacket>(packet.SourceAddress, ControlCodes.Register, RegisterFunctions.SendRegisterAddress, packet.Payload);
        }

        public QueryNormalInfoPacket QueryNormalInfo(byte inverterAddress)
        {
            return WaitForPacket<QueryNormalInfoPacket>(inverterAddress, ControlCodes.Read, ReadFunctions.QueryNormalInfo);
        }


        private bool ValidatePacket(byte[] data)
        {
            var highByte = data[data.Length - 2];
            var lowByte = data[data.Length - 1];

            var checksum = (ushort)(highByte << 8 | lowByte);

            return data.Take(data.Length - 2).Sum(b => b) == checksum;
        }

        private TPacket WaitForPacket<TPacket>(byte inverterAddress, ControlCodes controlCode, byte functionCode, byte[] payload = null)
            where TPacket : InverterPacket, new()
        {
            _receivedPacketData = null;

            while (_receivedPacketData == null)
            {
                WritePacket(inverterAddress, controlCode, functionCode, payload);
                Thread.Sleep(1000);

                Console.WriteLine("V - Sending packet");
            }

            var packet = new TPacket();
            packet.Parse(_receivedPacketData);

            return packet;
        }

        private void WritePacket(byte inverterAddress, ControlCodes controlCode, byte functionCode, byte[] payload = null)
        {
            payload = payload ?? new byte[0];

            var packet = new byte[11 + payload.Length];

            // Header
            packet[0] = 0xAA;
            packet[1] = 0x55;

            // Source address
            packet[2] = _address;
            packet[3] = 0x00;

            // Destination address
            packet[4] = 0x00;
            packet[5] = inverterAddress;

            // Operation
            packet[6] = (byte)controlCode;
            packet[7] = functionCode;

            // Data
            packet[8] = (byte)payload.Length;
            Array.Copy(payload, 0, packet, 9, payload.Length);

            // Checksum
            var checksum = (ushort)packet.Sum(b => b);

            packet[packet.Length - 2] = (byte)(checksum >> 8);
            packet[packet.Length - 1] = (byte)checksum;

            _serialPort.Write(packet, 0, packet.Length);
        }
    }
}
