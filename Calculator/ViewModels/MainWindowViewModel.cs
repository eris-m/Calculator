using CommunityToolkit.Mvvm.ComponentModel;

namespace Calculator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Text
    {
        get => _text;
        set
        {
            SetProperty(ref _text, value);
            IsValid = IsValidValue();
        }
    }
    
    [ObservableProperty]
    private bool _isValid = false;
    private string _text = "";

    private bool IsValidValue()
    {
        return float.TryParse(_text, out _);
    }
}