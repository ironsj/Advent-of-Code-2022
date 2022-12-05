win = {'A':'Y', 'B':'Z', 'C':'X'}
draw = {'A':'X', 'B':'Y', 'C':'Z'}
scores = {'X':1,'Y':2,'Z':3}

with open('input.txt', 'r') as f:
    sum = 0
    
    for line in f:
        sum += scores[line[2]]
        if line[2] == win[line[0]]:
            sum += 6
        elif line[2] == draw[line[0]]:
            sum += 3
    
print(sum)