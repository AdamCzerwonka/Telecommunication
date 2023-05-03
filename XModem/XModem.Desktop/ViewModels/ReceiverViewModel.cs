using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using XModem.Core;

namespace XModem.Desktop.ViewModels;

public class ReceiverViewModel : ViewModel
{
    private readonly Core.XModem _xModem;
    private bool _useCrc;

    public ReceiverViewModel(Core.XModem xModem)
    {
        _xModem = xModem;
        ReceiveCommand = new RelayCommand(Receive);
    }

    public ICommand ReceiveCommand { get; set; }

    public bool UseCrc
    {
        get => _useCrc;
        set
        {
            if (value == _useCrc) return;
            _useCrc = value;
            OnPropertyChanged();
        }
    }

    private void Receive()
    {
        var dialog = new SaveFileDialog();
        var result = dialog.ShowDialog();
        if (result == false)
        {
            return;
        }

        var stream = File.OpenWrite(dialog.FileName);
        _xModem.Start(XModemMode.Receiver, stream, UseCrc);
        stream.Flush();
        stream.Close();
        MessageBox.Show("Finished receiving");
    }
}