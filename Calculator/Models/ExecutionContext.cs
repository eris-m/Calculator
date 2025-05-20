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
    private Dictionary<string, FunctionDescription> _functions;

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

    private void DefaultFunctions()
    {
        // maybe set default length.
        _functions = [];

        _functions["sqrt"] = new FunctionDescription(1, (xs) => double.Sqrt(xs[0]));
        _functions["floor"] = new FunctionDescription(1, (xs) => double.Floor(xs[0]));
        _functions["ceil"] = new FunctionDescription(1, (xs) => double.Ceiling(xs[0]));
    }
}
