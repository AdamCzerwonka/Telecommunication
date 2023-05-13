using System.Media;
using NAudio.Wave;

namespace ConventerAC.Core;

public class ConventerAC
{
    private WaveInEvent waveSource { get; set; }
    private WaveFileWriter waveFile { get; set; }

    public void StartRecording(int sampling, int quantization, string filePath)
    {
        waveSource = new WaveInEvent();
        waveSource.WaveFormat = new WaveFormat(sampling, quantization, 2);

        waveSource.DataAvailable += OnDataAvailable;
        waveSource.RecordingStopped += OnRecordingStopped;

        waveFile = new WaveFileWriter(filePath, waveSource.WaveFormat);
        waveSource.StartRecording();
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        waveFile.Write(e.Buffer, 0, e.BytesRecorded);
        waveFile.Flush();
    }

    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        waveSource.Dispose();
        waveFile.Dispose();
    }

    public void StopRecording()
    {
        waveSource.StopRecording();
    }

    public bool PlaySound(string path)
    {
        try
        {
            var player = new SoundPlayer(path);
            player.Play();
        } catch (Exception _)
        {
            return false;
        }
        return true;
    }
}