// See https://aka.ms/new-console-template for more information

using System.Net;
using XModem.Core;

Console.WriteLine("Starting Sender Mode On COM2");

var modem = new XModem.Core.XModem("COM2");
var dataStream = File.OpenRead(@"C:\Users\macze\Downloads\kolos.pdf");
modem.Start(XModemMode.Sender, dataStream);