using System;
using Calculator.Models;
using Sprache;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace Calculator;

// TODO: Error handling!!!!

// /// <summary>
// ///     The output from a parser.
// /// </summary>
// public ref struct ParserOutput
// {
//     /// <summary>
//     ///     The amount of tokens consumed by the parser.
//     /// </summary>
//     public ReadOnlySpan<Token> Remaining { get; set; }
//
//     /// <summary>
//     ///     The expression parsed.
//     /// </summary>
//     public IExpression Expression { get; set; }
// }

/// <summary>
///     Static class containing methods to parse expressions.
/// </summary>
public class Parser
{
    /// <summary>
    ///     Parses the next expression in the provided string.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    public static IResult<IExpression> ParseNext(string input)
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

    private static float CreateFloat(IOption<char> negative, string whole, IOption<string> fraction)
    {
        var negativeMultiplier = negative.IsDefined ? -1f : 1f;
        var fractionS = fraction.IsEmpty ? "0" : fraction.Get();
        
        var wholeF = float.Parse(whole);
        var fractionF = float.Parse(fractionS);

        var digits = fractionS.Length;
        var fractionalMultiplier = MathF.Pow(10f, -digits);
        
        return negativeMultiplier * (wholeF + fractionalMultiplier * fractionF);
    }
}