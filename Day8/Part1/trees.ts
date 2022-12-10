import { readFileSync, promises as fsPromises } from 'fs';
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

function findVisible(trees: number[][]) {
    // outer edge of the grid is always visible
    let count = 2 * (trees.length - 1 + trees[0].length - 1);

    // find if the tree is visible from the top/bottom/left/right
    const max = ((arr: number[], value: number) => {
        // if the tree is the tallest in the direction
        if (Math.max.apply(null, arr) === value) {
            // get all trees with the same height in the directio
            const uniqueArray = arr.filter((item) => {
                return item === value;
            })
            // if there are no other trees the same height, it is visible
            if (uniqueArray.length === 1) {
                return true;
            }
        }
    });

    // loop through the grid
    for (let i = 1; i < trees.length - 1; i++) {
        for (let j = 1; j < trees[i].length - 1; j++) {
            let right = trees[i].slice(j, trees[i].length);
            let left = trees[i].slice(0, j + 1);
            let up = trees.slice(0, i + 1).map((row) => row[j]);
            let down = trees.slice(i, trees.length).map((row) => row[j]);
            if (max(right, trees[i][j]) || max(left, trees[i][j]) || max(up, trees[i][j]) || max(down, trees[i][j])) {
                count++;
            }
        }
    }

    return count;
}

const input: string = syncReadFile('../input.txt');
const trees: number[][] = create2dArray(input);
let arr = create2dArray(input);
let visibleTrees = findVisible(arr);
console.log(visibleTrees);
