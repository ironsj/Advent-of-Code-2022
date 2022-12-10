import { readFileSync, promises as fsPromises } from 'fs';
import { maxHeaderSize } from 'http';
import { join } from 'path';

function syncReadFile(filename: string) {
    const result = readFileSync(join(__dirname, filename), 'utf-8');
    return result;
}

function create2dArray(input: string) {
    const split = input.split('\n');
    const trees: number[][] = [];
    for (const line of split) {
        const charArray = line.split('');
        let treeArray = [];
        for (const tree of charArray) {
            treeArray.push(parseInt(tree));
        }
        trees.push(treeArray);
    }

    return trees;
}

function findScenic(trees: number[][]) {
    // 2d array to store the scores for each tree
    let scores = Array(trees.length).fill(0).map(() => Array(trees[0].length).fill(0));

    // function to calculate the score for each tree
    const score = (arr: number[], index: number) => {
        let count = 0;
        // calculate the score to the right/down of the tree
        if (index === 0) {
            for (let i = index + 1; i < arr.length; i++) {
                if (arr[index] > arr[i]) {
                    count++;
                }
                else {
                    count++;
                    break;
                }
            }
        }
        // calculate the score to the left/up of the tree
        else {
            for (let i = index - 1; i >= 0; i--) {
                if (arr[index] > arr[i]) {
                    count++;
                }
                else {
                    count++;
                    break;
                }
            }
        }
        return count;
    }

    // loop through each tree
    for (let i = 1; i < trees.length - 1; i++) {
        for (let j = 1; j < trees[i].length - 1; j++) {
            let right = trees[i].slice(j, trees[i].length);
            let left = trees[i].slice(0, j + 1);
            let up = trees.slice(0, i + 1).map((row) => row[j]);
            let down = trees.slice(i, trees.length).map((row) => row[j]);
            // calculate the score for each tree
            scores[i][j] = score(right, 0) * score(left, left.length - 1) * score(up, up.length - 1) * score(down, 0);
        }
    }

    // return the max score from the 2d array of scores
    return Math.max.apply(null, scores.map((row) => Math.max.apply(null, row)));
}

const input: string = syncReadFile('../input.txt');
const trees: number[][] = create2dArray(input);
let arr = create2dArray(input);
let scenicScore = findScenic(arr);
console.log(scenicScore);
