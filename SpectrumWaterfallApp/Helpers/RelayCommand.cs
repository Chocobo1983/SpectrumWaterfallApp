using System.Windows.Input;

namespace SpectrumWaterfallApp.Helpers;

public class RelayCommand(Action<object?> execute) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => execute(parameter);
}