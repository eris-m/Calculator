using System.Collections.Generic;

namespace Calculator;

/// <summary>
///     The kinds of tokens possible to be tokenised.
/// </summary>
public enum TokenKind
{
    /// <summary>
    ///     Anything that doesn't fit into any other category.
    /// </summary>
    Unknown,

    /// <summary>
    ///     A digit, 0-9.
    /// </summary>
    Digit,

    /// <summary>
    ///     Plus sign.
    /// </summary>
    Add,

    /// <summary>
    ///     Minus sign.
    /// </summary>
    Sub,

    /// <summary>
    ///     Multiplication sign.
    /// </summary>
    Mul,

    /// <summary>
    ///     Division sign.
    /// </summary>
    Div,

    /// <summary>
    ///     A period.
    /// </summary>
    Period,

    /// <summary>
    ///     A comma.
    /// </summary>
    Comma,

    /// <summary>
    ///     A space.
    /// </summary>
    Space
}

/// <summary>
///     A token produced by a lexer.
/// </summary>
public struct Token(TokenKind kind, string content)
{
    /// <summary>
    ///     The kind of token.
    /// </summary>
    public TokenKind TokenKind { get; set; } = kind;

    /// <summary>
    ///     The content of the token.
    ///     All characters in this string
    /// </summary>
    public string Content { get; set; } = content;

    /// <summary>
    ///     The length of the token in characters.
    /// </summary>
    public int Length => Content.Length;

    public Token() : this(TokenKind.Unknown, "")
    {
    }
}

/// <summary>
///     Static class with methods to tokenise characters and strings.
/// </summary>
public static class Tokeniser
{
    public static TokenKind GetTokenKind(char ch)
    {
        if (char.IsAsciiDigit(ch)) return TokenKind.Digit;

        return ch switch
        {
            '+' => TokenKind.Add,
            '-' => TokenKind.Sub,
            '*' => TokenKind.Mul,
            '×' => TokenKind.Mul,
            '/' => TokenKind.Div,
            '÷' => TokenKind.Div,
            '.' => TokenKind.Period,
            ',' => TokenKind.Comma,
            ' ' => TokenKind.Space,
            _ => TokenKind.Unknown
        };
    }

    public static Token? TokeniseNext(string str, int start = 0)
    {
        if (str.Length <= start)
            return null;

        var kind = GetTokenKind(str[start]);

        var i = 1;
        for (; i + start < str.Length; i++)
            if (GetTokenKind(str[i + start]) != kind)
                break;

        return new Token(kind, str.Substring(start, i));
    }

    public static IList<Token> TokeniseString(string str, int start = 0)
    {
        List<Token> tokens = [];

        while (start <= str.Length)
        {
            var token = TokeniseNext(str, start);
            if (token == null)
                break;

            tokens.Add(token.Value);
            start += token.Value.Length;
        }

        return tokens;
    }
}