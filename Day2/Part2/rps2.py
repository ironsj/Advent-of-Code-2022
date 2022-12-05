win = {'A':'Y', 'B':'Z', 'C':'X'}
draw = {'A':'X', 'B':'Y', 'C':'Z'}
lose = {'A':'Z', 'B':'X', 'C':'Y'}
scores = {'X':1,'Y':2,'Z':3}

with open('../input.txt', 'r') as f:
    sum = 0
    
    for line in f:
        my_choice = ''
        if line[2] == 'X':
            my_choice = lose[line[0]]
            sum += scores[my_choice]
        elif line[2] == 'Y':
            sum += 3
            my_choice = draw[line[0]]
            sum += scores[my_choice]
        elif line[2] == 'Z':
            sum += 6
            my_choice = win[line[0]]
            sum += scores[my_choice]
    
print(sum)