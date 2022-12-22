use std::collections::HashMap;

// a monkey knows a number or a calculation
#[derive(Debug)]
enum Monkey<'a> {
    // a number
    Num(i64),
    // the operator, the left side, the right side
    Calculated(Operator, &'a str, &'a str),
}

// the possible operators
#[derive(Debug)]
enum Operator {
    Add,
    Sub,
    Mul,
    Div,
}

// takes in input string and returns a hashmap with monkey names as the key and the job of the monkey as the value
fn parse(input: &str) -> HashMap<&str, Monkey> {
    input
        // split the input into lines
        .lines()
        // map each line using the monkey name and job
        .map(|line| {
            // get the name and job from the line
            let (name, right) = line.split_once(": ").unwrap();
            let monkey = match right.parse() {
                // if the job is a number, return a Num variant
                Ok(n) => Monkey::Num(n),
                // if the job is a calculation, return a Calculated variant
                Err(_) => {
                    // split the job into the operator and the two sides
                    let mut iter = right.split_ascii_whitespace();
                    // get the left side
                    let lhs = iter.next().unwrap();
                    // get the operator
                    let operator = match iter.next().unwrap() {
                        "+" => Operator::Add,
                        "-" => Operator::Sub,
                        "*" => Operator::Mul,
                        "/" => Operator::Div,
                        _ => panic!("Invalid math operator"),
                    };
                    // get the right side
                    let rhs = iter.next().unwrap();
                    // return the Calculated variant
                    Monkey::Calculated(operator, lhs, rhs)
                }
            };
            // return the name and the job
            (name, monkey)
        })
        //collect all of the monkeys into a hashmap
        .collect()
}

fn calc_name(name: &str, monkeys: &HashMap<&str, Monkey>) -> i64 {
    match &monkeys[name] {
        // if the monkey is a number, return the number it holds in the hash map
        Monkey::Num(n) => *n,
        // if the monkey is a calculation, calculate
        Monkey::Calculated(operator, lhs, rhs) => {
            // calculate the left side monkey by recursively calling calc_name
            let lhs_num = calc_name(lhs, monkeys);
            // calculate the right side monkey by recursively calling calc_name
            let rhs_num = calc_name(rhs, monkeys);
            // return the calculated value based on the operator
            match operator {
                Operator::Add => lhs_num + rhs_num,
                Operator::Sub => lhs_num - rhs_num,
                Operator::Mul => lhs_num * rhs_num,
                Operator::Div => lhs_num / rhs_num,
            }
        }
    }
}

fn part_1(input: &str) -> i64 {
    // parse the input into a hashmap
    let monkeys = parse(&input);
    // calculate from the root monkey
    calc_name("root", &monkeys)
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    println!("Part 1: {}", &part1);
}
