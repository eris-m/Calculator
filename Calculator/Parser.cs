using System;
using Calculator.Models;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace Calculator;

// TODO: Error handling!!!!

/// <summary>
/// The output from a parser.
/// </summary>
public ref struct ParserOutput
{
    /// <summary>
    /// The amount of tokens consumed by the parser.
    /// </summary>
    public ReadOnlySpan<Token> Remaining { get; set; }
    /// <summary>
    /// The expression parsed.
    /// </summary>
    public IExpression Expression { get; set; }
}

/// <summary>
/// Static class containing methods to parse expressions.
/// </summary>
public static class Parser
{
    /// <summary>
    /// Parses the next expression in the span of tokens.
    /// </summary>
    /// <param name="output">The output of the parser.</param>
    /// <param name="tokens">The input tokens to be parsed.</param>
    /// <returns><c>true</c> if parsing was successful, <c>false</c> otherwise</returns>
    public static bool ParseNext(out ParserOutput output, ReadOnlySpan<Token> tokens)
    {
        if (tokens.Length <= 0)
        {
            output = new ParserOutput();
            return false;
        }

        if (ParseMathsExpression(out output, tokens))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Parses a single float or integer.
    /// </summary>
    /// <param name="output">The output of the parser.</param>
    /// <param name="tokens">The input tokens.</param>
    /// <returns><c>true</c> if parsing was successful, <c>false</c> otherwise.</returns>
    public static bool ParseFloat(out ParserOutput output, ReadOnlySpan<Token> tokens)
    {
        if (tokens.Length <= 0)
        {
            output = new ParserOutput();
            return false;
        }

        
        bool negative = ExpectToken(TokenKind.Sub, tokens, out tokens);
        float negativeMultiplier = negative ? -1f : 1f;

        if (!ParseDigit(out uint whole, tokens, out tokens))
        {
            output = new ParserOutput();
            return false;
        }

        if (!ExpectToken(TokenKind.Period, tokens, out tokens))
        {
            output = new ParserOutput
            {
                Expression = new FloatExpression(negativeMultiplier * whole),
                Remaining = tokens
            };
            return true;
        }

        
        if (!ParseDigit(out uint fraction, tokens, out var remaining))
        {
            output = new ParserOutput
            {
                Expression = new FloatExpression(negativeMultiplier * whole),
                Remaining = tokens,
            };
            return true;
        }

        float fractionMultiplier = 1f / float.Pow(10f, tokens[0].Length);

        output = new ParserOutput
        {
            Expression = new FloatExpression(negativeMultiplier * (whole + (fraction * fractionMultiplier))),
            Remaining = remaining,
        };
        return true;
    }

    private static bool ParseMathsExpression(out ParserOutput output, ReadOnlySpan<Token> tokens)
    {
        if (tokens.Length == 0)
        {
            output = new ParserOutput();
            return false;
        }

        if (!ParseAdditionTerm(out var firstFloat, tokens))
        {
            output = new ParserOutput();
            return false;
        }

        tokens = SkipSpaces(firstFloat.Remaining);
        
        // ew.
        // TODO: Replace this.
        BinaryOperator @operator = BinaryOperator.Multiply;
        if (!ExpectToken(TokenKind.Mul, tokens, out tokens))
        {
            if (!ExpectToken(TokenKind.Div, tokens, out tokens))
            {
                output = firstFloat;
                return true;
            }

            @operator = BinaryOperator.Divide;
        }

        tokens = SkipSpaces(tokens);
        if (!ParseNext(out var secondFloat, tokens))
        {
            output = new ParserOutput();
            return false;
        }

        output = new ParserOutput
        {
            Expression = new BinaryOperationExpression(@operator, firstFloat.Expression, secondFloat.Expression),
            Remaining = secondFloat.Remaining,
        };
        return true;
    }
    
    private static bool ParseAdditionTerm(out ParserOutput output, ReadOnlySpan<Token> tokens)
    {
        if (tokens.Length == 0)
        {
            output = new ParserOutput();
            return false;
        }

        if (!ParseFloat(out ParserOutput firstFloat, tokens))
        {
            output = new ParserOutput();
            return false;
        }

        tokens = SkipSpaces(firstFloat.Remaining);

        BinaryOperator @operator = BinaryOperator.Add;
        // ew.
        // TODO: Replace this.
        if (!ExpectToken(TokenKind.Add, tokens, out tokens))
        {
            if (!ExpectToken(TokenKind.Sub, tokens, out tokens))
            {
                output = firstFloat;
                return true;
            }

            @operator = BinaryOperator.Subtract;
        }

        tokens = SkipSpaces(tokens);

        if (!ParseNext(out ParserOutput secondFloat, tokens))
        {
            output = new ParserOutput();
            return false; 
        }

        output = new ParserOutput
        {
            Expression = new BinaryOperationExpression(@operator, firstFloat.Expression, secondFloat.Expression),
            Remaining = secondFloat.Remaining,
        };

        return true;
    }

    private static ReadOnlySpan<Token> SkipSpaces(ReadOnlySpan<Token> tokens)
    {
        int i;
        for (i = 0; i < tokens.Length; i++)
        {
            if (tokens[i].TokenKind != TokenKind.Space)
                break;
        }

        return tokens[i..];
    }
    
    private static bool ExpectToken(TokenKind expected, ReadOnlySpan<Token> tokens, out ReadOnlySpan<Token> outTokens)
    {
        if (tokens.Length == 0)
        {
            outTokens = tokens;
            return false;
        }

        bool eq = tokens[0].TokenKind == expected;

        outTokens = tokens[(eq ? 1 : 0)..];
        return eq;
    }
    
    private static bool ParseDigit(out uint output, ReadOnlySpan<Token> tokens, out ReadOnlySpan<Token> outTokens)
    {
        if (tokens.Length == 0 || tokens[0].TokenKind != TokenKind.Digit)
        {
            output = 0;
            outTokens = tokens;
            return false;
        }

        outTokens = tokens[1..]; 
        return uint.TryParse(tokens[0].Content, out output);
    }
}