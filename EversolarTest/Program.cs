namespace EversolarTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pmu = new EversolarPmu("COM3", 0x01))
            {
                var registration = pmu.Register();

                System.Console.WriteLine(registration.Checksum);
            }
        }
    }
}
