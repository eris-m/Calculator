using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public enum ButtonId
    {
        One,
    }
    
    public string InputText
    {
        get => _inputText;
        set
        {
            SetProperty(ref _inputText, value);
            RunCalculation();
        }
    }
    
    private string _inputText = "";

    [ObservableProperty]
    private float _output = 0.0f;

    [RelayCommand]
    public void OnButtonClick(char button)
    {
        InputText += button;
    }

    [RelayCommand]
    public void Backspace()
    {
        if (InputText.Length == 0)
            return;
        
        InputText = InputText[..^1];
        //TODO
    }
    
    private void RunCalculation()
    {
        var tokens = Tokeniser.TokeniseString(InputText);
        var parsed = Parser.ParseNext(out var output, tokens.ToArray().AsSpan());

        if (!parsed)
        {
            Output = float.NaN;
        }
        else
        {
            Output = output.Expression.Evaluate();
        }
    }
}