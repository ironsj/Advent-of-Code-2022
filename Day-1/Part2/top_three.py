most_calories = [0, 0, 0]

with open('../input.txt', 'r') as f:
    sum = 0
    for line in f:
        if line == '\n':
            for elf in range(len(most_calories)):
                if sum > most_calories[elf]:
                    most_calories[elf] = sum
                    sum = 0
                    most_calories.sort()
                    continue
            sum = 0
            continue
        num = int(line)
        sum += num
        
top_three = 0
for val in most_calories:
    top_three += val
print(most_calories)
print(top_three)
        