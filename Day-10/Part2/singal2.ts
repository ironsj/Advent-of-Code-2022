import { readFileSync, promises as fsPromises } from 'fs';
import { join } from 'path';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function drawPixels(input: Array<string>) {
    let currentCycle: number = 1;
    let registerValue: number = 1;
    let drawing = '';

    // draws a pixel, '.' if the sprite is not visible, '#' if it is
    const draw = () => {
        const currentCrtColumn = (currentCycle - 1) % 40;
        // register value (where the sprite is located) is within 1 left or right of the current column being drawn
        drawing += Math.abs(currentCrtColumn - registerValue) <= 1 ? '#' : '.';
    }

    // draw first pixel
    draw();
    for (const line of input) {
        if (line.startsWith('addx')) {
            const split = line.split(' ');
            const value = parseInt(split[1]);
            currentCycle++;
            draw();
            registerValue += value;
        }
        currentCycle++;
        draw();
    }

    // split the drawing into 6 lines
    let drawingFinal = [];
    for (let i = 0; i < 6; i++) {
        drawingFinal.push(drawing.slice(i * 40, (i + 1) * 40));
    }
    return drawingFinal;
}
const input: string = syncReadFile('../input.txt');
const split = input.split('\n');
const result = drawPixels(split);
console.log(result);
