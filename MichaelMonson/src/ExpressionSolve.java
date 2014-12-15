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
        	
        	// Check for parenthesis:
            // This should be the easiest part, since I just take what's inside the left and right paren's
        	
        	//Evaluate using Arithmetic Rules:
        	int leftNumber = 0;
        	int rightNumber = 0;

        	
        	//Multiply and Divide:
        	while (solution == 0) {
	        	int multiplyIndex = mathProblem.indexOf("*");
	        	int divideIndex = mathProblem.indexOf("/");
	        	int operatorIndex = 0;
	        	
	    		if (multiplyIndex > 0 || divideIndex > 0) {
	    			
	    			int calculate = 0;
	    			
	    			if (multiplyIndex > 0 && (divideIndex <= 0 || divideIndex > multiplyIndex)) {
	    				leftNumber = constructLeftNumber(mathProblem, multiplyIndex);
	    				rightNumber = constructRightNumber(mathProblem, multiplyIndex);
	    				operatorIndex = multiplyIndex;
	    				calculate = leftNumber * rightNumber;
	    				
	    			} else if (divideIndex > 0 && (multiplyIndex <= 0 || multiplyIndex > divideIndex)) {
	    				leftNumber = constructLeftNumber(mathProblem, divideIndex);
	    				rightNumber = constructRightNumber(mathProblem, divideIndex);
	    				operatorIndex = divideIndex;
	    				calculate = leftNumber / rightNumber;
	    			}
	    			
	    			//Display Calculation and merge back:
	    			System.out.println("   -->: " + leftNumber + mathProblem.charAt(operatorIndex) + rightNumber + " = " + calculate);
	    			
	    			mathProblem = mathProblem.substring(0, operatorIndex - String.valueOf(leftNumber).length())
	    					+ String.valueOf(calculate)
	    					+ mathProblem.substring(operatorIndex + String.valueOf(rightNumber).length() + 1, mathProblem.length());
	    			
	    			System.out.println("\n" + mathProblem);
	    			
	    		} else {
	    			break;
	    		}
        	}
        	
        	
        	//Addition and Subtraction:
        	while (solution == 0) {
	        	int addIndex = mathProblem.indexOf("+");
	        	int subtractIndex = mathProblem.indexOf("-");
	        	int operatorIndex = 0;
	        	
	    		if (addIndex > 0 || subtractIndex > 0) {
	    			
	    			int calculate = 0;
	    			
	    			if (addIndex > 0 && (subtractIndex <= 0 || subtractIndex > addIndex)) {
	    				leftNumber = constructLeftNumber(mathProblem, addIndex);
	    				rightNumber = constructRightNumber(mathProblem, addIndex);
	    				operatorIndex = addIndex;
	    				calculate = leftNumber + rightNumber;
	    				
	    			} else if (subtractIndex > 0 && (addIndex <= 0 || addIndex > subtractIndex)) {
	    				leftNumber = constructLeftNumber(mathProblem, subtractIndex);
	    				rightNumber = constructRightNumber(mathProblem, subtractIndex);
	    				operatorIndex = subtractIndex;
	    				calculate = leftNumber - rightNumber;
	    			}
	    			
	    			//Display Calculation and merge back:
	    			System.out.println("   -->: " + leftNumber + mathProblem.charAt(operatorIndex) + rightNumber + " = " + calculate);
	    			
	    			mathProblem = mathProblem.substring(0, operatorIndex - String.valueOf(leftNumber).length())
	    					+ String.valueOf(calculate)
	    					+ mathProblem.substring(operatorIndex + String.valueOf(rightNumber).length() + 1, mathProblem.length());
	    			
	    			System.out.println("\n" + mathProblem);
	    			
	    		} else {
	    			break;
	    		}
	    		
        	}
        		
    		
    		System.out.println("\n\n FINAL ANSWER: " + mathProblem);


        } catch (Exception e) {
        	System.out.println( "\n  An error has occured..." );
        }
		
        
		
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
