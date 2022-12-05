with open('../input.txt', 'r') as f:
    sum = 0
    lines = f.readlines()
    for three in range(0, len(lines), 3):
        (first, second, third) = lines[three:three + 3]
        common1 = set(first.strip()).intersection(second.strip())
        common2 = common1.intersection(third.strip())
        position = 0
        if list(common2)[0] == list(common2)[0].lower():
            position = ord(list(common2)[0]) - 96
        else:
            position = ord(list(common2)[0]) - 38
        sum += position
        
        
print(sum)
        