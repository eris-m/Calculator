using Calculator;
using Calculator.Models;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace CalculatorTests;

public class ParserTests
{
    [Test]
    public void TestInt()
    {
        string input = "123";

        IList<Token> tokens = Tokeniser.TokeniseString(input);

        if (!Parser.ParseNext(out var output, tokens.ToArray().AsSpan()))
        {
            throw new Exception("Failed to parse!");
        }
        
        Assert.That(output.Remaining.Length, Is.EqualTo(0));

        AssertFloat(output.Expression, 123f);
    }

    [Test]
    public void TestFloat()
    {
        string input = "-5.392";

        IList<Token> tokens = Tokeniser.TokeniseString(input);

        if (!Parser.ParseNext(out var output, tokens.ToArray().AsSpan()))
        {
            throw new Exception("Failed to parse!");
        }
        
        Assert.That(output.Remaining.Length, Is.EqualTo(0));

        AssertFloat(output.Expression, -5.392f);
    }

    [TestCase("1 + 2", 1f, 2f, BinaryOperator.Add)]
    [TestCase("-5.2 * 2", -5.2f, 2f, BinaryOperator.Multiply)]
    [TestCase("2 - -3", 2f, -3f, BinaryOperator.Subtract)]
    [TestCase("1 / 3.14159", 1f, 3.14159f, BinaryOperator.Divide)]
    public void TestAddInt(string input, float a, float b, BinaryOperator @operator)
    {
        IList<Token> tokens = Tokeniser.TokeniseString(input);
        if (!Parser.ParseNext(out var output, tokens.ToArray().AsSpan()))
        {
            throw new Exception("Failed to parse!");
        }

        if (output.Expression is not BinaryOperationExpression binOp)
        {
            throw new Exception("Did not parse binary operator!");
        }
        
        AssertFloat(binOp.Left, a);
        AssertFloat(binOp.Right, b);
        Assert.That(binOp.Operator, Is.EqualTo(@operator));
    }

    [Test]
    public void TestLongExpr()
    {
        string input = "5 + 2 * 3 - 5";

        IList<Token> tokens = Tokeniser.TokeniseString(input);
        if (!Parser.ParseNext(out var output, tokens.ToArray().AsSpan()))
        {
            throw new Exception("Failed to parse!");
        }

        if (output.Expression is not BinaryOperationExpression mulExpr)
        {
            throw new Exception("Didn't parse binary expression");
        }
        
        Assert.That(mulExpr.Operator, Is.EqualTo(BinaryOperator.Multiply));

        if (mulExpr.Left is not BinaryOperationExpression addExpr)
        {
            throw new Exception("Didn't parse binary expression for the left");
        }
        Assert.That(addExpr.Operator, Is.EqualTo(BinaryOperator.Add));
        AssertFloat(addExpr.Left, 5);
        AssertFloat(addExpr.Right, 2);

        if (mulExpr.Right is not BinaryOperationExpression subExpr)
        {
            throw new Exception("Didn't parse binary expression for the right");
        }
        Assert.That(subExpr.Operator, Is.EqualTo(BinaryOperator.Subtract));
        AssertFloat(subExpr.Left, 3);
        AssertFloat(subExpr.Right, 5);
    }
   
    private static void AssertFloat(IExpression expression, float expected, float tolerance = 0.001f)
    {
        if (expression is not FloatExpression floatExpr)
        {
            throw new Exception("Didn't parse a float.");
        }
        
        Assert.That(floatExpr.Value, Is.EqualTo(expected).Within(tolerance));
    }
}