using System.IO.Ports;

namespace XModem.Core;

public class XModem
{
    private readonly SerialPort _port;

    public XModem(string portName)
    {
        _port = new SerialPort(portName);
        _port.ReadTimeout = 10000;
    }

    public void Start(XModemMode modemMode, bool useCrc)
    {
        _port.Open();
        if (modemMode == XModemMode.Receiver)
        {
            StartReceiver(useCrc);
        }
        else
        {
            StartSender(useCrc);
        }
    }

    private void StartSender(bool useCrc)
    {
        var buffer = new byte[132];
        while (true)
        {
            var test = ReadWithoutTimeout();
            Console.WriteLine(test.ToString("x2"));
            if (test == (int)XModemSymbol.NAK)
            {
                Console.WriteLine("GOT NAK");
                break;
            }
        }

        var text = "Ala ma kota"u8.ToArray();
        var data = new byte[128];
        text.CopyTo(data, 0);

        var packet = new XModemPacket(XModemSymbol.SOH, 1, data);
        _port.Write(packet.GetHeader(), 0, 3);
        _port.Write(packet.Data, 0, 128);
        _port.Write(packet.Checksum(), 0, 1);

        var res = _port.ReadByte();
        if (res == (int)XModemSymbol.ACK)
        {
            Console.WriteLine("TESTESTEST");
            _port.Write(new byte[] { (byte)XModemSymbol.EOT }, 0, 1);
        }

        res = _port.ReadByte();
        if (res == (int)XModemSymbol.ACK)
        {
            Console.WriteLine("HAHAH");
        }
    }

    private byte ReadWithoutTimeout()
    {
        try
        {
            var value = _port.ReadByte();
            return (byte)value;
        }
        catch (Exception e)
        {
        }

        return 0;
    }

    private void StartReceiver(bool useCrc)
    {
        var shouldContinue = false;
        var buffer = new byte[132];
        for (var i = 0; i < 6; i++)
        {
            _port.WriteByte((byte)XModemSymbol.NAK);

            try
            {
                _port.Read(buffer, 0, 132);
                if (buffer[0] == (int)XModemSymbol.SOH)
                {
                    shouldContinue = true;
                    Console.WriteLine("GOT SOH");
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(i);
            }
        }

        while (shouldContinue)
        {
            var packet = XModemPacket.FromBuffer(buffer);
            if (packet.Symbol == XModemSymbol.SOH)
            {
                _port.WriteByte((byte)XModemSymbol.ACK);
            }
            else if (packet.Symbol == XModemSymbol.EOT)
            {
                _port.WriteByte((byte)XModemSymbol.ACK);
                break;
            }

            _port.Read(buffer, 0, 132);
        }
    }
}