// either rotate or move forward
enum Instruction {
    Rotate(Turn),
    Forward(u8),
}

// either left or right
enum Turn {
    L,
    R,
}

// a tile can be open, a wall, or a blank space
#[derive(PartialEq)]
enum Tile {
    Open,
    Solid,
    None,
}

// coordinate for our grid
#[derive(Clone)]
struct Coord {
    row: i32,
    col: i32,
}

// the directions we can move be facing
enum Direction {
    L,
    R,
    U,
    D,
}

impl Direction {
    // get the score of this direction
    fn score(&self) -> usize {
        use Direction::*;
        match self {
            R => 0,
            D => 1,
            L => 2,
            U => 3,
        }
    }

    // applies a left or right turn to this direction
    fn turn(self, turn: &Turn) -> Direction {
        use Direction::*;
        match (self, turn) {
            (L, Turn::L) => D,
            (L, Turn::R) => U,
            (R, Turn::L) => U,
            (R, Turn::R) => D,
            (U, Turn::L) => L,
            (U, Turn::R) => R,
            (D, Turn::L) => R,
            (D, Turn::R) => L,
        }
    }

    // what happens when we move in this direction
    fn offset(&self) -> Coord {
        use Direction::*;
        match &self {
            L => Coord { row: 0, col: -1 },
            R => Coord { row: 0, col: 1 },
            U => Coord { row: -1, col: 0 },
            D => Coord { row: 1, col: 0 },
        }
    }
}

fn parse(input: &str) -> (Vec<Vec<Tile>>, Vec<Instruction>) {
    // split between our grid and our instructions
    let (grid, moves) = input.trim_end().split_once("\n\n").unwrap();
    let mut instructions = Vec::new();
    let mut digits = Vec::new();

    // go through each character in our instructions
    for c in moves.chars() {
        // if it's a digit, add it to our digits vector
        if c.is_numeric() {
            // convert char to decimal number
            let digit = c.to_digit(10).unwrap() as u8;
            digits.push(digit);
        } else {
            // this is not a digit (its a rotation instruction), so we need to construct a number out of any digits we've seen
            // turns the digits seen into one single number
            // e.g. 1, 2, 3 -> 1 * 10 + 2 = 12 -> 12 * 10 + 3 = 123
            let num = digits.iter().fold(0, |num, digit| num * 10 + digit);
            // clear the digits vector for the next number
            digits.clear();
            // add the number to our instructions
            instructions.push(Instruction::Forward(num));

            // get direction to rotate
            let turn = match c {
                'L' => Turn::L,
                'R' => Turn::R,
                _ => panic!("Invalid input"),
            };
            // add the rotation to our instructions
            instructions.push(Instruction::Rotate(turn));
        }
    }

    // if our instructions end with a number, we need to add it to our instructions
    // turns the digits seen into one single number
    // e.g. 1, 2, 3 -> 1 * 10 + 2 = 12 -> 12 * 10 + 3 = 123
    let num = digits.iter().fold(0, |num, digit| num * 10 + digit);
    // add the number to our instructions
    instructions.push(Instruction::Forward(num));

    // parse our grid
    let mut map = Vec::new();
    // split the grid into lines
    for line in grid.lines() {
        // create a new row
        let mut row = Vec::new();
        // go through each character in the line
        for c in line.chars() {
            // add the tile to the row
            let tile = match c {
                '.' => Tile::Open,
                '#' => Tile::Solid,
                ' ' => Tile::None,
                _ => panic!("invalid input"),
            };
            row.push(tile);
        }
        // add the row to the map
        map.push(row);
    }

    // return the map and instructions
    (map, instructions)
}

// lets up wrap around the grid if we reach the end of open positions
fn wrap(map: &[Vec<Tile>], pos: &Coord, dir: &Direction) -> Coord {
    // get the offset of the direction
    let Coord {
        row: row_offset,
        col: col_offset,
    } = dir.offset();

    // copies our current position
    let mut curr = pos.clone();

    // walk in the opposite direction until we find a blank space (this will be the same as wrapping around)
    while let Some(tile) = map
        .get((curr.row - row_offset) as usize)
        .and_then(|row| row.get((curr.col - col_offset) as usize))
    {
        // if we find a blank space, we've reached the end of the open positions
        if *tile == Tile::None {
            break;
        }

        // update our position
        curr = Coord {
            row: curr.row - row_offset,
            col: curr.col - col_offset,
        };
    }

    curr
}

fn wrap2(pos: &Coord, dir: &Direction) -> (Coord, Direction) {
    // if we are to divide our input into 50x50 grids we have something as follows (* indicates 50x50 grid):
    // * * *
    // * * *
    // * * *
    // * * *
    // Therefore, all rows are 0..150 and all columns are 0..200
    // Our cubes are located in:
    // * # #
    // * # *
    // # # *
    // # * *
    // Thus we will index the cubes (e.g. farthest top and left cub is 0,1) and hard code what cube and direction they end up in as follows:
    // P.S. this is not a comprehensive list, but it covers all of the cases you would leave the solid/open tiles
    let (cube_row, cube_col, new_dir) = match (pos.row / 50, pos.col / 50, dir) {
        (0, 1, Direction::L) => (2, 0, Direction::R),
        (0, 1, Direction::U) => (3, 0, Direction::R),
        (0, 2, Direction::U) => (3, 0, Direction::U),
        (0, 2, Direction::R) => (2, 1, Direction::L),
        (0, 2, Direction::D) => (1, 1, Direction::L),
        (1, 1, Direction::L) => (2, 0, Direction::D),
        (1, 1, Direction::R) => (0, 2, Direction::U),
        (2, 0, Direction::U) => (1, 1, Direction::R),
        (2, 0, Direction::L) => (0, 1, Direction::R),
        (2, 1, Direction::R) => (0, 2, Direction::L),
        (2, 1, Direction::D) => (3, 0, Direction::L),
        (3, 0, Direction::L) => (0, 1, Direction::D),
        (3, 0, Direction::R) => (2, 1, Direction::U),
        (3, 0, Direction::D) => (0, 2, Direction::D),
        _ => unreachable!(),
    };

    // get our index within the current cube
    let (row_index, column_index) = (pos.row % 50, pos.col % 50);

    // i is used to calculate the new rows and columns
    // you can see the pattern for how i is used in the match statements below
    // it is best to draw out the map and see how the rows and columns change
    // Shoutout to u/0xu- on Reddit for creating this diagram: https://imgur.com/LyJ11iD
    let i = match dir {
        Direction::L => 49 - row_index,
        Direction::R => row_index,
        Direction::U => column_index,
        Direction::D => 49 - column_index,
    };

    // find the row index in the new cube
    let new_row = match new_dir {
        // if coming from left: row size, right: 49 - row size, up: 49 - column size, down: column size
        Direction::L => 49 - i,
        // if coming from left: 49 - row size, right: row size, up: column size, down: 49 - column size
        Direction::R => i,
        // if you enter a cube going up, you are in row 49
        Direction::U => 49,
        // if you enter a cube going down, you are in row 0
        Direction::D => 0,
    };
    //find the column index in the new cube
    let new_col = match new_dir {
        // if you enter a cube going left, you are in column 49
        Direction::L => 49,
        // if you enter a cube going right, you are in column 0
        Direction::R => 0,
        // if coming from left: 49 - row size, right: row size, up: column size, down: 49 - column size
        Direction::U => i,
        // if coming from left: row size, right: 49 - row size, up: 49 - column size, down: column size
        Direction::D => 49 - i,
    };

    // find the new position
    // multiply the cube (row, col) by 50 to add the offset for the row,col in the cube
    let new_pos = Coord {
        row: cube_row * 50 + new_row,
        col: cube_col * 50 + new_col,
    };

    // return the new position and direction
    (new_pos, new_dir)
}

fn part_1(input: &str) -> i32 {
    // parse the input and get our grid and instructions
    let (map, instructions) = parse(input);
    // find the index of the first open position in the first row
    let start_col = map[0].iter().position(|tile| *tile == Tile::Open).unwrap() as i32;

    // set our current position
    let mut pos = Coord {
        row: 0,
        col: start_col,
    };
    // set our current direction
    let mut dir = Direction::R;

    // go through each instruction
    for inst in &instructions {
        // match the instruction
        match inst {
            // if it's a rotation, rotate our direction
            Instruction::Rotate(turn) => dir = dir.turn(&turn),
            // if it's a forward instruction, move forward
            Instruction::Forward(num) => {
                // repeat num times
                for _ in 0..*num {
                    // get the offset of our current direction (e.g. facing right, offset is (0, 1) from current position)
                    let Coord {
                        row: row_offset,
                        col: col_offset,
                    } = dir.offset();

                    // get the tile at the new position
                    let new_tile = map
                        .get((pos.row + row_offset) as usize)
                        .and_then(|row| row.get((pos.col + col_offset) as usize))
                        .unwrap_or(&Tile::None);

                    // match the new tile
                    match new_tile {
                        // if new tile is solid, we stop moving in this direction and go to next instruction
                        Tile::Solid => break,
                        // if new tile is open, we move there
                        Tile::Open => {
                            pos = Coord {
                                row: pos.row + row_offset,
                                col: pos.col + col_offset,
                            };
                        }
                        // if new tile is not found, wrap around
                        Tile::None => {
                            // get the wrapped position
                            let new_pos = wrap(&map, &pos, &dir);

                            // if the wrapped position is a wall, we stop moving in this direction and go to next instruction
                            if map[new_pos.row as usize][new_pos.col as usize] == Tile::Solid {
                                break;
                            }
                            // otherwise we move to the wrapped position
                            pos = new_pos;
                        }
                    }
                }
            }
        }
    }

    // return the score
    // (adding 1 to row and col because index starts at 1)
    1000 * (pos.row + 1) + 4 * (pos.col + 1) + dir.score() as i32
}

fn part_2(input: &str) -> i32 {
    // parse the input and get our grid and instructions
    let (map, instructions) = parse(input);
    // find the index of the first open position in the first row
    let start_col = map[0].iter().position(|tile| *tile == Tile::Open).unwrap() as i32;

    // set our current position
    let mut pos = Coord {
        row: 0,
        col: start_col,
    };
    // set our current direction
    let mut dir = Direction::R;

    // go through each instruction
    for inst in &instructions {
        // match the instruction
        match inst {
            // if it's a rotation, rotate our direction
            Instruction::Rotate(turn) => dir = dir.turn(&turn),
            // if it's a forward instruction, move forward
            Instruction::Forward(num) => {
                // repeat num times
                for _ in 0..*num {
                    // get the offset of our current direction (e.g. facing right, offset is (0, 1) from current position)
                    let Coord {
                        row: row_offset,
                        col: col_offset,
                    } = dir.offset();

                    // get the tile at the new position
                    let new_tile = map
                        .get((pos.row + row_offset) as usize)
                        .and_then(|row| row.get((pos.col + col_offset) as usize))
                        .unwrap_or(&Tile::None);

                    // match the new tile
                    match new_tile {
                        // if new tile is solid, we stop moving in this direction and go to next instruction
                        Tile::Solid => break,
                        // if new tile is open, we move there
                        Tile::Open => {
                            pos = Coord {
                                row: pos.row + row_offset,
                                col: pos.col + col_offset,
                            };
                        }
                        // if new tile is not found, wrap around
                        Tile::None => {
                            // get the position and direction in the new 50x50 cube
                            let (new_pos, new_dir) = wrap2(&pos, &dir);

                            // if the position in the new cube is a wall, we stop moving in this direction and go to next instruction
                            if map[new_pos.row as usize][new_pos.col as usize] == Tile::Solid {
                                break;
                            }
                            // otherwise we move to the position in the new cube and update the direction
                            pos = new_pos;
                            dir = new_dir;
                        }
                    }
                }
            }
        }
    }

    // return the score
    // (adding 1 to row and col because index starts at 1)
    1000 * (pos.row + 1) + 4 * (pos.col + 1) + dir.score() as i32
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    let part2 = part_2(&input);
    println!("Part 1: {}", &part1);
    println!("Part 2: {}", &part2);
}
