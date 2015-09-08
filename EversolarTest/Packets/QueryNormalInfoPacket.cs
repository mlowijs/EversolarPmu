namespace EversolarTest.Packets
{
    public class QueryNormalInfoPacket : InverterPacket
    {
        public double TodayKwh { get; private set; }
        public int CurrentPowerAc { get; private set; }
        public double TotalKwh { get; private set; }


        public override void Parse(byte[] data)
        {
            base.Parse(data);

            TodayKwh = (Payload[2] << 8 | Payload[3]) / 100d;
            CurrentPowerAc = Payload[18] << 8 | Payload[19];
            TotalKwh = (Payload[22] << 24 | Payload[23] << 16 | Payload[24] << 8 | Payload[25]) / 10d;
        }
    }
}
