using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/*
 Week 4 Challenge:

Evaluate a mathematical expression. For example, if str were "2+(3-1)*3" the output should be 8. Another example: if str were "(2-0)(6/2)" 
the output should be 6. There can be parenthesis within the string so you must evaluate it properly according to the rules of arithmetic. 
The string will contain the operators: +, -, /, *, (, and ). If you have a string like this: #/#*# or #+#(#)/#, then evaluate from left to 
right. So divide then multiply, and for the second one multiply, divide, then add. 
The evaluations will be such that there will not be any decimal operations, so you do not need to account for rounding and whatnot.

 */
namespace CodeChallenge04_MathExpressions
{
	class Program
	{
		public static void Main()
		{
			string input;
			do
			{
				Console.WriteLine("\n\nEnter a mathmatical expression using only +, -, /, *, (, ), and 0-9 (q to quit):");
				input = Console.ReadLine();
				Console.WriteLine("\nResult: {0}", EvaluateMathExpression(input));
			} while (input != "q");
		}

		public static string EvaluateMathExpression(string input)
		{
			input = input.RemoveAllSpaces();
			if (input.IsValidMathmaticalExpression())
			{
				return String.Format("{0} = {1}", input, input.EvaluateMathmaticalExpression());
			}
			return "Invalid Mathmatical Expression. Make sure you have balanced parens and only +-*/()0-9 characters in the proper order";
		}
	}

	public static class MathmaticalExpressionExtensions
	{
		public static bool IsValidMathmaticalExpression(this string input)
		{
			return input.ContainsOnlyMathExpressionCharacters() &&
					input.HasBalancedParens();
		}

		// can only contain numbers and/or add, subtract, multiply, divide operators, i.e. +-*/()0-9
		public static bool ContainsOnlyMathExpressionCharacters(this string input)
		{
            if (input == null)
                return false;
            var isValid = Regex.IsMatch(input, @"^[+*\-/()0-9]+$");
			return isValid;
		}

		// e.g. (()) would pass, but (() or )( would not
		public static bool HasBalancedParens(this string input)
		{
			var parenCount = 0;
			foreach (var c in input)
			{
				switch (c)
				{
					case '(':
						parenCount++;
						break;
					case ')':
						if (parenCount == 0)
							return false;
						parenCount--;
						break;
				}
			}
			var isBalanced = parenCount == 0;
			return isBalanced;
		}

		// assumes IsValidMathmaticalExpression() has passed at this point, and that spaces are stripped
		public static string EvaluateMathmaticalExpression(this string input)
		{
            input = input.ReplaceMultiplierParens(); // insert a * operator when necessary so that multiplying with parens can be handled more gracefully
            var currentExpressionStr = new StringBuilder(input);
			if (currentExpressionStr.Length > 0 && currentExpressionStr.ToString().ContainsOnlyNumbers())
			{
				return input;
			}

            var expression = input; 
            expression = currentExpressionStr.EvaluateExpressionWithParens(expression); // evaluate parens first
			expression = currentExpressionStr.EvaluateExpressionNoParens(expression);
			
			return currentExpressionStr.ToString();
		}

        public static string EvaluateExpressionWithParens(this StringBuilder currentExpressionStr, string input)
        {
            var expression = input;

            while (expression.Contains('(') || expression.Contains(')'))
            {
                expression = EvaluateExpressionInstanceWithParens(currentExpressionStr, expression);
            }

            return currentExpressionStr.ToString();
        }

		public static string EvaluateExpressionInstanceWithParens(this StringBuilder currentExpressionStr, string input)
		{
			var parenExpression = new StringBuilder();
            var leftParenIndex = -1;
            var rightParenIndex = -1;
            for (var i = 0; i < input.Length; i++)
            {
                var currentChar = input[i];
                if (currentChar == '(')
                {
                    parenExpression.Clear();
                    leftParenIndex = i;
                }
                else if (currentChar == ')')
                {
                    rightParenIndex = i;
                    break;
                }
                else
                {
                    parenExpression.Append(currentChar);
                }
            }

            // replace the innermost parens with evaluated expression, e.g. 5*(4+3+2) becomes 5*9
            parenExpression.EvaluateExpressionNoParens(parenExpression.ToString());
            currentExpressionStr.Remove(leftParenIndex, rightParenIndex - leftParenIndex + 1);
            currentExpressionStr.Insert(leftParenIndex, parenExpression.ToString());

            return currentExpressionStr.ToString();
		}

		public static string EvaluateExpressionNoParens(this StringBuilder currentExpressionStr, string input)
		{
			var expression = input;

			do
			{
				expression = EvaluateExpressionInstanceNoParens(currentExpressionStr, expression, new[] { '*', '/' });
			} while (expression.Contains('*') || expression.Contains('/'));

			do
			{
				expression = EvaluateExpressionInstanceNoParens(currentExpressionStr, expression, new[] { '+', '-' });
			} while (expression.Contains('+') || expression.Substring(1).Contains('-'));

			return currentExpressionStr.ToString();
		}

		public static string EvaluateExpressionInstanceNoParens(this StringBuilder currentExpressionStr, string input, char[] operators)
		{
			for (var i = 0; i < input.Length; i++)
			{
				var currentChar = input[i];
				if (operators.Contains(currentChar) && i != 0)
				{
					EvaluateExpressionInstanceByOperator(currentExpressionStr, input, i);
					break;
				}
			}

			return currentExpressionStr.ToString();
		}

		/// <summary>
		/// For a given operator symbol, figure out the left and right operands, then replace the entire single expression with the calculated result
		/// E.g. Given expression 2*3+5+4+2*3 with an operator index of 1, the output would be 6+5+4+2*3
		/// </summary>
		/// <param name="sb">String builder object to return with the given expression evaluated</param>
		/// <param name="expression">The entire expression string</param>
		/// <param name="operatorIndex">The index of the operatorSymbol within the given expression that you wish to evaluate</param>
		/// <returns></returns>
		public static StringBuilder EvaluateExpressionInstanceByOperator(this StringBuilder sb, string expression, int operatorIndex)
		{
			var operatorSymbol = expression[operatorIndex];

			// extract the right operand
			var rightOperandStr = new StringBuilder();
			var index = operatorIndex;
			do
			{
				index++;
				if (index >= expression.Length)
					break;
				if (rightOperandStr.Length == 0 && expression[index] == '-') // negative number check
				{
					rightOperandStr.Append(expression[index]);
					continue;
				}
				if (!expression[index].IsNumber())
					break;
				rightOperandStr.Append(expression[index]);

			} while (index < expression.Length);

			var leftOperandStr = new StringBuilder();

			// extract the left operand
			index = operatorIndex;
			do
			{
				index--;
				if (index < 0)
					break;
				if (leftOperandStr.Length > 0 && expression[index] == '-') // negative number check
				{
					leftOperandStr.Append(expression[index]);
					break;
				}
				if (!expression[index].IsNumber())
					break;
				leftOperandStr.Append(expression[index]);

			} while (index >= 0);
			var leftOperandVal = leftOperandStr.ToString().ReverseString();

			if (!leftOperandVal.IsValidInt()
				|| !rightOperandStr.IsValidInt()
				|| (operatorSymbol == '/' && rightOperandStr.ToString() == "0")) 
			{
				sb.Clear();
				sb.Append("Invalid Expression. Are you trying to divide by zero?");
				return sb;
			}

			// evaluate the expression, then go back and replace the instance of the operation string with the evaluated value, e.g. 5 + 4 would be replaced with 9
			var leftOperand = Convert.ToInt64(leftOperandVal);
			var rightOperand = Convert.ToInt64(rightOperandStr.ToString());
			var singleExpressionEvaluated = EvaluateSingleOperation(leftOperand, rightOperand, operatorSymbol);
			var originalSingleExpression = leftOperand + "" + operatorSymbol + "" + rightOperand;

			// E.g. Given expression 2*3+5+4+2*3 with an operator index of 1, the output would be 6+5+4+2*3
			sb.Replace(originalSingleExpression, singleExpressionEvaluated.ToString(), operatorIndex - leftOperandStr.Length, originalSingleExpression.Length);

			return sb;
		}

		public static Int64? EvaluateSingleOperation(Int64 leftOperand, Int64 rightOperand, char theOperator)
		{
			Int64? result;
			switch (theOperator)
			{
				case '+':
					result = leftOperand + rightOperand;
					break;
				case '-':
					result = leftOperand - rightOperand;
					break;
				case '*':
					result = leftOperand * rightOperand;
					break;
				case '/':
					result = leftOperand / rightOperand;
					break;
				default:
					result = null;
					break;
			}
			return result;
		}

		public static bool IsNumber(this char c)
		{
			return c >= '0' && c <= '9';
		}

		public static bool IsValidInt(this StringBuilder numString)
		{
			Int64 num;
			return Int64.TryParse(numString.ToString(), out num);
		}

		public static bool IsValidInt(this string numString)
		{
			Int64 num;
			return Int64.TryParse(numString, out num);
		}

		public static bool ContainsOnlyNumbers(this string str)
		{
			return str.All(c => c.IsNumber());
		}

		public static string RemoveAllSpaces(this string input)
		{
			if (input == null) 
                return null;
            var stringNoSpaces = Regex.Replace(input, @"[\s]+", "");
			return stringNoSpaces;
		}

        // e.g. (3)2+5 -> (3)*2+5
        // 2+5(3) -> 2+5*(3)
        // (2+5)(3) -> (2+5)*(3)
        public static string ReplaceMultiplierParens(this string input)
        {
            if (input == null)
                return null;
            var sb = new StringBuilder(input);
            sb = sb.Replace(")(", ")*(");
            var leftParenRegEx = new Regex(@"[0-9]\(");
            var rightParenRegex = new Regex(@"\)[0-9]");
            foreach (Match match in Regex.Matches(input, @"[0-9]\("))
            {
                var matchVal = match.Groups[0].Value;
                sb = sb.Replace(matchVal, matchVal[0] + "*" + "(");
            }
            foreach (Match match in Regex.Matches(input, @"\)[0-9]"))
            {
                var matchVal = match.Groups[0].Value;
                sb = sb.Replace(matchVal, ")" + "*" + matchVal[1]);
            }
            return sb.ToString();
        }

		public static string ReverseString(this string input)
		{
			return new string(input.ToCharArray().Reverse().ToArray());
		}
	}
}
