use std::{
    cmp::Ordering,
    collections::{BinaryHeap, HashMap, HashSet},
    iter,
};

// struct that represents a coordinate on the map
#[derive(Debug, PartialEq, Eq, Hash, Clone, Copy)]
struct Coord {
    row: usize,
    col: usize,
}

// impl block for the Coord struct
impl Coord {
    // get the neighbors of a coordinate
    fn neighbors(&self, rows: usize, cols: usize) -> Vec<Self> {
        use Direction::*;
        let mut neighbors = Vec::new();
        if self.row > 0 {
            neighbors.push(self.add_dir(&Up));
        }
        if self.col < cols - 1 {
            neighbors.push(self.add_dir(&Right));
        }
        if self.row < rows - 1 {
            neighbors.push(self.add_dir(&Down));
        }
        if self.col > 0 {
            neighbors.push(self.add_dir(&Left));
        }
        neighbors
    }

    // add a direction to a coordinate
    fn add_dir(&self, dir: &Direction) -> Self {
        use Direction::*;
        match dir {
            Up => Coord {
                row: self.row - 1,
                col: self.col,
            },
            Right => Coord {
                row: self.row,
                col: self.col + 1,
            },
            Down => Coord {
                row: self.row + 1,
                col: self.col,
            },
            Left => Coord {
                row: self.row,
                col: self.col - 1,
            },
        }
    }

    // get the Manhattan distance between two coordinates
    fn manhattan(&self, other: Coord) -> usize {
        other.col.abs_diff(self.col) + other.row.abs_diff(self.row)
    }
}

// enum that identifies the type of tile
#[derive(Debug, PartialEq, Eq)]
enum Tile {
    Wall,
    Blizzard(Direction),
}

// enum that identifies the direction of the blizzard
#[derive(Debug, PartialEq, Eq, Hash, Clone, Copy)]
enum Direction {
    Up,
    Right,
    Down,
    Left,
}

// struct that represents a node in the search tree
#[derive(PartialEq, Eq)]
struct Node {
    cost: usize,
    heuristic: usize,
    pos: Coord,
}

// impl blocks for the Node struct (this allows for comparison of nodes in the BinaryHeap)
impl Ord for Node {
    // the ordering of nodes is based on the cost
    fn cmp(&self, other: &Self) -> Ordering {
        let self_total = self.cost + self.heuristic;
        let other_total = other.cost + other.heuristic;
        other_total.cmp(&self_total)
    }
}

impl PartialOrd for Node {
    // the ordering of nodes is based on the cost
    fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

// struct that holds the map information
struct MapInfo {
    rows: usize,
    cols: usize,
    walls: HashSet<Coord>,
    blizzard_maps: HashMap<usize, HashSet<Coord>>,
    repeats_at: usize,
}

fn parse(input: &str) -> (HashMap<Coord, Tile>, usize, usize) {
    // a hashmap that hold the coordinates of walls and blizzards
    let mut map = HashMap::new();

    // the number of rows and columns in the map
    let rows = input.lines().count();
    let cols = input.lines().next().unwrap().chars().count();

    // iterate over the lines of the input
    for (row, line) in input.lines().enumerate() {
        // iterate over the characters of the line
        for (col, c) in line.chars().enumerate() {
            // if the character is a period (a ground tile), skip it
            if c == '.' {
                continue;
            }

            // create a coordinate for the row and column
            let coord = Coord { row, col };
            // create a tile based on the character
            let tile = match c {
                '#' => Tile::Wall,
                '^' => Tile::Blizzard(Direction::Up),
                'v' => Tile::Blizzard(Direction::Down),
                '<' => Tile::Blizzard(Direction::Left),
                '>' => Tile::Blizzard(Direction::Right),
                _ => panic!("invalid input"),
            };
            // insert the coordinate and tile into the map
            map.insert(coord, tile);
        }
    }

    // return the HashMap and the number of rows and columns
    (map, rows, cols)
}

fn lcm(first: usize, second: usize) -> usize {
    // the least common multiple of two numbers is the product of the two numbers divided by their greatest common divisor
    first * second / gcd(first, second)
}

fn gcd(first: usize, second: usize) -> usize {
    let mut max = first;
    let mut min = second;
    // make sure max is greater than min
    if min > max {
        // swap the values if min is greater than max
        std::mem::swap(&mut max, &mut min);
    }

    // keep dividing max by min until the remainder is 0
    loop {
        // divide max by min and store the remainder
        let result = max % min;
        // if the remainder is 0, return min
        if result == 0 {
            return min;
        }

        // set max to min and min to the remainder
        max = min;
        min = result;
    }
}

fn blizzard_maps(
    map: &HashMap<Coord, Tile>,
    rows: usize,
    cols: usize,
    max_time: usize,
) -> HashMap<usize, HashSet<Coord>> {
    // a hashmap that holds the coordinates of blizzards at each time
    let mut cache = HashMap::new();

    // a vector that holds the coordinates and directions of blizzards
    let mut blizzards: Vec<(Coord, Direction)> = map
        .iter()
        .filter_map(|(pos, tile)| match tile {
            Tile::Wall => None,
            Tile::Blizzard(dir) => Some((*pos, *dir)),
        })
        .collect();

    // get all of the blizzard coordinates from the vector
    let coords = blizzards.iter().map(|(coord, _)| *coord).collect();
    // insert the blizzard coordinates into the cache for time 0
    cache.insert(0, coords);

    // iterate over the amount of time before the all blizzards are in the same spot
    for time in 1..max_time {
        // iterate over the blizzards
        for (coord, dir) in blizzards.iter_mut() {
            // move the blizzard in its direction
            *coord = coord.add_dir(dir);

            // if the blizzard is at a wall, wrap it around to the other side
            match dir {
                Direction::Left => {
                    if coord.col == 0 {
                        coord.col = cols - 2;
                    }
                }
                Direction::Right => {
                    if coord.col == cols - 1 {
                        coord.col = 1;
                    }
                }
                Direction::Up => {
                    if coord.row == 0 {
                        coord.row = rows - 2;
                    }
                }
                Direction::Down => {
                    if coord.row == rows - 1 {
                        coord.row = 1;
                    }
                }
            }
        }

        // get all of the blizzard coordinates from the vector
        let coords = blizzards.iter().map(|(coord, _)| *coord).collect();
        // insert the blizzard coordinates into the cache for the current time
        cache.insert(time, coords);
    }

    // return the blizzard coordinates for each time before they all repeat
    cache
}

fn shortest(from: Coord, to: Coord, start_time: usize, map_info: &MapInfo) -> usize {
    // destructure the map info
    let MapInfo {
        rows,
        cols,
        walls,
        blizzard_maps,
        repeats_at,
    } = map_info;

    // a priority queue that holds the nodes to visit ordered by total cost
    let mut pq = BinaryHeap::new();
    // keep track of our visited coordinates and at what time we visited them (backtracking is allowed)
    let mut seen = HashSet::new();

    // add the start node to the priority
    pq.push(Node {
        cost: start_time,
        heuristic: from.manhattan(to),
        pos: from,
    });
    // add the start node to the seen set at the starting time
    seen.insert((from, start_time));

    // keep looping until the priority queue is empty
    while let Some(Node { cost, pos, .. }) = pq.pop() {
        // if we find a node that is at the end coordinate, return the cost as it is our shortest path
        if pos == to {
            return cost;
        }

        // the cost of the next node is the current cost plus 1
        let new_cost = cost + 1;
        // get the coordinates of the blizzards at the new cost modulo the lcm (since the blizzards repeat)
        let blizzards = &blizzard_maps[&(new_cost % repeats_at)];

        let candidates = pos
            .neighbors(*rows, *cols)
            // add all neighbors into an iterator (since we can choose to move to these)
            .into_iter()
            // add the current node into the iterator (since we can choose not to move)
            .chain(iter::once(pos))
            // filter out any neighbors that are walls or blizzards
            .filter(|coord| !walls.contains(coord))
            .filter(|coord| !blizzards.contains(coord));

        // iterate over all possible candidates to move to (including not moving)
        for new_pos in candidates {
            // push to the priority queue if we haven't seen this coordinate at this time
            if seen.insert((new_pos, new_cost)) {
                pq.push(Node {
                    cost: new_cost,
                    heuristic: new_pos.manhattan(to),
                    pos: new_pos,
                });
            }
        }
    }
    usize::MAX
}

fn part_1(input: &str) -> usize {
    // parse the input into a HashMap of coordinates and tiles
    let (map, rows, cols) = parse(input);

    // get the coordinates of the walls
    let walls: HashSet<Coord> = map
        .iter()
        .filter(|(_, tile)| **tile == Tile::Wall)
        .map(|(pos, _)| *pos)
        .collect();

    // since the blizzards wrap around the map when they reach a wall
    // the horizontal blizzards repeat every cols - 2 steps and the vertical blizzards repeat every rows - 2 steps
    // therefore, the blizzards coordinates all repeat every lcm(row-2, col-2) steps
    let lcm = lcm(rows - 2, cols - 2);
    // get the coordinates of the blizzards at each time before they all repeat
    let blizzard_maps = blizzard_maps(&map, rows, cols, lcm);

    // the coordinate where we start
    let start = Coord { row: 0, col: 1 };
    // the coordinate where we end
    let end = Coord {
        row: rows - 1,
        col: cols - 2,
    };

    // a struct that holds the information about the map
    let map_info = MapInfo {
        rows,
        cols,
        repeats_at: lcm,
        walls,
        blizzard_maps,
    };

    // find the shortest path from the start to the end
    shortest(start, end, 0, &map_info)
}

fn part_2(input: &str) -> usize {
    // parse the input into a HashMap of coordinates and tiles
    let (map, rows, cols) = parse(input);

    // get the coordinates of the walls
    let walls: HashSet<Coord> = map
        .iter()
        .filter(|(_, tile)| **tile == Tile::Wall)
        .map(|(pos, _)| *pos)
        .collect();

    // since the blizzards wrap around the map when they reach a wall
    // the horizontal blizzards repeat every cols - 2 steps and the vertical blizzards repeat every rows - 2 steps
    // therefore, the blizzards coordinates all repeat every lcm(row-2, col-2) steps
    let lcm = lcm(rows - 2, cols - 2);
    // get the coordinates of the blizzards at each time before they all repeat
    let blizzard_maps = blizzard_maps(&map, rows, cols, lcm);
    // the coordinate where we start
    let start = Coord { row: 0, col: 1 };
    // the coordinate where we end
    let end = Coord {
        row: rows - 1,
        col: cols - 2,
    };

    // a struct that holds the information about the map
    let map_info = MapInfo {
        rows,
        cols,
        repeats_at: lcm,
        walls,
        blizzard_maps,
    };

    // find the shortest path from the start to the end
    let there = shortest(start, end, 0, &map_info);
    // find the shortest path from the end to the start
    let back = shortest(end, start, there, &map_info);
    // find the shortest path from the start to the end again
    shortest(start, end, back, &map_info)
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    let part2 = part_2(&input);
    println!("Part 1: {}", &part1);
    println!("Part 2: {}", &part2);
}
