using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Calculator.Controls;

public class GridButton : TemplatedControl
{
    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<GridButton, string>(nameof(Value));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<GridButton, ICommand?>(nameof(Command));
    
    public string Value 
    { 
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}