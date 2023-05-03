using System.IO.Ports;

namespace XModem.Core;

public class XModem : IDisposable
{
    private readonly SerialPort _port;

    public XModem(string portName)
    {
        _port = new SerialPort(portName);
        _port.ReadTimeout = 10000;
    }

    public XModem(SerialPortConfiguration configuration)
    {
        _port = new SerialPort(configuration.PortName, configuration.BaudRate, configuration.Parity,
            configuration.DataBits, configuration.StopBits);
        _port.ReadTimeout = 10000;
    }

    public void Start(XModemMode modemMode, Stream stream, bool useCrc = false)
    {
        _port.Open();
        if (modemMode == XModemMode.Receiver)
        {
            StartReceiver(stream, useCrc);
        }
        else
        {
            StartSender(stream, useCrc);
        }
    }

    private void StartSender(Stream stream, bool useCrc)
    {
        // Wait for synchorization with the receiver
        while (true)
        {
            var response = ReadWithoutTimeout();
            if (response == (int)XModemSymbol.NAK)
            {
                break;
            }

            if (response == (int)XModemSymbol.C && useCrc)
            {
                break;
            }
        }

        var sendBuffer = new byte[128];
        var packetNumber = 1;
        while (true)
        {
            var bytesRead = stream.Read(sendBuffer, 0, 128);
            if (bytesRead == 0)
            {
                break;
            }

            for (var i = bytesRead; i < 128; i++)
            {
                sendBuffer[i] = 26;
            }

            var packet = new XModemPacket(XModemSymbol.SOH, packetNumber, sendBuffer);
            _port.Write(packet.GetHeader(), 0, 3);
            _port.Write(packet.Data, 0, 128);
            var checksumArray = packet.Checksum(useCrc);
            _port.Write(checksumArray, 0, checksumArray.Length);

            var response = _port.ReadByte();
            if (response == (int)XModemSymbol.ACK)
            {
                packetNumber++;
            }

            if (response == (int)XModemSymbol.NAK)
            {
                stream.Position -= bytesRead;
            }
        }

        while (true)
        {
            _port.WriteByte((byte)XModemSymbol.EOT);
            var response = _port.ReadByte();
            if (response == (int)XModemSymbol.ACK)
            {
                return;
            }
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
            // ignored
        }

        return 0;
    }

    private void StartReceiver(Stream stream, bool useCrc = false)
    {
        var packetLenght = useCrc ? 133 : 132;
        var synSymbol = useCrc ? XModemSymbol.C : XModemSymbol.NAK;
        var buffer = new byte[packetLenght];
        for (var i = 0; i < 6; i++)
        {
            _port.WriteByte((byte)synSymbol);

            try
            {
                var read = 0;
                while (true)
                {
                    read += _port.Read(buffer, read, packetLenght - read);
                    if (read == packetLenght)
                    {
                        break;
                    }
                }

                if (buffer[0] == (int)XModemSymbol.SOH)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(i);
            }
        }

        var data = new List<byte>();

        while (true)
        {
            var packet = XModemPacket.FromBuffer(buffer);
            if (packet.Symbol == XModemSymbol.SOH)
            {
                if (packet.IsPacketValid(useCrc) == false)
                {
                    _port.WriteByte((byte)XModemSymbol.NAK);
                }
                else
                {
                    data.AddRange(packet.Data);
                    _port.WriteByte((byte)XModemSymbol.ACK);
                }
            }
            else if (packet.Symbol == XModemSymbol.EOT)
            {
                _port.WriteByte((byte)XModemSymbol.ACK);
                break;
            }

            var read = 0;
            while (true)
            {
                read += _port.Read(buffer, read, packetLenght - read);
                if (read == packetLenght)
                {
                    break;
                }

                if (buffer[0] == (byte)XModemSymbol.EOT)
                {
                    break;
                }
            }
        }
        
        var paddingLenght = 0;
        for (var i = data.Count - 1; i >= 0; i--)
        {
            if (data[i] == 26)
            {
                paddingLenght++;
            }
            else
            {
                break;
            }
        }
        stream.Write(data.Take(data.Count - paddingLenght).ToArray());
    }

    public void Dispose()
    {
        _port.Dispose();
    }
}