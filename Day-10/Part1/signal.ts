import { readFileSync, promises as fsPromises } from 'fs';
import { join } from 'path';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function signalStrength(input: Array<string>) {
    let signalStrength = [];

    let currentCycle: number = 1;
    let registerValue: number = 1;
    signalStrength.push(currentCycle * registerValue);

    for (const line of input) {
        if (line.startsWith('addx')) {
            const split = line.split(' ');
            const value = parseInt(split[1]);
            currentCycle++;
            signalStrength.push(currentCycle * registerValue);
            registerValue += value;
        }
        currentCycle++;
        signalStrength.push(currentCycle * registerValue);

    }

    return signalStrength[19] + signalStrength[59] + signalStrength[99] + signalStrength[139] + signalStrength[179] + signalStrength[219];
}

const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const result = signalStrength(split);
console.log(result);
