// See https://aka.ms/new-console-template for more information

using XModem.Core;

Console.WriteLine("Starting Receiver Mode on COM3");

var modem = new XModem.Core.XModem("COM3");
modem.Start(XModemMode.Receiver, false);