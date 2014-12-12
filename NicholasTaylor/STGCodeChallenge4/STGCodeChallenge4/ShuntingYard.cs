using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STGCodeChallenge4
{
    public class ShuntingYard
    {
        private string expression = "";
        Dictionary<string, int> precedence = new Dictionary<string, int>();

        public ShuntingYard(string exp)
        {
            expression = Regex.Replace(exp, @"(\d)(-)(\d)", "$1 $2 $3");
            setPrecedence();
        }

        private void setPrecedence()
        {
            precedence.Add("+", 0);
            precedence.Add("-", 0);
            precedence.Add("*", 1);
            precedence.Add("/", 1);
        }

        private int getPrecedence(string op)
        {
            return precedence[op];
        }

        public Decimal processExpression()
        {
            Stack<string> output = new Stack<string>();
            Stack<string> operatorStack = new Stack<string>();
            string pattern = @"([-]*\d+|[()*+/-])";
            List<string> splitExpression = Regex.Split(expression, pattern).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            foreach (string piece in splitExpression)
            {
                //If piece is a number, then add it to the output queue
                if (Regex.Match(piece, @"^[-]*\d+$").Success)
                {
                    output.Push(piece);
                }
                else if (Regex.Match(piece, @"^[*+/-]$").Success) //If piece is an operator
                {
                    while (operatorStack.Count() > 0 && operatorStack.First() != "(" && (getPrecedence(piece) <= getPrecedence(operatorStack.First())))
                    {
                        output.Push(operatorStack.Pop());
                    }
                    operatorStack.Push(piece);
                }
                else if (piece == "(") //If piece is a left parenthesis, then push it onto the operator stack
                {
                    operatorStack.Push(piece);
                }
                else if (piece == ")") //If piece is a right parenthesis
                {
                    while (operatorStack.First() != "(" && operatorStack.Count() > 0) //Until item in the top of the stack is a left parenthesis, pop operators off the stack onto the output queue
                    {
                        output.Push(operatorStack.Pop());
                    }
                    if (operatorStack.First() == "(") //Pop the left parenthesis from the stack, but not onto the output queue
                    {
                        operatorStack.Pop();
                    }
                    else //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                    {
                        throw new Exception("There is a mismatched parenthesis.  Cannot evaluate expression.");
                    }
                }
            }
            while (operatorStack.Count() > 0)
            {
                output.Push(operatorStack.First());
                operatorStack.Pop();
            }

            ExpressionTree root = new ExpressionTree();
            buildTreeFromStack(output, root);
            Decimal finalValue = evaluateProcessedExpression(root);
            return finalValue;
        }

        private ExpressionTree buildTreeFromStack(Stack<string> output, ExpressionTree root)
        {
            while (output.Count() > 0)
            {
                if (Regex.Match(output.First(), @"^[*+/-]$").Success)
                {
                    if (string.IsNullOrEmpty(root.op))
                    {
                        root.op = output.Pop();
                    }
                    else
                    {
                        root.right = buildTreeFromStack(output, new ExpressionTree());
                    }
                }
                else if (Regex.Match(output.First(), @"^[-]*\d+$").Success)
                {
                    if (root.left == null)
                    {
                        root.left = new ExpressionTree(output.Pop());
                    }
                    else if (root.right == null)
                    {
                        root.right = new ExpressionTree(output.Pop());
                        return root;
                    }
                    else
                    {
                        return root;
                    }
                }
            }
            return root;
        }

        private Decimal evaluateProcessedExpression(ExpressionTree root)
        {
            if (root.right != null && root.right.value == null)
            {
                evaluateProcessedExpression(root.right);
            }
            if (root.right != null && root.right.value != null && root.left != null && root.left.value != null)
            {
                switch (root.op)
                {
                    case "+":
                        root.value = "" + (Decimal.Parse(root.right.value) + Decimal.Parse(root.left.value));
                        break;
                    case "-":
                        root.value = "" + (Decimal.Parse(root.right.value) - Decimal.Parse(root.left.value));
                        break;
                    case "*":
                        root.value = "" + (Decimal.Parse(root.right.value) * Decimal.Parse(root.left.value));
                        break;
                    case "/":
                        root.value = "" + (Decimal.Parse(root.right.value) / Decimal.Parse(root.left.value));
                        break;
                    default:
                        break;
                }
            }
            return Decimal.Parse(root.value);
        }
    }

    public class ExpressionTree
    {
        public ExpressionTree left { get; set; }
        public ExpressionTree right { get; set; }
        public string value { get; set; }
        public string op { get; set; }

        public ExpressionTree()
        {
        }

        public ExpressionTree(string val)
        {
            value = val;
        }
    }
}
