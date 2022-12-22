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

fn depends_on_human(name: &str, monkeys: &HashMap<&str, Monkey>) -> bool {
    // return true if the "monkey" is us!
    if name == "humn" {
        return true;
    }
    match &monkeys[name] {
        // if the monkey is a number, return false as it doesn't depend on the human
        Monkey::Num(_) => false,
        // if the monkey is a calculation, return true if either side depends on the human
        Monkey::Calculated(_, lhs, rhs) => {
            depends_on_human(lhs, monkeys) || depends_on_human(rhs, monkeys)
        }
    }
}

fn calc_human(name: &str, value: i64, monkeys: &HashMap<&str, Monkey>) -> i64 {
    // if the monkey is the human, return the value passed in as a parameter
    if name == "humn" {
        return value;
    }
    match &monkeys[name] {
        // if the monkey is a number, return the number it holds in the hash map
        Monkey::Num(n) => *n,
        // reorder all operations to solve for unknown side
        Monkey::Calculated(operator, lhs, rhs) => {
            // check if the left side depends on the human
            let (new_name, new_value) = if depends_on_human(lhs, monkeys) {
                // right side does not depend on human so calculate
                let rhs_num = calc_name(rhs, monkeys);
                // find the new value based on the operator
                let new_value = match operator {
                    // lhs = value - rhs
                    Operator::Add => value - rhs_num,
                    // lhs = value + rhs
                    Operator::Sub => value + rhs_num,
                    // lhs = value / rhs
                    Operator::Mul => value / rhs_num,
                    // lhs = value * rhs
                    Operator::Div => value * rhs_num,
                };
                // will be used to recursively call calc_human
                (lhs, new_value)
            } else {
                // left side does not depend on human so calculate
                let lhs_num = calc_name(lhs, monkeys);
                // find the new value based on the operator
                let new_value = match operator {
                    // rhs = value - lhs
                    Operator::Add => value - lhs_num,
                    // rhs = lhs - value
                    Operator::Sub => lhs_num - value,
                    // rhs = value / lhs
                    Operator::Mul => value / lhs_num,
                    // rhs = lhs / value
                    Operator::Div => lhs_num / value,
                };
                // will be used to recursively call calc_human
                (rhs, new_value)
            };

            // recursively call calc_human for the left or right side with the new value
            calc_human(new_name, new_value, monkeys)
        }
    }
}

fn part_1(input: &str) -> i64 {
    // parse the input into a hashmap
    let monkeys = parse(&input);
    // calculate from the root monkey
    calc_name("root", &monkeys)
}

fn part_2(input: &str) -> i64 {
    // parse the input into a hashmap
    let monkeys = parse(&input);

    // get the left and right sides of the root monkey calculation
    let Monkey::Calculated(_, lhs, rhs) = &monkeys["root"] else {
        panic!("root has to be a calculated monkey");
    };

    // first check if the left or right side depends on the human
    let (name, value) = if depends_on_human(lhs, &monkeys) {
        // right hand side doesn't depend on human so we can solve for it
        let rhs_num = calc_name(rhs, &monkeys);
        // assign name,value to the left hand side and right side value
        (lhs, rhs_num)
    } else {
        // left hand side doesn't depend on human so we can solve for it
        let lhs_num = calc_name(lhs, &monkeys);
        // assign name,value to the right hand side and left side value
        (rhs, lhs_num)
    };

    // find the value needed by the human so the lhs=rhs
    calc_human(name, value, &monkeys)
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    let part2 = part_2(&input);
    println!("Part 1: {}", &part1);
    println!("Part 2: {}", &part2);
}
