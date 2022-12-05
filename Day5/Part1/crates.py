import re

with open('../input.txt', 'r') as f:
    # get each line of the input file
    lines = f.read()
    
    # split the crates and instructions from input file
    crates, instructions = lines.split('\n\n')
    crates = crates.split('\n')
    
    # get rid of whitespace and unecessary characters from crates ( [, ], and spaces)
    # replace tabs (areas in columns with no crates with @ symbol)
    rows = [line.replace('[', '').replace(']', '').replace('    ', '@').replace(' ', '') for line in crates[:-1]]
    
    # get rid of white space in final row showing the numbers of stacks and get total number of stacks
    final_row = crates[-1].replace(' ', '')
    num_stacks = int(final_row[-1])
    
    # create list of empty lists that will hold the stacks
    stacks = [[] for _ in range(num_stacks)]
    
    # go through each row and add the elements to the appropriate columns
    for row in rows:
        for index in range(len(row)):
            # do not add to stack if it was empty
            if row[index] != '@':
                stacks[index].append(row[index])
    
    # reverse the stacks (so we can pop off the top of them)
    for stack in stacks:
        stack = stack.reverse()
    
    # split up the instructions
    instructions = instructions.split('\n')
    
    # simplify the list of instructions so that it contains lists of the numbers in each instruction
    simplified = []
    for instruction in instructions:
        numeric_instructions = [int(num) for num in re.findall(r'\d+', instruction)]
        simplified.append(numeric_instructions)
    
    # for each set of instructions, find how many crates must be moved, take them off the appropriate stack and append to the correct stack
    for numbered_instructions in simplified:
        for _ in range(numbered_instructions[0]):
            moving = stacks[numbered_instructions[1] - 1].pop()
            stacks[numbered_instructions[2] - 1].append(moving)
    
    # add top of each stack to string and print
    final_message = ""
    for stack in stacks:
        final_message += stack[-1]
    print(final_message)
            