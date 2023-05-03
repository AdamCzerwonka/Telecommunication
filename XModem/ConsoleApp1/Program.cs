// See https://aka.ms/new-console-template for more information

using XModem.Core;

Console.WriteLine("Starting Receiver Mode on COM3");

var modem = new XModem.Core.XModem("COM3");
var stream = File.OpenWrite(@"C:\studia\tele\test");
modem.Start(XModemMode.Receiver, stream, true);
stream.Flush();
stream.Close();