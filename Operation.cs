namespace ProgrammerCalcMAUI;

public partial class Calculator
{
    public class Operation
    {
        /// <summary>
        /// Null
        /// </summary>
        public static string Null(long operand1, long operand2, out long result)
        {
            result = 0;
            return "";
        }

        /// <summary>
        /// Multiply
        /// </summary>
        public static string Multiply(long operand1, long operand2, out long result)
        {
            result = operand1 * operand2;
            return "";
        }

        /// <summary>
        /// Divide
        /// </summary>
        public static string Divide(long operand1, long operand2, out long result)
        {
            result = 0;
            if (operand2 == 0)
                return "Division by zero";
            result = operand1 / operand2;
            return "";
        }

        /// <summary>
        /// Modulus
        /// </summary>
        public static string Modulus(long operand1, long operand2, out long result)
        {
            result = 0;
            if (operand2 == 0)
                return "Division by zero";
            result = operand1 % operand2;
            return "";
        }

        /// <summary>
        /// Add
        /// </summary>
        public static string Add(long operand1, long operand2, out long result)
        {
            result = operand1 + operand2;
            return "";
        }

        /// <summary>
        /// Subtract
        /// </summary>
        public static string Subtract(long operand1, long operand2, out long result)
        {
            result = operand1 - operand2;
            return "";
        }

        /// <summary>
        /// Shift left
        /// </summary>
        public static string ShiftLeft(long operand1, long operand2, out long result)
        {
            result = 0;
            if (operand1 < 0)
                return SyntaxError;

            result = operand1 << (int)operand2;
            return "";
        }

        /// <summary>
        /// Shift right
        /// </summary>
        public static string ShiftRight(long operand1, long operand2, out long result)
        {
            result = 0;
            if (operand2 < 0)
                return SyntaxError;

            result = operand1 >> (int)operand2;
            return "";
        }

        /// <summary>
        /// AND
        /// </summary>
        public static string And(long operand1, long operand2, out long result)
        {
            result = operand1 & operand2;
            return "";
        }

        /// <summary>
        /// XOR
        /// </summary>
        public static string Xor(long operand1, long operand2, out long result)
        {
            result = operand1 ^ operand2;
            return "";
        }

        /// <summary>
        /// OR
        /// </summary>
        public static string Or(long operand1, long operand2, out long result)
        {
            result = operand1 | operand2;
            return "";
        }
    }
}
