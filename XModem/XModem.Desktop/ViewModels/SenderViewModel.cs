using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace XModem.Desktop.ViewModels;

public class SenderViewModel : ViewModel
{
    public SenderViewModel()
    {
        ChooseFileCommand = new RelayCommand(ChooseFile);
        SendCommand = new RelayCommand(Send);
    }

    public IRelayCommand ChooseFileCommand { get; }
    public IRelayCommand SendCommand { get; }

    private string _filePath = null!;
    private bool _useCrc;

    public string FilePath
    {
        get => _filePath;
        set
        {
            if (value == _filePath) return;
            _filePath = value;
            OnPropertyChanged();
        }
    }

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

    private void ChooseFile()
    {
        var dialog = new OpenFileDialog();
        var result = dialog.ShowDialog();
        if (result == false)
        {
            return;
        }

        FilePath = dialog.FileName;
    }

    private void Send()
    {
        MessageBox.Show("SENDING " + FilePath);
    }
}