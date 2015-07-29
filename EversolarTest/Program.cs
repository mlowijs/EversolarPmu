using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EversolarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);

            var pmu = new EversolarPmu(serialPort, 0x10);
            
        }
    }
}
