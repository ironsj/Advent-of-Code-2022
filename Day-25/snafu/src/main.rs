fn part_1(input: &str) -> usize {
    return 1;
}

fn part_2(input: &str) -> usize {
    return 1;
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    let part2 = part_2(&input);
    println!("Part 1: {}", &part1);
    println!("Part 2: {}", &part2);
}
