using Calculator;
using static Calculator.Tokeniser;

namespace CalculatorTests;

public class TokeniserTests
{
    [TestCase(TokenKind.Digit, "0123456789")]
    [TestCase(TokenKind.Add, "+")]
    [TestCase(TokenKind.Sub, "-")]
    [TestCase(TokenKind.Mul, "*")]
    [TestCase(TokenKind.Div, "/")]
    [TestCase(TokenKind.Period, ".")]
    [TestCase(TokenKind.Comma, ",")]
    [TestCase(TokenKind.Space, " ")]
    public void TokeniseCharTest(TokenKind kind, string chars)
    {
        foreach (char c in chars)
        {
            Assert.That(GetTokenKind(c), Is.EqualTo(kind));
        }
    }

    [Test]
    public void TokeniseNextTest()
    {
        string input = "123.";
        Token? token = TokeniseNext(input);
        
        Assert.That(token, Is.Not.Null);
        Assert.That(token.Value.Length, Is.EqualTo(3));
        Assert.That(token.Value.TokenKind, Is.EqualTo(TokenKind.Digit));
    }

    [Test]
    public void TokeniseStringTest()
    {
        string input = "123.456";
        IList<Token> tokens = TokeniseString(input);
        
        Assert.That(tokens, Has.Count.EqualTo(3));
        
        Assert.Multiple(() =>
        {
            Assert.That(tokens[0].Length, Is.EqualTo(3));
            Assert.That(tokens[0].TokenKind, Is.EqualTo(TokenKind.Digit));
            
            Assert.That(tokens[1].Length, Is.EqualTo(1));
            Assert.That(tokens[1].TokenKind, Is.EqualTo(TokenKind.Period));
        
            Assert.That(tokens[2].Length, Is.EqualTo(3));
            Assert.That(tokens[2].TokenKind, Is.EqualTo(TokenKind.Digit));
        });
    }
}