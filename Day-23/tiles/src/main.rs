use itertools::Itertools;
use std::collections::{HashMap, HashSet};

// struct for the coordinates of the elves
#[derive(PartialEq, Eq, Hash, Clone, Copy)]
struct Coord {
    row: i32,
    col: i32,
}

//helping functions for the Coord struct
impl Coord {
    // returns the eight neighbors of the coordinate
    fn neighbors(&self) -> [Self; 8] {
        use Direction::*;
        let n = self.add_dir(&North);
        let ne = self.add_dir(&NorthEast);
        let e = self.add_dir(&East);
        let se = self.add_dir(&SouthEast);
        let s = self.add_dir(&South);
        let sw = self.add_dir(&SouthWest);
        let w = self.add_dir(&West);
        let nw = self.add_dir(&NorthWest);
        [n, ne, e, se, s, sw, w, nw]
    }

    // gets the coordinate in the given direction
    fn add_dir(&self, dir: &Direction) -> Self {
        use Direction::*;
        match dir {
            North => Coord {
                row: self.row - 1,
                col: self.col,
            },
            NorthEast => Coord {
                row: self.row - 1,
                col: self.col + 1,
            },
            East => Coord {
                row: self.row,
                col: self.col + 1,
            },
            SouthEast => Coord {
                row: self.row + 1,
                col: self.col + 1,
            },
            South => Coord {
                row: self.row + 1,
                col: self.col,
            },
            SouthWest => Coord {
                row: self.row + 1,
                col: self.col - 1,
            },
            West => Coord {
                row: self.row,
                col: self.col - 1,
            },
            NorthWest => Coord {
                row: self.row - 1,
                col: self.col - 1,
            },
        }
    }
}

// the eight directions an elf can consider
enum Direction {
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
}

// helping functions for the Direction enum
impl Direction {
    fn check(&self, neighbors: &[bool; 8]) -> bool {
        // the 8 neighbors of the elf and whether or not they are occupied
        let [n, ne, e, se, s, sw, w, nw] = neighbors;
        match &self {
            // no elf n, ne, nw, elf proposes to move north
            Direction::North => !n && !ne && !nw,
            // no elf s, se, sw, elf proposes to move south
            Direction::South => !s && !se && !sw,
            // no elf w, nw, sw, elf proposes to move west
            Direction::West => !w && !nw && !sw,
            // no elf e, ne, se, elf proposes to move east
            Direction::East => !e && !ne && !se,
            _ => false,
        }
    }
}

fn parse(input: &str) -> HashSet<Coord> {
    // will hold the positions of the elves
    let mut elves = HashSet::new();
    // iterate over the lines of the input
    for (row, line) in input.lines().enumerate() {
        // iterate over the characters of the line
        for (col, c) in line.chars().enumerate() {
            // if the character is a '#', add the coordinate to the HashSet
            if c == '#' {
                // add the coordinate to the HashSet
                elves.insert(Coord {
                    row: row as i32,
                    col: col as i32,
                });
            }
        }
    }
    // return the HashSet
    elves
}

fn part_1(input: &str) -> usize {
    // get all of the elves from the input
    let mut elves = parse(input);
    // the four directions the elves can propose in order
    let mut direction_checks = [
        Direction::North,
        Direction::South,
        Direction::West,
        Direction::East,
    ];

    for round in 1.. {
        // the key is the proposed coordinate and the value is a list of elves coordinates that propose going to it
        let mut proposals: HashMap<Coord, Vec<Coord>> = HashMap::new();

        // PHASE ONE: Consider 8 positions
        for elf in &elves {
            // get the eight neighbors of the elf
            let neighbors = elf.neighbors();
            // check if all of the neighbors are empty. if so, the elf doesn't do anything this round
            if neighbors.iter().all(|coord| !elves.contains(coord)) {
                continue;
            }

            // get a list of booleans that indicate if the neighbor is occupied
            let neighbors = neighbors
                .iter()
                .map(|neighbor| elves.contains(neighbor))
                .collect::<Vec<_>>()
                .try_into()
                .unwrap();

            // check if the elf can move in any of the four directions, returns the first that it can propose
            let proposed_dir = direction_checks.iter().find(|dir| dir.check(&neighbors));
            // if the elf can move in a direction
            if let Some(dir) = proposed_dir {
                // get the coordinate the elf would move to in the proposed direction
                let proposal = elf.add_dir(dir);
                // add the elf to the list of elves that would move to the proposed coordinate
                proposals.entry(proposal).or_default().push(*elf);
            }
        }

        // PHASE TWO: Move
        for (new_coord, old_coords) in proposals {
            // if only one elf would move to the proposed coordinate, move the elf
            if old_coords.len() == 1 {
                // remove the old coordinate from the HashSet
                elves.remove(&old_coords[0]);
                // add the new coordinate to the HashSet
                elves.insert(new_coord);
            }
        }

        // move the first direction to the end of the list (first round North moves to back, second round South moves to back, etc.)
        direction_checks.rotate_left(1);

        // we have reached our final round
        if round == 10 {
            // get the min and max row and column of the elves
            let (minmax_row, minmax_col) = elves.iter().fold(
                // initialize min and max row and column to the max and min values of i32
                ((i32::MAX, i32::MIN), (i32::MAX, i32::MIN)),
                // for each elf, update the min and max row and column
                |(minmax_row, minmax_col), Coord { row, col }| {
                    (
                        (minmax_row.0.min(*row), minmax_row.1.max(*row)),
                        (minmax_col.0.min(*col), minmax_col.1.max(*col)),
                    )
                },
            );

            // find all of the empty spaces in the grid where an elf is not present and return the count
            return (minmax_row.0..=minmax_row.1)
                .cartesian_product(minmax_col.0..=minmax_col.1)
                .filter(|(row, col)| {
                    !elves.contains(&Coord {
                        row: *row,
                        col: *col,
                    })
                })
                .count();
        }
    }
    usize::MAX
}

fn part_2(input: &str) -> i32 {
    // get all of the elves from the input
    let mut elves = parse(input);
    // the four directions the elves can propose in order
    let mut checks = [
        Direction::North,
        Direction::South,
        Direction::West,
        Direction::East,
    ];

    for round in 1.. {
        // the moved boolean changes to true if an elf moves. if no elves move at the end of a round, we have reached our final round
        let mut moved = false;
        // the key is the proposed coordinate and the value is a list of elves coordinates that propose going to it
        let mut proposals: HashMap<Coord, Vec<Coord>> = HashMap::new();

        // PHASE ONE: Consider 8 positions
        for elf in &elves {
            // get the eight neighbors of the elf
            let neighbors = elf.neighbors();
            // check if all of the neighbors are empty. if so, the elf doesn't do anything this round
            if neighbors.iter().all(|coord| !elves.contains(coord)) {
                continue;
            }

            // get a list of booleans that indicate if the neighbor is occupied
            let neighbors = neighbors
                .iter()
                .map(|neighbour| elves.contains(neighbour))
                .collect::<Vec<_>>()
                .try_into()
                .unwrap();

            // check if the elf can move in any of the four directions, returns the first that it can propose
            let proposed_dir = checks.iter().find(|dir| dir.check(&neighbors));
            // if the elf can move in a direction
            if let Some(dir) = proposed_dir {
                // get the coordinate the elf would move to in the proposed direction
                let proposal = elf.add_dir(dir);
                // add the elf to the list of elves that would move to the proposed coordinate
                proposals.entry(proposal).or_default().push(*elf);
            }
        }

        // PHASE TWO: Move
        for (new_coord, old_coords) in proposals {
            // if only one elf would move to the proposed coordinate, move the elf
            if old_coords.len() == 1 {
                // an elf has moved, change the moved boolean to true
                moved = true;
                // remove the old coordinate from the HashSet
                elves.remove(&old_coords[0]);
                // add the new coordinate to the HashSet
                elves.insert(new_coord);
            }
        }

        // round finished, if no elves moved return the current round
        if !moved {
            return round;
        }

        // move the first direction to the end of the list (first round North moves to back, second round South moves to back, etc.)
        checks.rotate_left(1);
    }

    i32::MAX
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    let part2 = part_2(&input);
    println!("Part 1: {}", &part1);
    println!("Part 2: {}", &part2);
}
