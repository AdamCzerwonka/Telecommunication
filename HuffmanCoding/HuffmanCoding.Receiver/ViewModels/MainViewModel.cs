using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuffmanCoding.Core;
using Microsoft.Win32;

namespace HuffmanCoding.Receiver.ViewModels;

public class MainViewModel : ObservableObject
{
    private int _portNumber = 11111;
    private string _message = string.Empty;
    private bool _isRunning;
    private string _buttonText = "Listen";

    public MainViewModel()
    {
        ListenCommand = new RelayCommand(Listen, () => !IsRunning);
        SaveFileCommand = new RelayCommand(SaveFile);
        Message = string.Empty;
    }

    public IRelayCommand ListenCommand { get; }
    public IRelayCommand SaveFileCommand { get; }

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

    public string ButtonText
    {
        get => _buttonText;
        set
        {
            if (value == _buttonText) return;
            _buttonText = value;
            OnPropertyChanged();
        }
    }

    private bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (value == _isRunning) return;
            _isRunning = value;
            ListenCommand.NotifyCanExecuteChanged();
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
        IsRunning = true;
        ButtonText = "Listening...";
        var encodedStream = new MemoryStream();
        var endpoint = new IPEndPoint(IPAddress.Any, PortNumber);
        Task.Run(() =>
        {
            try
            {
                // utworz socket
                var socket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // nawiaz polaczenie
                socket.Bind(endpoint);
                socket.Listen();
                var handler = socket.Accept();
                // odbierz dane
                var bytes = new byte[1024];
                var bytesReceived = handler.Receive(bytes);
                var huffman = HuffmanEncoding.CreateFromEncoding(bytes[..bytesReceived]);
                encodedStream.Write(bytes.AsSpan()[..bytesReceived]);
                handler.Send(new byte[] { 0 });

                handler.Receive(bytes);
                handler.Send(new byte[] { 0 });
                var msgLenght = BitConverter.ToInt32(bytes.AsSpan()[..4]);
                encodedStream.Write(bytes.AsSpan()[..4]);
                var msgByteLenght = (int)Math.Ceiling(msgLenght / 8.0);
                var stream = new MemoryStream();
                var receivedMsgBytes = 0;
                while (true)
                {
                    var count = handler.Receive(bytes);
                    receivedMsgBytes += count;
                    stream.Write(bytes.AsSpan()[..count]);
                    if (receivedMsgBytes == msgByteLenght)
                    {
                        break;
                    }
                }

                // zakoncz polaczenie
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                stream.Position = 0;
                stream.CopyTo(encodedStream);

                var encodedMessage = stream
                    .ToArray()
                    .Select(b => Convert.ToString(b, 2).PadLeft(8, '0'))
                    .JoinString()[(msgByteLenght * 8 - msgLenght)..];

                var msg = huffman.DecodeMessage(encodedMessage);
                Message = msg;


                using var encFile = File.OpenWrite("test.bin");
                encodedStream.Position = 0;
                encodedStream.CopyTo(encFile);
                
                IsRunning = false;
                ButtonText = "Listen";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
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