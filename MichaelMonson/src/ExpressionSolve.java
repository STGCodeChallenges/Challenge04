import java.util.Scanner;


public class ExpressionSolve {
	

	// Create a single shared Scanner for keyboard input
    private static Scanner scanner = new Scanner( System.in );
    
    

	public static void main(String[] args) {

		displayInstructionsMarquee();
		
		System.out.print( "\n\n  Enter a mathmatical expression:" );
		
        // Read the text entered by the user:
        String mathProblem = scanner.nextLine();
    	int solution = 0;
        
        try {        	

        	//Clear all whitespace:
            mathProblem = mitigateWhiteSpace(mathProblem);
            System.out.println("\n\nOriginal Problem: \n" + mathProblem);

                    	
        	//Evaluate using Arithmetic Rules:
        	int leftNumber = 0;
        	int rightNumber = 0;

        	
        	/* Check for parenthesis:
        	 *   TODO:  Expand support from a single set of paren's to evaluate multiple and nested parenthesis sets:
        	 */        		
        	int leftParen = mathProblem.indexOf("(");
        	int rightParen = mathProblem.indexOf(")");
        	
        	if (leftParen != -1) {
        		String subFormula = mathProblem.substring(leftParen + 1, rightParen);        		
        		String answer = evaluateStatement(subFormula);
        		mathProblem = mathProblem.substring(0, leftParen)
    					+ String.valueOf(answer)
    					+ mathProblem.substring(rightParen + 1, mathProblem.length());
        	}
        	
        	//Parenthesis cleared.  Now evaluate the rest of the problem:
        	String finalAnswer = evaluateStatement(mathProblem);
    		
    		System.out.println("\n\n FINAL ANSWER: " + finalAnswer);


        } catch (Exception e) {
        	System.out.println( "\n  An error has occured... invalid characters may have been entered" );
        }
        
	}
	
	
	private static String evaluateStatement (String statement) {
		
    	int leftNumber = 0;
    	int rightNumber = 0;
    	int solution = 0;
		
    	//Multiplication and Division:
    	while (solution == 0) {
        	int multiplyIndex = statement.indexOf("*");
        	int divideIndex = statement.indexOf("/");
        	int operatorIndex = 0;
        	
    		if (multiplyIndex > 0 || divideIndex > 0) {
    			
    			int calculate = 0;
    			
    			if (multiplyIndex > 0 && (divideIndex <= 0 || divideIndex > multiplyIndex)) {
    				leftNumber = constructLeftNumber(statement, multiplyIndex);
    				rightNumber = constructRightNumber(statement, multiplyIndex);
    				operatorIndex = multiplyIndex;
    				calculate = leftNumber * rightNumber;
    				
    			} else if (divideIndex > 0 && (multiplyIndex <= 0 || multiplyIndex > divideIndex)) {
    				leftNumber = constructLeftNumber(statement, divideIndex);
    				rightNumber = constructRightNumber(statement, divideIndex);
    				operatorIndex = divideIndex;
    				calculate = leftNumber / rightNumber;
    			}
    			
    			//Display Calculation and merge back:
    			System.out.println("   -->: " + leftNumber + statement.charAt(operatorIndex) + rightNumber + " = " + calculate);
    			
    			statement = statement.substring(0, operatorIndex - String.valueOf(leftNumber).length())
    					+ String.valueOf(calculate)
    					+ statement.substring(operatorIndex + String.valueOf(rightNumber).length() + 1, statement.length());
    			
    			System.out.println("\n" + statement);
    			
    		} else {
    			break;
    		}
    	}
    	
    	
    	//Addition and Subtraction:
    	while (solution == 0) {
        	int addIndex = statement.indexOf("+");
        	int subtractIndex = statement.indexOf("-");
        	int operatorIndex = 0;
        	
    		if (addIndex > 0 || subtractIndex > 0) {
    			
    			int calculate = 0;
    			
    			if (addIndex > 0 && (subtractIndex <= 0 || subtractIndex > addIndex)) {
    				leftNumber = constructLeftNumber(statement, addIndex);
    				rightNumber = constructRightNumber(statement, addIndex);
    				operatorIndex = addIndex;
    				calculate = leftNumber + rightNumber;
    				
    			} else if (subtractIndex > 0 && (addIndex <= 0 || addIndex > subtractIndex)) {
    				leftNumber = constructLeftNumber(statement, subtractIndex);
    				rightNumber = constructRightNumber(statement, subtractIndex);
    				operatorIndex = subtractIndex;
    				calculate = leftNumber - rightNumber;
    			}
    			
    			//Display Calculation and merge back:
    			System.out.println("   -->: " + leftNumber + statement.charAt(operatorIndex) + rightNumber + " = " + calculate);
    			
    			statement = statement.substring(0, operatorIndex - String.valueOf(leftNumber).length())
    					+ String.valueOf(calculate)
    					+ statement.substring(operatorIndex + String.valueOf(rightNumber).length() + 1, statement.length());
    			
    			System.out.println("\n" + statement);
    			
    		} else {
    			break;
    		}
    		
    	}
		
    	return statement;
		
	}
	
	
	private static int constructLeftNumber (String currentMathProblem, int index) {
		
		String leftNumber = "";
		
		while (index > 0) {
			String peekCharacter = String.valueOf(currentMathProblem.charAt(index-1));
			if (peekCharacter.matches("[0-9]")) {
				leftNumber = peekCharacter.concat(leftNumber);
			} else {
				break;
			};
			
			index--;			
		}
		
		return Integer.valueOf(leftNumber);
	}

	private static int constructRightNumber (String currentMathProblem, int index) {
		
		String rightNumber = "";
		
		while (index < currentMathProblem.length() - 1) {
			String peekCharacter = String.valueOf(currentMathProblem.charAt(index + 1));
			if (peekCharacter.matches("[0-9]")) {
				rightNumber = rightNumber.concat(peekCharacter);
			} else {
				break;
			};
			
			index++;			
		}
		
		return Integer.valueOf(rightNumber);
	}
	
	private static void displayInstructionsMarquee() {
		String headerFill = "\n\t ~                                               ~";
		String headerBorder = "\n\t ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~";
		System.out.print( headerBorder + headerFill + "\n\t ~   STG ~ Code Challenge #4 ~ Expression Solver ~" + headerFill + headerBorder + "\n\n" );
		System.out.print( "Evaluate a mathmatical expression, using the rules of arithmetic, allowing for parenthesis. \n" );
		System.out.print( "It is understood that the math problem entered will not result in decimal operations. \n" );
		System.out.print( "Standard Operators: +, -, /, *, (). " );
	}
	
	private static String mitigateWhiteSpace(String input) {
		return input.replaceAll("\\s+","");
	}

}
