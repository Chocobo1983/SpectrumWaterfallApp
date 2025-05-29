using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpectrumWaterfallApp.ViewModels;

public abstract class VmBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? prop = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}