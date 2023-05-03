// See https://aka.ms/new-console-template for more information

using XModem.Core;

Console.WriteLine("Starting Sender Mode On COM2");

var modem = new XModem.Core.XModem("COM2");
modem.Start(XModemMode.Sender, false);
