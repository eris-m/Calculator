using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Calculator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
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