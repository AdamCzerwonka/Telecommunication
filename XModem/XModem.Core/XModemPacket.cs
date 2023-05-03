namespace XModem.Core;

public class XModemPacket
{
    public XModemPacket(XModemSymbol symbol, byte packetNumber, byte[] data) : this(symbol, data)
    {
        PacketNumber = (byte)(packetNumber % 256);
        PacketNumberChecksum = (byte)(0xff - PacketNumber);
    }

    private XModemPacket(XModemSymbol symbol, byte[] data)
    {
        Symbol = symbol;
        Data = data;
    }

    public XModemSymbol Symbol { get; }
    public byte PacketNumber { get; private init; }
    public byte PacketNumberChecksum { get; private init; }
    public byte[] Data { get; }
    public short DataChecksum { get; private set; }


    public byte[] GetHeader()
    {
        var header = new byte[3];
        header[0] = (byte)Symbol;
        header[1] = PacketNumber;
        header[2] = PacketNumberChecksum;

        return header;
    }

    public byte[] Checksum(bool useCrc = false)
    {
        var checksumValue = AlgebraicChecksum();
        DataChecksum = checksumValue;

        var checksum = new byte[1];
        checksum[0] = checksumValue;

        return checksum;
    }

    private byte AlgebraicChecksum()
    {
        var sum = 0;
        foreach (var b in Data)
        {
            sum += b;
            sum %= 256;
        }

        return (byte)sum;
    }

    public static XModemPacket FromBuffer(byte[] packetBuffer)
    {
        var isCrcUsed = packetBuffer.Length == 133;
        var symbol = (XModemSymbol)packetBuffer[0];
        var data = packetBuffer[3..131];
        var packet = new XModemPacket(symbol, data)
        {
            PacketNumber = packetBuffer[1],
            PacketNumberChecksum = packetBuffer[2],
            DataChecksum = isCrcUsed ? BitConverter.ToInt16(packetBuffer.AsSpan()[131..133]) : packetBuffer[131]
        };

        return packet;
    }
}