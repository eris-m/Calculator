using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public ObservableCollection<string> History { get; } = [];

    private string _inputText = "";

    [ObservableProperty]
    private double _output = 0.0d;

    private ExecutionContext _executionContext = new();

    public MainWindowViewModel()
    {
        History.CollectionChanged += (sender, args) => InputText = "";
    }
    
    [RelayCommand]
    public void OnEquals()
    {
        History.Add($"{InputText} = {Output}");
    }
    
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
        var parsed = Parser.ParseExpression(InputText);

        if (!parsed.WasSuccessful)
        {
            Output = double.NaN;
        }
        else
        {
            Output = parsed.Value.Evaluate(_executionContext);
        }
    }
}