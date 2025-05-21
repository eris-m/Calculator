using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculator.ViewModels;

public partial class CalculationViewModel: ViewModelBase
{
    private EvaluationContext _context;

    //[ObservableProperty]
    private string _expression;
    [ObservableProperty]
    private double _value;
    
    public CalculationViewModel(EvaluationContext context, string expression)
    {
        _context = context;
        Expression = expression;

        UpdateValue();
    }

    public CalculationViewModel(EvaluationContext context) : this(context, "")
    {
    }

    public Action<CalculationViewModel> RemoveAction;
    
    public string Expression
    {
        get => _expression;
        set {
            SetProperty(ref _expression, value);
            UpdateValue();
        }
    }

    [RelayCommand]
    private void Remove()
    {
        RemoveAction(this);
    }
    
    private void UpdateValue()
    {
        var parseResult = Parser.ParseExpression(Expression);
        if (!parseResult.WasSuccessful)
            Value = double.NaN;
        else
            Value = parseResult.Value.Evaluate(_context);
    }
}

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<CalculationViewModel> History { get; }

    private CalculationViewModel CurrentInput 
    { 
        get => History[History.Count - 1]; 
        set => History[History.Count - 1] = value; 
    }

    [ObservableProperty]
    private double _output = 0.0d;

    private EvaluationContext _evaluationContext = new();

    public MainWindowViewModel()
    {
        History = [CalculationFactory()];
        //History.CollectionChanged += (sender, args) => InputText = "";
    }
    
    [RelayCommand]
    public void OnEquals()
    {
        History.Add(CalculationFactory());
        //History.Add($"{InputText} = {Output}");
    }
    
    [RelayCommand]
    public void OnButtonClick(char button)
    {
        CurrentInput.Expression += button;
    }

    [RelayCommand]
    public void Backspace()
    {
        if (CurrentInput.Expression.Length == 0)
            return;
        
        CurrentInput.Expression = CurrentInput.Expression[..^1];
        //TODO
    }

    private CalculationViewModel CalculationFactory(string expr = "")
    {
        var calc = new CalculationViewModel(_evaluationContext, expr);
        calc.RemoveAction += model =>
        {
            if (History.Count > 1)
                History.Remove(model);
        };

        return calc;
    }
    
    //private void RunCalculation()
    //{
    //    var parsed = Parser.ParseExpression(InputText);

    //    if (!parsed.WasSuccessful)
    //    {
    //        Output = double.NaN;
    //    }
    //    else
    //    {
    //        Output = parsed.Value.Evaluate(_evaluationContext);
    //    }
    //}
}