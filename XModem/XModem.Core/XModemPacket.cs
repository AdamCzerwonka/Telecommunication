namespace XModem.Core;

public class XModemPacket
{
    public XModemPacket(XModemSymbol symbol, int packetNumber, byte[] data) : this(symbol, data)
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

    public bool IsPacketValid(bool useCrc = false)
    {
        if (PacketNumber + PacketNumberChecksum != 0xff)
        {
            return false;
        }

        var calucaltedChecksum = useCrc ? CrcChecksum() : AlgebraicChecksum();
        return calucaltedChecksum == DataChecksum;
    }

    public byte[] Checksum(bool useCrc = false)
    {
        var value = useCrc ? CrcChecksum() : AlgebraicChecksum();
        DataChecksum = value;

        var checksum = BitConverter.GetBytes(value);

        return useCrc ? checksum : checksum[..1];
    }

    private short CrcChecksum()
    {
        var crc = 0;
        byte b;

        for (var i = 0; i < 128; i++)
        {
            crc ^= Data[i] << 8;
            b = 8;
            do
            {
                if ((crc & 0x8000) != 0)
                {
                    crc = crc << 1 ^ 0x1021;
                }
                else
                {
                    crc = crc << 1;
                }
            } while (--b != 0);
        }

        return (short)crc;
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