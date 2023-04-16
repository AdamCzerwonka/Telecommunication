using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

    private string _ipAddress;
    private int _portNumber;
    private string _fileName;


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
        var fileContent = File.ReadAllText(FileName);
        var encoding = new HuffmanEncoding(fileContent);
        var dict = encoding.GetEncoding();
        var msg = encoding.EncodeMessage(fileContent);
        var dec = encoding.DecodeMessage(msg);

        var ipAddr = IPAddress.Parse(IpAddress);
        var endpoint = new IPEndPoint(ipAddr, PortNumber);

        var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            sender.Connect(endpoint);
            var data = Encoding.UTF8.GetBytes(dict + "<MSG>" + msg + "<EOF>");
            sender.Send(data);

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