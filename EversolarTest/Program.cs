using EversolarTest.Packets;
using System;

namespace EversolarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pmu = new EversolarPmuDriver("COM3", 0x01))
            {
                var registration = pmu.Register();

                Console.WriteLine(registration);
            }
        }
    }
}
