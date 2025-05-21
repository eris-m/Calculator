using System;
using System.Collections.Generic;

namespace Calculator.Models;

public class VariableRegistry
{
    private readonly Dictionary<string, double> _variables = [];
    private readonly Dictionary<string, double> _constants = DefaultConstants();

    public bool IsDefined(string name) => _variables.ContainsKey(name) || _constants.ContainsKey(name);
    public bool IsConstant(string name) => _constants.ContainsKey(name);

    #region Get & Set
    public void AddConstant(string name, double value)
    {
        _constants.Add(name, value);
    }

    public void SetVariable(string name, double value)
    {
        _variables[name] = value;
    }

    public bool TryGet(string name, out double value)
    {
        if (!IsDefined(name))
        {
            value = 0;
            return false;
        }

        if (!IsConstant(name))
            value = _variables[name];
        else
            value = _constants[name];
        return true;
    }

    #endregion

    public double this[string name]
    {
        get
        {
            if (TryGet(name, out var value))
                return value;
            throw new KeyNotFoundException($"Variable {name} is not defined.");
        }
        set => _variables[name] = value;
    }

    private static Dictionary<string, double> DefaultConstants()
    {
        var constants = new Dictionary<string, double>();

        constants["pi"] = double.Pi;
        constants["tau"] = double.Tau;
        constants["e"] = double.E;
        constants["gold"] = (1 + double.Sqrt(5)) / 2; // golden ratio

        return constants;
    }
}