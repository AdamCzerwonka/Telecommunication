using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuffmanCoding.Core;
using Microsoft.Win32;

namespace HuffmanCoding.Sender.ViewModels;

public class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        ChooseFileCommand = new RelayCommand(ChooseFile);
        SendCommand = new RelayCommand(Send);
    }

    private string _ipAddress = "127.0.0.1";
    private int _portNumber = 11111;
    private string _fileName = string.Empty;


    public IRelayCommand ChooseFileCommand { get; }
    public IRelayCommand SendCommand { get; }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (value == _ipAddress) return;
            _ipAddress = value;
            OnPropertyChanged();
        }
    }

    public int PortNumber
    {
        get => _portNumber;
        set
        {
            if (value == _portNumber) return;
            _portNumber = value;
            OnPropertyChanged();
        }
    }

    public string FileName
    {
        get => _fileName;
        set
        {
            if (value == _fileName) return;
            _fileName = value;
            OnPropertyChanged();
        }
    }

    private void Send()
    {
        // wczytaj plik
        var fileContent = File.ReadAllText(FileName);
        // utworz instancje klasy z tekstu
        var encoding = new HuffmanEncoding(fileContent);
        // utworz slownik oraz zakodowana wiadomosc
        var msg = encoding.EncodeMessage(fileContent);

        var ipAddr = IPAddress.Parse(IpAddress);
        var endpoint = new IPEndPoint(ipAddr, PortNumber);
        try
        {
            // utworz socket
            var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // nawiaz polaczenie
            sender.Connect(endpoint);
            // przeslij dane slownikowe z zakodowana wiadomoscia
            // var data = Encoding.UTF8.GetBytes(dict + "<MSG>" + msg + "<EOF>");
            var data = encoding.GetBinaryEncoding();
            sender.Send(data);
            var buffer = new byte[20];
            sender.Receive(buffer);
            if (buffer[0] != 0)
            {
                MessageBox.Show("ERROR");
            }

            var msgLenght = BitConverter.GetBytes(msg.Length);
            sender.Send(msgLenght);

            sender.Receive(buffer);
            if (buffer[0] != 0)
            {
                MessageBox.Show("ERROR");
            }

            var msgBytes = msg.ConvertBinaryString();
            sender.Send(msgBytes);

            // zakoncz polaczenie
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ChooseFile()
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        FileName = openFileDialog.FileName;
    }
}