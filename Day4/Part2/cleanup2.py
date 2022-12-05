with open('../input.txt', 'r') as f:
    count = 0
    range1= []
    range2 = []
    for line in f:
        for num in line.split(','):
            minMax = list(map(int, num.split('-')))
            temp = []
            for i in range(minMax[0], minMax[1] + 1):
                temp.append(i)
            if len(range1) == 0:
                range1 = temp
            else:
                range2 = temp
        intersect1 = set(range1).intersection(set(range2))
        intersect2 = set(range2).intersection(set(range1))
        if len(intersect1) != 0 or len(intersect2) != 0:
            count += 1
            range1 = []
            range2 = []
        else:
            range1 = []
            range2 = []
        
        
print(count)
        