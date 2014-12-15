import re

def calc(first, second, part):
    if part == "*":
        return str(int(first)*int(second))
    elif part == "/":
        return str(int(first)/int(second))
    elif part == "+":
        return str(int(first)+int(second))
    elif part == "-":
        return str(int(first)-int(second))    

def rank(op, flag):
    if op in "+-":
        return 1
    elif op in "*/":
        return 2
    elif op == "(" and flag:
        return 3
    else:
        return 0
        

def calculator(express):
    nums = []
    opers = []
    print express + "=",
    express = express.replace(")(", ")*(")
    for part in re.findall("(\d+\()", express):
        index = part.find("(")
        newPart = part[:index] + "*" + part[index:]
        express = express.replace(part, newPart)
    parts = re.findall("[?()+-/*]|[\d+]", express)
    for part in parts:
        if part.isdigit():
            nums.append(part)
        else:
            if len(opers) > 0:
                if rank(part, True) < rank(opers[-1], False):
                    if part == ")":
                        part = opers.pop()
                        while part != "(":
                            second = nums.pop()
                            first = nums.pop()
                            nums.append(calc(first,second,part))
                            part = opers.pop()
                    else:
                        second = nums.pop()
                        first = nums.pop()
                        nums.append(calc(first,second,part))
                else:
                    opers.append(part)
            else:
                opers.append(part)
    while len(opers) > 0:
        op = opers.pop()
        second = nums.pop()
        first = nums.pop()
        nums.append(calc(first,second,op))       
    print nums[0]

if __name__ == "__main__":
    calculator("2(7-1)/3")
    calculator("2+(3-1)*3")
    calculator("(2-0)(6/2)")