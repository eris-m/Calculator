using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models;

public sealed class ExecutionContext
{
    //private FunctionRegistry _registry = new();

    public ExecutionContext()
    {
    }

    public FunctionRegistry Functions { get; set; } = new();
}

public delegate double Function(double[] numbers);

public record struct FunctionDescription(int ExpectedArguments, Function Fun);

public class FunctionRegistry
{
    private Dictionary<string, FunctionDescription> _functions = [];

    public FunctionRegistry()
    {
        DefaultFunctions();
    }

    public FunctionDescription? GetFunction(string name)
    {
        if (!_functions.TryGetValue(name, out var func))
            return null;
        return func;
    }

    public void DefineFunction(string name, FunctionDescription fn)
    {
        _functions.Add(name, fn);
    }

    public void DefineFunction(string name, Func<double, double> fn)
    {
        DefineFunction(name, new FunctionDescription(1, xs => fn(xs[0])));
    }

    public void DefineFunction(string name, Func<double, double, double> fn)
    {
        DefineFunction(name, new FunctionDescription(2, xs => fn(xs[0], xs[1])));
    }

    private void DefaultFunctions()
    {
        // maybe set default length.
        _functions = [];
        
        #region Arithmetic functions.
        DefineFunction("pow", double.Pow);
        DefineFunction("sqrt", double.Sqrt);
        DefineFunction("root", (x, n) => double.RootN(x, (int)n));
        #endregion
    
        #region Trig functions.
        // regular
        DefineFunction("sin", double.Sin);
        DefineFunction("cos", double.Cos);
        DefineFunction("tan", double.Tan);
        
        // inverse
        DefineFunction("asin", double.Asin);
        DefineFunction("acos", double.Acos);
        DefineFunction("atan", double.Atan);
        #endregion
        
        #region Misc functions.
        DefineFunction("min", double.Min);
        DefineFunction("max", double.Max);
        DefineFunction("floor", double.Floor);
        DefineFunction("ceil", double.Ceiling);
        #endregion
    }
}
