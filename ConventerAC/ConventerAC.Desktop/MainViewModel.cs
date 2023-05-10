using System;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace ConventerAC.Desktop;

public class MainViewModel : NotifyPropertyChanged
{
    private string _saveFilePath;
    private Core.ConventerAC _conventerAc;
    private string _recordButtonText = "Start recording";

    public MainViewModel()
    {
        _conventerAc = new();
        RecordCommand = new RelayCommand(Record);
        ChooseSaveFileCommand = new RelayCommand(ChooseSaveFile);
        PlaySoundCommand = new RelayCommand(PlaySound);
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

    public string Sampling { get; set; } = "48000";
    public string Quantization { get; set; } = "8";

    public string SaveFilePath
    {
        get => _saveFilePath;
        set
        {
            if (value == _saveFilePath) return;
            _saveFilePath = value;
            OnPropertyChanged();
        }
    }

    public ICommand RecordCommand { get; }
    public ICommand ChooseSaveFileCommand { get; }
    public ICommand PlaySoundCommand { get; }

    private bool _isRecording { get; set; } = false;

    private void ChooseSaveFile()
    {
        var fileDialog = new SaveFileDialog();
        if (fileDialog.ShowDialog() == false)
        {
            return;
        }

        SaveFilePath = fileDialog.FileName;
    }

    private void Record()
    {
        if (!_isRecording)
        {
            _conventerAc.StartRecording(int.Parse(Sampling), int.Parse(Quantization), SaveFilePath);
            _isRecording = true;
            RecordButtonText = "Stop recording";
        }
        else
        {
            _conventerAc.StopRecording();
            _isRecording = false;
            RecordButtonText = "Start recording";
        }
    }

    private void PlaySound()
    {
        _conventerAc.PlaySound(SaveFilePath);
    }
}