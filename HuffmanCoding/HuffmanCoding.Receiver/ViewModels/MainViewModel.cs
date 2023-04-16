using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuffmanCoding.Core;
using Microsoft.Win32;

namespace HuffmanCoding.Receiver.ViewModels;

public class MainViewModel : ObservableObject
{
    private int _portNumber;
    private string _message;

    public MainViewModel()
    {
        ListenCommand = new RelayCommand(Listen);
        SaveFileCommand = new RelayCommand(SaveFile);
        Message = string.Empty;
    }

    public RelayCommand ListenCommand { get; }
    public RelayCommand SaveFileCommand { get; }

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

    public string Message
    {
        get => _message;
        set
        {
            if (value == _message) return;
            _message = value;
            OnPropertyChanged();
        }
    }

    private void Listen()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, PortNumber);
        try
        {
            var socket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endpoint);
            socket.Listen();
            var handler = socket.Accept();
            var data = string.Empty;
            while (true)
            {
                var bytes = new byte[1024];
                var bytesReceived = handler.Receive(bytes);
                data += System.Text.Encoding.UTF8.GetString(bytes, 0, bytesReceived);
                if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                {
                    break;
                }
            }
            
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            var splittedData = data.Split("<MSG>");
            var dict = splittedData[0];
            var msg = splittedData[1];
            msg = msg[..^5];
            var huffman = HuffmanEncoding.CreateFromEncoding(dict);
            
            Message = huffman.DecodeMessage(msg);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void SaveFile()
    {
        var saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() != true)
        {
            return;
        }

        File.WriteAllText(saveFileDialog.FileName, Message);
    }
}