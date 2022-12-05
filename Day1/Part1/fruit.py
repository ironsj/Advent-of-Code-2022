most_calories = 0

with open('../input.txt', 'r') as f:
    sum = 0
    for line in f:
        if line == '\n':
            if sum > most_calories:
                most_calories = sum
                sum = 0
                continue
            else:
                sum = 0
                continue
        num = int(line)
        sum += num
        
print(most_calories)
        