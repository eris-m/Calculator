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

public class FunctionRegistry
{
    private Dictionary<string, Function> _functions;

    public FunctionRegistry()
    {
        _functions = [];    
    }

    public Function? GetFunction(string name)
    {
        if (!_functions.TryGetValue(name, out var func))
            return null;
        return func;
    }

    public void DefineFunction(string name, Function fn)
    {
        _functions.Add(name, fn);
    }
}
