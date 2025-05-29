using SpectrumWaterfallApp.Models;
using SpectrumWaterfallApp.Services;
using SpectrumWaterfallApp.Helpers;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using Timer = System.Timers.Timer;

namespace SpectrumWaterfallApp.ViewModels;

public class VmMain : VmBase
{
    public WriteableBitmap SpectrumBitmap { get; }
    public WriteableBitmap WaterfallBitmap { get; }

    private readonly SpectrumDataModel _model;
    private readonly SpectrumGenerator _generator;
    private readonly Timer _timer;

    private double _timeOffset = 0;
    private DateTime? _pauseTime = null;

    private double _zoom = 1.0;
    public double Zoom
    {
        get => _zoom;
        set { _zoom = Math.Clamp(value, 0.1, 10.0); OnPropertyChanged(); }
    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }

    public VmMain()
    {
        _model = new SpectrumDataModel(1024, 100, 200);
        _generator = new SpectrumGenerator(_model.Width);

        SpectrumBitmap = new WriteableBitmap(_model.Width, _model.SpectrumHeight, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
        WaterfallBitmap = new WriteableBitmap(_model.Width, _model.WaterfallHeight, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);

        StartCommand = new RelayCommand(_ => Start());
        StopCommand = new RelayCommand(_ => Stop());
        ZoomInCommand = new RelayCommand(_ => Zoom += 0.1);
        ZoomOutCommand = new RelayCommand(_ => Zoom -= 0.1);

        _timer = new Timer(50);
        _timer.Elapsed += Timer_Elapsed;
    }

    private void Start()
    {
        if (_pauseTime.HasValue)
        {
            _timeOffset += (DateTime.Now - _pauseTime.Value).TotalSeconds;
            _pauseTime = null;
        }
        _timer.Start();
    }

    private void Stop()
    {
        _pauseTime = DateTime.Now;
        _timer.Stop();
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _generator.Generate(_model.CurrentSpectrum, DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds + _timeOffset);

        Application.Current.Dispatcher.Invoke(() =>
        {
            SpectrumRenderer.RenderSpectrumLine(SpectrumBitmap, _model.CurrentSpectrum, _zoom);
            WaterfallRenderer.RenderWaterfall(WaterfallBitmap, _model.CurrentSpectrum, _model.WaterfallPowerMap, _model.WaterfallColorMap, _zoom);
        });
    }
}