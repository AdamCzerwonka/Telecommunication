using System;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ConventerAC.Desktop;

public class MainViewModel : ObservableObject
{
    private string _saveFilePath = string.Empty;
    private Core.ConventerAC _conventerAc;
    private string _recordButtonText = "Start recording";
    private string _loadFilePath;
    private string _snr = "SNR: -";
    private string _quantization;

    public MainViewModel()
    {
        _conventerAc = new Core.ConventerAC();
        RecordCommand = new RelayCommand(Record, () => SaveFilePath != string.Empty);
        ChooseSaveFileCommand = new RelayCommand(ChooseSaveFile);
        ChooseLoadFileCommand = new RelayCommand(ChooseLoadFile);
        PlayRecordedSoundCommand = new RelayCommand(PlayRecordedSound);
        PlayLoadedSoundCommand = new RelayCommand(PlayLoadedSound);
        Quantization = "8";
        Sampling = "48000";
    }

    public string RecordButtonText
    {
        get => _recordButtonText;
        set
        {
            if (value == _recordButtonText) return;
            _recordButtonText = value;
            OnPropertyChanged();
        }
    }

    public string Sampling { get; set; }

    public string Quantization
    {
        get => _quantization;
        set
        {
            if (value == _quantization) return;
            _quantization = value;
            OnPropertyChanged();
            CalculateSnr();
        }
    }

    public string SaveFilePath
    {
        get => _saveFilePath;
        set
        {
            if (value == _saveFilePath) return;
            _saveFilePath = value;
            OnPropertyChanged();
            RecordCommand.NotifyCanExecuteChanged();
        }
    }

    public string LoadFilePath
    {
        get => _loadFilePath;
        set
        {
            if (value == _loadFilePath) return;
            _loadFilePath = value;
            OnPropertyChanged();
        }
    }

    public string Snr
    {
        get => _snr;
        set
        {
            if (value == _snr) return;
            _snr = value;
            OnPropertyChanged();
        }
    }

    public IRelayCommand RecordCommand { get; }
    public IRelayCommand ChooseSaveFileCommand { get; }
    public IRelayCommand ChooseLoadFileCommand { get; }
    public IRelayCommand PlayRecordedSoundCommand { get; }
    public IRelayCommand PlayLoadedSoundCommand { get; }
    private bool IsRecording { get; set; }

    private void ChooseSaveFile()
    {
        var fileDialog = new SaveFileDialog
        {
            Filter = "Wav files (*.wav)|*.wav|All files (*.*)|*.*"
        };
        if (fileDialog.ShowDialog() == false)
        {
            return;
        }

        SaveFilePath = fileDialog.FileName;
    }

    private void ChooseLoadFile()
    {
        var fileDialog = new OpenFileDialog()
        {
            Filter = "Wav files (*.wav)|*.wav|All files (*.*)|*.*"
        };
        fileDialog.ShowDialog();

        LoadFilePath = fileDialog.FileName;
    }

    private void Record()
    {
        if (!IsRecording)
        {
            _conventerAc.StartRecording(int.Parse(Sampling), int.Parse(Quantization), SaveFilePath);
            IsRecording = true;
            RecordButtonText = "Stop recording";
        }
        else
        {
            _conventerAc.StopRecording();
            IsRecording = false;
            RecordButtonText = "Start recording";
        }
    }

    private void PlayRecordedSound()
    {
        if (!_conventerAc.PlaySound(SaveFilePath))
        {
            MessageBox.Show("Invalid file path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PlayLoadedSound()
    {
        if (!_conventerAc.PlaySound(LoadFilePath))
        {
            MessageBox.Show("Invalid file path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CalculateSnr()
    {
        var value = 20 * Math.Log10(Math.Pow(2, int.Parse(Quantization)));
        Snr = $"SNR: {value:F4}";
    }

}