import { readFileSync, promises as fsPromises } from 'fs';
import { join } from 'path';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function findFour(input: string) {
    let uniqueSet = new Set();
    let index = -1;
    for (let i = 0; i < input.length; i++) {
        for (let j = i; j < i + 14; j++) {
            uniqueSet.add(input[j]);
        }
        if (uniqueSet.size == 14) {
            index = i + 14;
            break;
        }
        uniqueSet = new Set();
    }
    return index;
}

const input: string = syncReadFile('../input.txt');
let index: number = findFour(input);
console.log(index);
