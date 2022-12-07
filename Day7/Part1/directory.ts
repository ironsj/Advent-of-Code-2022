import { readFileSync, promises as fsPromises } from 'fs';
import { join } from 'path';
import { stringify } from 'querystring';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function makeTree(input: Array<string>) {
    let directoryDepth: Array<string> = [];
    let directorySizeMap = new Map<string, number>();
    let sum = 0;
    for (const line of input) {
        if (line.startsWith('$ cd')) {
            const newDirectory = line.replace('$ cd ', '');
            if (newDirectory === '..') {
                directoryDepth.pop();
            }
            else {
                directoryDepth.push(newDirectory);
            }
        }
        else if (!isNaN(parseInt(line[0]))) {
            const fileSize: Array<string> = line.split(' ');
            let key: string = '';
            for (const directory of directoryDepth) {
                key += directory;
                let size = parseInt(fileSize[0]);
                if (directorySizeMap.has(key)) {
                    size += directorySizeMap.get(key)!
                }
                directorySizeMap.set(key, size);
            }
        }
    }

    directorySizeMap.forEach((value: number) => {
        if (value <= 100000) {
            sum += value;
        }
    })

    return sum;
}

const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const result = makeTree(split);
console.log(result);

