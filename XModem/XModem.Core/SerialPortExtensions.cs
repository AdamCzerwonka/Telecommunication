using System.IO.Ports;

namespace XModem.Core;

public static class SerialPortExtensions
{
    public static void WriteByte(this SerialPort port, byte data)
    {
        var buffer = new byte[1];
        buffer[0] = data;
        port.Write(buffer, 0, 1);
    }
}