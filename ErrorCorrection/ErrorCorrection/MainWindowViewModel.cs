using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace ErrorCorrection;

public class MainWindowViewModel : NotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        InputFile = "Ścieżka do pliku";
        OutputFile = "Ścieżka do pliku";
        
        ChooseInputCommand = new RelayCommand(ChooseInput);
        ChooseOutputCommand = new RelayCommand(ChooseOutput);
        EncodeCommand = new RelayCommand(Encode);
        DecodeCommand = new RelayCommand(Decode);
    }

    #region Commands

    public ICommand ChooseInputCommand { get; }
    public ICommand ChooseOutputCommand { get; }

    public ICommand EncodeCommand { get; }
    public ICommand DecodeCommand { get; }

    #endregion

    #region Properties

    private string _inputFile = null!;

    public string InputFile
    {
        get => _inputFile;
        set
        {
            _inputFile = value;
            OnPropertyChanged();
        }
    }

    private string _outputFile = null!;

    public string OutputFile
    {
        get => _outputFile;
        set
        {
            _outputFile = value;
            OnPropertyChanged();
        }
    }

    #endregion


    private void ChooseInput(object _)
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        InputFile = openFileDialog.FileName;
    }

    private void ChooseOutput(object _)
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        OutputFile = openFileDialog.FileName;
    }

    private void Encode(object _)
    {
        var saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() != true)
        {
            return;
        }

        var resultFile = saveFileDialog.FileName;
        Lib.Correction.Encode(InputFile, resultFile);
        MessageBox.Show("Pomyślnie zakodowano");
    }
    private void Decode(object _)
    {
        var saveFileDialog = new SaveFileDialog();
        if (saveFileDialog.ShowDialog() != true)
        {
            return;
        }

        var resultFile = saveFileDialog.FileName;
        Lib.Correction.Decode(OutputFile, resultFile);
        MessageBox.Show("Pomyślnie odkodowano");
    }
}