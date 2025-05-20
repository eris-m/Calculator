using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Models;

/// <summary>
/// Interface for all expressions.
/// </summary>
public interface IExpression
{
    /// <summary>
    /// Similar to <c>ToString</c> but is meant for UI representation.
    /// </summary>
    // Not really used
    string AsString();

    /// <summary>
    /// Evaluates the expression into a double.
    /// </summary>
    /// <param name="context">The context to evaluate the function in.</param>
    double Evaluate(EvaluationContext context);
}

/// <summary>
/// A simple number expression.
/// </summary>
/// <param name="value">The value of the expression.</param>
public class FloatExpression(double value) : IExpression
{
    /// <summary>
    /// The floating point value of the expression.
    /// </summary>
    public double Value { get; set; } = value;
    
    public string AsString()
    {
        return Value.ToString();
    }

    public double Evaluate(EvaluationContext context)
    {
        return Value;
    }
}

/// <summary>
/// An expression representing a binary operation.
/// </summary>
public class BinaryOperationExpression : IExpression
{
    /// <summary>
    /// Binary operator kind.
    /// </summary>
    public enum BinaryOperator
    {
        /// <summary>
        /// Addition.
        /// </summary>
        Add,
        /// <summary>
        /// Subtraction.
        /// </summary>
        Subtract,
        /// <summary>
        /// Multiplication.
        /// </summary>
        Multiply,
        /// <summary>
        /// Division.
        /// </summary>
        Divide
    }

    /// <summary>
    /// The operator used by the expression.
    /// </summary>
    public BinaryOperator Operator { get; set; }
    /// <summary>
    /// Left hand side of the expression.
    /// </summary>
    public IExpression Left { get; set; }
    /// <summary>
    /// Right hand side of the expression.
    /// </summary>
    public IExpression Right { get; set; }

    public BinaryOperationExpression(BinaryOperator @operator, IExpression left, IExpression right)
    {
        Operator = @operator;
        Left = left;
        Right = right;
    }

    public string AsString()
    {
        var operatorChar = Operator switch
        {
            BinaryOperator.Add => '+',
            BinaryOperator.Subtract => '-',
            BinaryOperator.Multiply => '×',
            BinaryOperator.Divide => '÷',
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{Left.AsString()} {operatorChar} {Right.AsString()}";
    }

    public double Evaluate(EvaluationContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        return Operator switch
        {
            BinaryOperator.Add => left + right,
            BinaryOperator.Subtract => left - right,
            BinaryOperator.Multiply => left * right,
            BinaryOperator.Divide => left / right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

/// <summary>
/// An expression representing a function call.
/// </summary>
/// <param name="name">The name of the function.</param>
/// <param name="arguments">The arguments passed to the function.</param>
public class FunctionExpression(string name, IList<IExpression> arguments) : IExpression
{
    /// <summary>
    /// The name of the function.
    /// </summary>
    public string Name { get; set; } = name;
    /// <summary>
    /// Arguments passed to the function.
    /// </summary>
    public IList<IExpression> Arguments { get; set; } = arguments;

    public string AsString()
    {
        var argList = Arguments.Select(e => e.AsString());
        return $"{Name}({argList})";
    }

    public double Evaluate(EvaluationContext ctx)
    {
        var fun = ctx.Functions.GetFunction(Name);
        if (fun == null)
            return float.NaN;

        if (Arguments.Count != fun.Value.ExpectedArguments)
            return float.NaN;
        
        var args = Arguments.Select(e => e.Evaluate(ctx)).ToArray();

       
        return fun.Value.Fun(args);
    }
}
