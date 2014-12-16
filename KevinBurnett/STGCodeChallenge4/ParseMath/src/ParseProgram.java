/**
 * Created with IntelliJ IDEA.
 * User: Kevin Burnett
 * Date: 12/9/14
 * Time: 10:09 AM
 * To change this template use File | Settings | File Templates.
 *
 * Code Challenge #4
 * Evaluate a mathematical expression. For example, if str were "2+(3-1)*3" the output should be 8.
 * Another example: if str were "(2-0)(6/2)" the output should be 6. There can be parenthesis within
 * the string so you must evaluate it properly according to the rules of arithmetic. The string will
 * contain the operators: +, -, /, *, (, and ). If you have a string like this: #/#*# or #+#(#)/#,
 * then evaluate from left to right. So divide then multiply, and for the second one multiply, divide,
 * then add. The evaluations will be such that there will not be any decimal operations, so you do
 * not need to account for rounding and whatnot.
 */

import java.util.*;

public class ParseProgram
{
    //Associativity constraints for operators
    private static final int LEFT_ASSOC = 0;
    private static final int RIGHT_ASSOC = 1;

    //Operators
    private static final Map<String, int[]> OPERATORS = new HashMap<String, int[]>();
    static
    {
        //Map<"token", []{precedence, associativity}>
        OPERATORS.put("+", new int[] {0, LEFT_ASSOC});
        OPERATORS.put("-", new int[] {0, LEFT_ASSOC});
        OPERATORS.put("*", new int[] {5, LEFT_ASSOC});
        OPERATORS.put("/", new int[] {5, LEFT_ASSOC});
    }

    //Test if token is an operator
    private static boolean isOperator(String token)
    {
        return OPERATORS.containsKey(token);
    }

    //Test associativity of operator
    private static boolean isAssociative(String token, int type)
    {
        if(!isOperator(token))
        {
            throw new IllegalArgumentException("Invalid token: " + token);
        }

        if (OPERATORS.get(token)[1] == type)
        {
            return true;
        }
        return false;
    }

    //Compare precedence of operators
    private static final int cmpPrecedence(String token1, String token2)
    {
        if(!isOperator(token1) || !isOperator(token2))
        {
            throw new IllegalArgumentException("Invalid tokens: " + token1 + " " + token2);
        }
        return OPERATORS.get(token1)[0] - OPERATORS.get(token2)[0];
    }

    //Convert into RPN
    public static String[] infixToRPN(String[] inputTokens)
    {
        ArrayList<String> out = new ArrayList<String>();
        Stack<String> stack = new Stack<String>();

        //For each token
        for(String token : inputTokens)
        {
            //If token is an operator
            if(isOperator(token))
            {
                //While stack not empty AND stack top element is an operator
                while (!stack.empty()&& isOperator(stack.peek()))
                {
                    if((isAssociative(token, LEFT_ASSOC) &&
                       cmpPrecedence(token, stack.peek()) <= 0) ||
                       (isAssociative(token, RIGHT_ASSOC) &&
                       cmpPrecedence(token, stack.peek()) < 0))
                    {
                        out.add(stack.pop());
                        continue;
                    }
                    break;
                }
                //push the new operator on the stack
                stack.push(token);
            }

            //If token is a left bracket '('
            else if(token.equals("("))
            {
                stack.push(token);
            }
            //If token is a right bracket ')'
            else if(token.equals(")"))
            {
                while(!stack.empty() && !stack.peek().equals("("))
                {
                    out.add(stack.pop());
                }
                stack.pop();

            }
            //If token is a number
            else
            {
                out.add(token);
            }
        }
        while (!stack.empty())
        {
            out.add(stack.pop());
        }
        String[] output = new String[out.size()];
        return out.toArray(output);
    }

    public static double RPNtoDouble(String[] tokens)
    {
        Stack<String> stack = new Stack<String>();

        //For each token
        for (String token : tokens)
        {
            //If the token is a value push it onto the stack
            if(!isOperator(token))
            {
                stack.push(token);
            }
            else
            {
                //Token is an operator.  Pop top two entries
                Double d2 = Double.valueOf(stack.pop());
                Double d1 = Double.valueOf(stack.pop());

                //Get the numerical result
                Double result = token.compareTo("+") == 0 ? d1 + d2 :
                                token.compareTo("-") == 0 ? d1 - d2 :
                                token.compareTo("*") == 0 ? d1 * d2 :
                                                            d1 / d2;

                //Push result onto stack
                stack.push( String.valueOf(result));
            }
        }
        return Double.valueOf(stack.pop());
    }

    public static void main(String[] args)
    {
        //
        //Test Strings
        //
        //String inputString = "(2-0)(6/2)"; //Should be 6
        //String inputString = "( 1 + 2 ) * ( 3 / 4 ) - ( 5 + 6 )";  //Should = -8.75
        String inputString = "2 + ( 3 - 1 ) * 3";  //Should = 8
        //String inputString = "( 3 - 1 )";  //Should = 2

        //Get all spaces out of string
        inputString = inputString.replaceAll(" ", "");

        //Check for ")(" case and insert a "*".
        int offset = inputString.indexOf(")(");

        if (offset > -1)
        {
            inputString = new StringBuilder(inputString).insert(offset +1, "*").toString();
        }

        //Place string into array
        String[] input = new String[inputString.length()];
        input = inputString.split("(?!^)");

        //Convert string to RPN
        String[] output = infixToRPN(input);

        //Send the RPN string to RPNDouble and display result
        Double result = RPNtoDouble(output);
        System.out.print("Result = " + result);
    }
}