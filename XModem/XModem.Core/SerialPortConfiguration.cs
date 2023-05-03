using System.IO.Ports;

namespace XModem.Core;

public class SerialPortConfiguration
{
    public int BaudRate { get; set; }
    public int DataBits { get; set; }
    public Parity Parity { get; set; }
    public StopBits StopBits { get; set; }
}