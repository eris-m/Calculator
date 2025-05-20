using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Models;

public interface IExpression
{
    /// <summary>
    /// Similar to <c>ToString</c> but is meant for UI representation.
    /// </summary>
    string AsString();

    /// <summary>
    /// Evaluates the expression into a float.
    /// </summary>
    double Evaluate(ExecutionContext context);
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

    public double Evaluate(ExecutionContext context)
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

    public double Evaluate(ExecutionContext context)
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

public class FunctionExpression(string name, IList<IExpression> arguments) : IExpression
{
    public string Name { get; set; } = name;
    public IList<IExpression> Arguments { get; set; } = arguments;

    public string AsString()
    {
        var argList = Arguments.Select(e => e.AsString());
        return $"{Name}({argList})";
    }

    public double Evaluate(ExecutionContext ctx)
    {
        var fun = ctx.Functions.GetFunction(Name);
        if (fun == null)
            return float.NaN;

        var args = Arguments.Select(e => e.Evaluate(ctx)).ToArray();

        return fun(args);
    }
}
