using System;
using System.IO.Ports;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XModem.Core;
using XModem.Desktop.Services;


namespace XModem.Desktop.ViewModels;

public class PortSettingsViewModel : ViewModel
{
    private readonly SerialPortConfiguration _configuration;
    private string _portName;
    private int _portBaudRate;
    private int _portDataBits;
    private string _parityChoose;
    private string _stopBitsChoose;
    private Parity _portParity;
    private StopBits _portStopBits;

    public string[] PortNameOptions { get; set; } = SerialPort.GetPortNames();
    public int[] BaudRateOptions { get; set; } = { 300, 600, 1200, 2400, 9600, 14400, 19200, 38400, 57600, 115200 };
    public int[] DataBitsOptions { get; set; } = { 5, 6, 7, 8 };
    public string[] PortParityOptions { get; set; } = { "None", "Even", "Mark", "Odd", "Space" };
    public string[] StopBitsOptions { get; set; } = { "One", "Two", "OnePointFive" };

    public string PortName
    {
        get => _portName;
        set
        {
            if (value == _portName) return;
            _portName = value;
            OnPropertyChanged();
        }
    }

    private INavigationService _navigationService = null!;

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public int PortBaudRate
    {
        get => _portBaudRate;
        set
        {
            if (value == _portBaudRate) return;
            _portBaudRate = value;
            OnPropertyChanged();
        }
    }

    public int PortDataBits
    {
        get => _portDataBits;
        set
        {
            if (value == _portDataBits) return;
            _portDataBits = value;
            OnPropertyChanged();
        }
    }

    public string ParityChoose
    {
        get => _parityChoose;
        set
        {
            if (value == _parityChoose) return;
            _parityChoose = value;
            _portParity = value switch
            {
                "Even" => Parity.Even,
                "Mark" => Parity.Mark,
                "Odd" => Parity.Odd,
                "Space" => Parity.Space,
                _ => Parity.None
            };
            OnPropertyChanged();
        }
    }

    public string StopBitsChoose
    {
        get => _stopBitsChoose;
        set
        {
            if (value == _stopBitsChoose) return;
            _stopBitsChoose = value;
            _portStopBits = value switch
            {
                "Two" => StopBits.Two,
                "OnePointFive" => StopBits.OnePointFive,
                _ => StopBits.One
            };
            OnPropertyChanged();
        }
    }

    public IRelayCommand NextCommand { get; set; }

    public PortSettingsViewModel(INavigationService navigationService, SerialPortConfiguration configuration)
    {
        _configuration = configuration;
        NavigationService = navigationService;
        NextCommand = new RelayCommand(Next);
        PortBaudRate = 9600;
        PortDataBits = 8;
        ParityChoose = "None";
        StopBitsChoose = "One";
    }

    private void Next()
    {
        _configuration.PortName = PortName;
        _configuration.BaudRate = PortBaudRate;
        _configuration.Parity = _portParity;
        _configuration.StopBits = _portStopBits;
        _configuration.DataBits = PortDataBits;
        NavigationService.NavigateTo<ModeSelectionViewModel>();
    }
}