using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Utilities;
using Sprache;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace Calculator.Models;

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
        return ExpressionParser()(new Input(input));
    }

    private static Parser<IExpression> ExpressionParser()
    {
        return AddExpressionParser();
    }

    private static Parser<IExpression> TermParser()
    {
        return FunctionParser().Or(ParenthesisParser().Or(FloatParser()));
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
                TermParser().Token(),
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

    private static Parser<IExpression> FunctionParser() =>
        from name in IdentifierParser()
        from open in Parse.Char('(')
        from exprs in ArgsParser()
        from close in Parse.Char(')')
        select new FunctionExpression(name, exprs);

    private static Parser<IList<IExpression>> ArgsParser() => 
        ExpressionParser().Token().DelimitedBy(Parse.Char(',')).Select(x => x.ToList());
    

    private static Parser<string> IdentifierParser()
    {
        //throw new NotImplementedException();
        return Parse.Letter.AtLeastOnce().Select(x => new string(x.ToArray()));
    }

    private static Parser<IExpression> FloatParser() => 
        from negative in Parse.Char('-').Optional()
        from first in Parse.Number
        from period in Parse.Char('.').Optional()
        from second in Parse.Number.Optional()
        select new FloatExpression(CreateFloat(negative, first, second));

    private static Parser<IExpression> ParenthesisParser() =>
        from open in Parse.Char('(')
        from body in ExpressionParser()
        from close in Parse.Char(')')
        select body;

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