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

    let rope = [
        head,
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        { x: 0, y: 0 },
        tail
    ];

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

            for (let j = 0; j < rope.length - 1; j++) {
                const xDiff = Math.abs(rope[j].x - rope[j + 1].x);
                const yDiff = Math.abs(rope[j].y - rope[j + 1].y);
                const diagonal = xDiff + yDiff;

                const xStep = xDiff >= 2 || diagonal >= 3;
                const yStep = yDiff >= 2 || diagonal >= 3;

                if (xStep) {
                    rope[j + 1].x += rope[j].x > rope[j + 1].x ? 1 : -1;
                }
                if (yStep) {
                    rope[j + 1].y += rope[j].y > rope[j + 1].y ? 1 : -1;
                }

                visited.add(`${tail.x}, ${tail.y}`);
            }
        }
    }
    return visited.size;
}

const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const result = follow(split);
console.log(result);
