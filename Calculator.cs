namespace ProgrammerCalcMAUI;

public partial class Calculator
{
    /// <summary>
    /// Function delegate
    /// </summary>
    /// <param name="param">Parameters</param>
    /// <param name="result">Result</param>
    /// <returns></returns>
    private delegate string Function(long[] param, out long result);

    /// <summary>
    /// Operator delegate
    /// </summary>
    /// <param name="operand1">Operand 1</param>
    /// <param name="operand2">Operand 2</param>
    /// <param name="result">Result</param>
    /// <returns></returns>
    private delegate string Operator(long operand1, long operand2, out long result);

    /// <summary>
    /// Syntax error message
    /// </summary>
    private static string SyntaxError = "Syntax error";

    /// <summary>
    /// Functions
    /// </summary>
    private static Dictionary<string, Function> functions = new Dictionary<string, Function>
        {
            { "pow", Functions.Power },
            { "sqrt", Functions.Sqrt }
        };

    /// <summary>
    /// Operators
    /// </summary>
    private static Dictionary<string, Operator> OperatorDict = new Dictionary<string, Operator>
        {
            { "*", Operation.Multiply },
            { "/", Operation.Divide },
            { "%", Operation.Modulus },
            { "+", Operation.Add },
            { "-", Operation.Subtract },
            { "<<", Operation.ShiftLeft },
            { ">>", Operation.ShiftRight },
            { "&", Operation.And },
            { "^", Operation.Xor },
            { "|", Operation.Or }
        };

    /// <summary>
    /// Subexpression types
    /// </summary>
    private enum SubexprType
    {
        Root,
        Sub,
        Function
    }

    /// <summary>
    /// Expression node
    /// </summary>
    private struct ExpressionNode
    {
        public ExpressionNode(long number, Operator oper)
        {
            Number = number;
            Operator = oper;
        }

        /// <summary>
        /// Number
        /// </summary>
        public long Number;

        /// <summary>
        /// Operator
        /// </summary>
        public Operator Operator;
    }

    /// <summary>
    /// Calculate an expression
    /// </summary>
    /// <param name="expression">Expression to calculate</param>
    /// <param name="result">Result</param>
    /// <param name="error">Error message</param>
    /// <returns>True if successful, false if error</returns>
    public static bool Calculate(string expression, out long result, out string error)
    {
        int exprPos = 0;
        bool success = CalculateRecursive(expression.ToLower(), ref exprPos, SubexprType.Root, out result, out error);
        if (!success)
            result = 0;
        return success;
    }

    /// <summary>
    /// Calculation function
    /// </summary>
    /// <param name="expression">Expression to calculate</param>
    /// <param name="exprPos">Position in expression</param>
    /// <param name="subexprType">Subexpression type</param>
    /// <param name="result">Result</param>
    /// <param name="error">Error message</param>
    /// <returns>True if successful, false if error</returns>
    private static bool CalculateRecursive(string expression, ref int exprPos, SubexprType subexprType, out long result, out string error)
    {
        error = "";
        result = 0;

        List<ExpressionNode> nodes = new List<ExpressionNode>();
        KeyValuePair<string, Operator> oper;
        KeyValuePair<string, Function> func;

        bool subexprDone = false;
        bool whitespace = false;
        bool gotNumber = true;
        string numStr = "";

        while (!subexprDone && exprPos < expression.Length)
        {
            char c = expression[exprPos++];
            if (c == '(')
            {
                // Left parenthesis
                if (!CalculateRecursive(expression, ref exprPos, SubexprType.Sub, out result, out error))
                    return false;
                gotNumber = false;
            }
            else if (c == ')' || (c == ',' && subexprType == SubexprType.Function))
            {
                // Right parenthesis
                subexprDone = true;
            }
            else if (FindFunction(expression, exprPos - 1, out func))
            {
                // Function
                exprPos += func.Key.Length;

                // Get function parameters
                List<long> funcParams = new List<long>();
                bool gotParams = true;
                while (gotParams)
                {
                    if (!CalculateRecursive(expression, ref exprPos, SubexprType.Function, out result, out error))
                        return false;
                    funcParams.Add(result);

                    if (expression[exprPos - 1] != ',')
                        gotParams = false;
                }

                // Run function
                error = func.Value(funcParams.ToArray(), out result);
                if (error.Length > 0)
                    return false;
                gotNumber = false;
            }
            else if (c == '-' && numStr.Length == 0)
            {
                // Negative sign
                numStr += c.ToString();
            }
            else if (" \t\r\n".Contains(c))
            {
                // Whitespace
                if (numStr.Length > 0)
                    whitespace = true;
            }
            else if (FindOperator(expression, exprPos - 1, out oper))
            {
                // Operator
                if (gotNumber)
                {
                    error = ParseNumber(numStr, out result);
                    if (error.Length > 0)
                        return false;
                }

                nodes.Add(new ExpressionNode(result, oper.Value));
                numStr = "";
                whitespace = false;
                exprPos += oper.Key.Length - 1;
            }
            else
            {
                // Digit
                if (!"0123456789abcdefox".Contains(char.ToLower(c)))
                {
                    // Invalid digit
                    error = SyntaxError;
                    return false;
                }
                if (whitespace)
                {
                    // Space in number
                    error = SyntaxError;
                    return false;
                }
                numStr += c;
                gotNumber = true;
            }
        }

        // Check inappropriate expression end
        if ((subexprDone && subexprType == SubexprType.Root) ||
            (!subexprDone && subexprType != SubexprType.Root))
        {
            error = SyntaxError;
            return false;
        }

        // Check if there's still a number queued up
        if (gotNumber)
        {
            error = ParseNumber(numStr, out result);
            if (error.Length > 0)
                return false;
        }

        // Run through order of operators
        nodes.Add(new ExpressionNode(result, Operation.Null));
        foreach (KeyValuePair<string, Operator> operChk in OperatorDict)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Operator == operChk.Value)
                {
                    ExpressionNode leftNode = nodes[i];
                    ExpressionNode rightNode = nodes[i + 1];
                    ExpressionNode value = new ExpressionNode(0, rightNode.Operator);

                    error = leftNode.Operator(leftNode.Number, rightNode.Number, out value.Number);
                    if (error.Length > 0)
                        return false;

                    nodes[i] = value;
                    nodes.RemoveAt(i-- + 1);
                }
            }
            if (nodes.Count <= 1)
                break;
        }

        if (nodes.Count != 1)
        {
            error = "Catastrophic failure";
            return false;
        }
        result = nodes[0].Number;
        return true;
    }

    /// <summary>
    /// Find operator in expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="exprPos">Position in expression</param>
    /// <param name="oper">Found operator</param>
    /// <returns>True if found, flase if not</returns>
    private static bool FindOperator(string expression, int exprPos, out KeyValuePair<string, Operator> oper)
    {
        foreach (KeyValuePair<string, Operator> keyValuePair in OperatorDict)
        {
            if (expression.IndexOf(keyValuePair.Key, exprPos) == exprPos)
            {
                oper = keyValuePair;
                return true;
            }
        }
        oper = new KeyValuePair<string, Operator>("", Operation.Null);
        return false;
    }

    /// <summary>
    /// Find function in expression
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="exprPos">Position in expression</param>
    /// <param name="func">Found function</param>
    /// <returns>True if found, flase if not</returns>
    private static bool FindFunction(string expression, int exprPos, out KeyValuePair<string, Function> func)
    {
        foreach (KeyValuePair<string, Function> keyValuePair in functions)
        {
            if (expression.IndexOf(keyValuePair.Key, exprPos) == exprPos &&
                expression[exprPos + keyValuePair.Key.Length] == '(')
            {
                func = keyValuePair;
                return true;
            }
        }
        func = new KeyValuePair<string, Function>("", Functions.Null);
        return false;
    }

    /// <summary>
    /// Parse number
    /// </summary>
    /// <param name="numberStr">Number string</param>
    /// <param name="numberOut">Parsed number</param>
    /// <returns>Error message if failed</returns>
    private static string ParseNumber(string numberStr, out long numberOut)
    {
        numberOut = 0;
        if (numberStr.Length == 0)
            return SyntaxError;

        bool negative = false;
        bool gotPrefix = false;
        bool gotDigit = false;
        long numBase = 10;
        string validDigits = "0123456789";

        for (int i = 0; i < numberStr.Length; i++)
        {
            if (numberStr[i] == '-' && i == 0)
            {
                // Negative sign
                negative = true;
            }
            else if (numberStr[i] == '0' && !gotPrefix && i < numberStr.Length - 1 &&
                "box".Contains(char.ToLower(numberStr[i + 1]).ToString()))
            {
                // Prefix
                switch (char.ToLower(numberStr[++i]))
                {
                    case 'b':
                        // Binary
                        numBase = 2;
                        validDigits = "01";
                        break;

                    case 'o':
                        // Octal
                        numBase = 8;
                        validDigits = "01234567";
                        break;

                    case 'x':
                        // Hexadecimal
                        numBase = 16;
                        validDigits = "0123456789abcdef";
                        break;
                }
                gotPrefix = true;
            }
            else
            {
                // Digit
                if (!validDigits.Contains(char.ToLower(numberStr[i])))
                    return SyntaxError;

                long digit = char.ToLower(numberStr[i]);
                if (digit >= 'a')
                    digit = digit - 'a' + 10;
                else
                    digit -= '0';

                gotDigit = true;
                numberOut = (numberOut * numBase) + digit;
            }
        }

        if (!gotDigit)
            return SyntaxError;
        numberOut *= negative ? -1 : 1;
        return "";
    }
}
