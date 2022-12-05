with open('../input.txt', 'r') as f:
    sum = 0
    for line in f:
        length = len(line) >> 1
        first_compartment = line[length:]
        second_compartment = line[:length]
        common = set(first_compartment).intersection(second_compartment)
        position = 0
        if list(common)[0] == list(common)[0].lower():
            position = ord(list(common)[0]) - 96
        else:
            position = ord(list(common)[0]) - 38
        sum += position
        
print(sum)
        