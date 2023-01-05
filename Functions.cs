namespace ProgrammerCalcMAUI;

public partial class Calculator
{
    public class Functions
    {
        /// <summary>
        /// Null
        /// </summary>
        public static string Null(long[] param, out long result)
        {
            result = 0;
            return "";
        }

        /// <summary>
        /// Exponent
        /// </summary>
        public static string Power(long[] param, out long result)
        {
            result = 0;
            if (param.Length != 2)
                return SyntaxError;

            result = (long)Math.Pow(param[0], param[1]);
            return "";
        }

        /// <summary>
        /// Square root
        /// </summary>
        public static string Sqrt(long[] param, out long result)
        {
            result = 0;
            if (param.Length != 1 || param[0] < 0)
                return SyntaxError;

            result = (long)Math.Sqrt(param[0]);
            return "";
        }
    }
}
