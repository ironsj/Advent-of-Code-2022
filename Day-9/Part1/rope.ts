import { readFileSync, promises as fsPromises } from 'fs';
import { join } from 'path';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function follow(input: Array<string>) {
    let head = {
        x: 0,
        y: 0,
    };

    let tail = {
        x: 0,
        y: 0,
    };

    let visited = new Set<string>();
    visited.add(`${tail.x}, ${tail.y}`);

    for (const line of input) {
        const split = line.split(' ');
        const direction = split[0];
        const distance = parseInt(split[1]);

        for (let i = 0; i < distance; i++) {
            if (direction === 'R') {
                head.x++;
            } else if (direction === 'L') {
                head.x--;
            } else if (direction === 'U') {
                head.y++;
            } else if (direction === 'D') {
                head.y--;
            }

            const xDiff = Math.abs(head.x - tail.x);
            const yDiff = Math.abs(head.y - tail.y);
            const diagonal = xDiff + yDiff;

            const xStep = xDiff >= 2 || diagonal >= 3;
            const yStep = yDiff >= 2 || diagonal >= 3;

            if (xStep) {
                tail.x += head.x > tail.x ? 1 : -1;
            }
            if (yStep) {
                tail.y += head.y > tail.y ? 1 : -1;
            }

            visited.add(`${tail.x}, ${tail.y}`);
        }
    }
    return visited.size;
}

const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const result = follow(split);
console.log(result);
