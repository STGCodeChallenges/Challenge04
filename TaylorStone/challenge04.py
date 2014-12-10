#!/usr/bin/python3

import ast
import operator as op

# Mathematical operators supported in script
operators = {ast.Add: op.add, ast.Sub: op.sub, ast.Mult: op.mul, ast.Div: op.truediv, ast.Pow: op.pow, ast.BitXor: op.xor, ast.USub: op.neg}

def evaluate_expression(expr):
    return eval_(ast.parse(expr, mode='eval').body)

def eval_(arith):
    if isinstance(arith, ast.Num):
        return arith.n
    elif isinstance(arith, ast.BinOp):
        return operators[type(arith.op)](eval_(arith.left), eval_(arith.right))
    elif isinstance(arith, ast.UnaryOp):
        return operators[type(arith.op)](eval_(arith.operand))
    else:
        raise TypeError(arith)

def main():
    while True:
        try:
            result = evaluate_expression(input("Please enter a valid mathematical expression: "))
            break
        except (TypeError, KeyError):
            print("Sorry, I didn't understand that. Please make sure you formatted the expression correctly and try again.")
            continue
    print("Answer: %s" % result)
    again = input("Do you want to enter another?")
    if again in ("yes", "y"):
        main()
    else:
        print("Mischief managed!")

if __name__ == "__main__":
    main()
