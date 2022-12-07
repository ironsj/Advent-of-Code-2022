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

    return directorySizeMap;
}

function getSpace(tree: Map<string, number>) {
    const UPDATE_SPACE = 30000000;
    const SPACE_AVAILABLE = 70000000 - tree.get('/')!;
    const SPACE_NEEDED = UPDATE_SPACE - SPACE_AVAILABLE;
    let possibleDeletions: Array<number> = []

    tree.forEach((value: number) => {
        if (value >= SPACE_NEEDED) {
            possibleDeletions.push(value);
        }
    })

    possibleDeletions.sort((a, b) => a - b);

    return possibleDeletions[0];
}

const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const tree = makeTree(split);
const gone = getSpace(tree);
console.log(gone);

