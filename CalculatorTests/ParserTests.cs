using Calculator.Models;
using Sprache;
using BinaryOperator = Calculator.Models.BinaryOperationExpression.BinaryOperator;

namespace CalculatorTests;

public class ParserTests
{
    [Test]
    public void TestInt()
    {
        const string input = "123";
        var result = Parser.ParseExpression(input);

        if (!result.WasSuccessful)
        {
            ParserError(input);
        }
        
        Assert.That(result.Remainder.AtEnd);

        AssertFloat(result.Value, 123f);
    }

    [Test]
    public void TestFloat()
    {
        const string input = "-5.392";

        var result = Parser.ParseExpression(input);

        if (!result.WasSuccessful)
        {
            ParserError(input);
        }
        
        Assert.That(result.Remainder.AtEnd);

        AssertFloat(result.Value, -5.392f);
    }

    [TestCase("1 + 2", 1f, 2f, BinaryOperator.Add)]
    [TestCase("-5.2 * 2", -5.2f, 2f, BinaryOperator.Multiply)]
    [TestCase("2 - -3", 2f, -3f, BinaryOperator.Subtract)]
    [TestCase("1 / 3.14159", 1f, 3.14159f, BinaryOperator.Divide)]
    public void TestAdd(string input, float a, float b, BinaryOperator @operator)
    {
        var result = Parser.ParseExpression(input);
        
        if (!result.WasSuccessful)
        {
            ParserError(input);
        }

        if (result.Value is not BinaryOperationExpression binOp)
        {
            throw new Exception("Did not parse binary operator!");
        }
        
        AssertFloat(binOp.Left, a);
        AssertFloat(binOp.Right, b);
        Assert.That(binOp.Operator, Is.EqualTo(@operator));
    }

    [Test]
    public void TestFunction()
    {
        const string input = "sqrt(2)";
        var result = Parser.ParseExpression(input);

        if (!result.WasSuccessful)
            throw new Exception("Failed to parse!");

        if (result.Value is not FunctionExpression fun)
            throw new Exception("Did not parse function!");

        Assert.That(fun.Name, Is.EqualTo("sqrt"));
        Assert.That(fun.Arguments, Has.Count.EqualTo(1));
    }

    [TestCase("1 + 2*3", 7f)]
    [TestCase("(1 + 2)*3", 9f)]
    [TestCase("1 + 2 * 3 + 4", 11f)]
    public void TestExpression(string input, float expected)
    {
        var executionCtx = new Calculator.Models.ExecutionContext();

        var result = Parser.ParseExpression(input);
        if (!result.WasSuccessful)
        {
            ParserError(input);
        }

        var eval = result.Value.Evaluate(executionCtx);

        Assert.That(eval, Is.EqualTo(expected).Within(0.005f));
    }

    private static void ParserError(string expression)
    {
        throw new Exception($"Failed to parse expression: \"{expression}\"");
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