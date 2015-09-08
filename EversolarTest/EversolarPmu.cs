using EversolarTest.Packets;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace EversolarTest
{
    public class EversolarPmu : IDisposable
    {
        private SerialPort _serialPort;
        private byte _address;
        private byte[] _lastPacketData = null;

        public EversolarPmu(string portName, byte address)
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
            var buffer = new byte[9];
            _serialPort.Read(buffer, 0, buffer.Length);

            var remainder = new byte[buffer[8] + 2];
            _serialPort.Read(remainder, 0, remainder.Length);

            _lastPacketData = buffer.Concat(remainder).ToArray();
        }


        public RegisterRequestPacket Register()
        {
            return WaitForPacket<RegisterRequestPacket>(0x00, ControlCodes.Register, RegisterFunctions.OfflineQuery);
        }

        //public InverterPacket ReRegister()
        //{
        //    return WaitForPacket(0x00, ControlCodes.Register, RegisterFunctions.ReRegister);
        //}

        //public InverterPacket SendRegisterAddress(InverterPacket packet)
        //{
        //    return WaitForPacket(packet.SourceAddress, ControlCodes.Register, RegisterFunctions.SendRegisterAddress);
        //}


        private TPacket WaitForPacket<TPacket>(byte destAddress, ControlCodes controlCode, byte funcCode, byte[] data = null)
            where TPacket : InverterPacket, new()
        {
            _lastPacketData = null;

            while (_lastPacketData == null)
            {
                WritePacket(destAddress, controlCode, funcCode, data);
                Thread.Sleep(1000);

                Console.WriteLine("Sending packet");
            }

            var packet = new TPacket();
            packet.Fill(_lastPacketData);

            return packet;
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

            packet[packet.Length - 2] = (byte)(checksum >> 8);
            packet[packet.Length - 1] = (byte)checksum;

            //_serialPort.DiscardInBuffer();
            _serialPort.Write(packet, 0, packet.Length);
        }
    }
}
