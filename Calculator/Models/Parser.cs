using System;
using Calculator.Models;
using Sprache;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace Calculator;

/// <summary>
///     Static class to parse expressions out of strings.
/// </summary>
/// <remarks>
/// To parse an expression, call <c cref="ParseExpression">ParseExpression</c>.
/// 
/// For example:
/// <example>
/// <code>
/// const string myExpression = "1 + 2 + 3 * 4";
/// IResult&lt;IExpression&gt; result = Parser.ParseExpression(myExpression);
/// 
/// // Check that it parsed
/// if (!result.WasSuccessful)
/// {
///     // handle error.
/// }
/// 
/// IExpression expression = result.Value;
/// </code>
/// </example>
/// </remarks>
/// <see cref="ParseExpression"/>
public static class Parser
{
    /// <summary>
    ///     Parses the expression in the provided string.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    public static IResult<IExpression> ParseExpression(string input)
    {
        return AddExpressionParser()(new Input(input));
    }

    private static Parser<IExpression> FloatParser()
    {
        return
            from negative in Parse.Char('-').Optional()
            from first in Parse.Number
            from period in Parse.Char('.').Optional()
            from second in Parse.Number.Optional()
            select new FloatExpression(CreateFloat(negative, first, second));
        // return Parse.Number.Select(str => new FloatExpression(float.Parse(str)));
    }

    private static Parser<IExpression> AddExpressionParser()
    {
        return Parse.ChainOperator(
            Parse.Chars('+', '-').Token(),
            MulExpressionParser().Token(),
            (op, l, r) =>
            {
                return op switch
                {
                    '+' => new BinaryOperationExpression(BinaryOperator.Add, l, r),
                    '-' => new BinaryOperationExpression(BinaryOperator.Subtract, l, r),
                    _ => throw new Exception("Invalid parsing state!")
                };
            }
        );
    }

    private static Parser<IExpression> MulExpressionParser()
    {
        return Parse.ChainOperator(
            Parse.Chars('*', '×', '/', '÷').Token(), 
                FloatParser().Token(),
                (op, l, r) =>
                {
                    return op switch
                    {
                        '*' or '×' => new BinaryOperationExpression(BinaryOperator.Multiply, l, r),
                        '/' or '÷' => new BinaryOperationExpression(BinaryOperator.Divide, l, r),
                        _ => throw new Exception("Invalid parsing state!")
                    };
                }
            );
    }

    private static double CreateFloat(IOption<char> negative, string whole, IOption<string> fraction)
    {
        var negativeMultiplier = negative.IsDefined ? -1f : 1f;
        var fractionS = fraction.IsEmpty ? "0" : fraction.Get();
        
        var wholeF = double.Parse(whole);
        var fractionF = double.Parse(fractionS);

        var digits = fractionS.Length;
        var fractionalMultiplier = MathF.Pow(10f, -digits);
        
        return negativeMultiplier * (wholeF + fractionalMultiplier * fractionF);
    }
}