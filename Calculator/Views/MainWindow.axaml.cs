using Avalonia.Controls;
using Avalonia.Layout;

namespace Calculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public static Button GridButtonFactory()
    {
        var button = new Button
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        return button;
    }
}