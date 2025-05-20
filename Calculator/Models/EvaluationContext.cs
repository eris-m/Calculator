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
