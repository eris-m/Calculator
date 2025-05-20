using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models;

/// <summary>
/// The context for evaluating expressions.
/// Stores state like variable and function names.
/// </summary>
public sealed class EvaluationContext
{
    //private FunctionRegistry _registry = new();

    public EvaluationContext()
    {
    }

    /// <summary>
    /// The functions registered.
    /// </summary>
    public FunctionRegistry Functions { get; set; } = new();
}

/// <summary>
/// The method for a function.
/// </summary>
/// <param name="numbers">
/// The arguments to the function.
/// Will not be more than <c>ExpectedArguments</c> in the function's <c>FunctionDescription</c>.
/// </param>
/// <returns>
/// The value of the function.
/// </returns>
///
/// <see cref="FunctionDescription"/>
public delegate double Function(double[] numbers);

/// <summary>
/// The description of a function.
/// </summary>
/// <param name="ExpectedArguments">The number of arguments that passed to the function.</param>
/// <param name="Fun">The function to call.</param>
public record struct FunctionDescription(int ExpectedArguments, Function Fun);

/// <summary>
/// A registry of functions.
/// </summary>
///
/// <see cref="FunctionDescription" />
public class FunctionRegistry
{
    private Dictionary<string, FunctionDescription> _functions = [];

    /// <summary>
    /// Creates a function registry with default functions.
    /// </summary>
    public FunctionRegistry()
    {
        DefaultFunctions();
    }

    /// <summary>
    /// Tries to get a function by the given name.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <returns><c>null</c> if the function does not exist.</returns>
    public FunctionDescription? GetFunction(string name)
    {
        if (!_functions.TryGetValue(name, out var func))
            return null;
        return func;
    }

    /// <summary>
    /// Defines a function.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="fn">The description of the function.</param>
    public void DefineFunction(string name, FunctionDescription fn)
    {
        _functions.Add(name, fn);
    }

    /// <summary>
    /// Defines a function with one argument.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="fn">The function to call.</param>
    public void DefineFunction(string name, Func<double, double> fn)
    {
        DefineFunction(name, new FunctionDescription(1, xs => fn(xs[0])));
    }

    /// <summary>
    /// Defines a function with two arguments.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="fn">The function to call.</param>
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
