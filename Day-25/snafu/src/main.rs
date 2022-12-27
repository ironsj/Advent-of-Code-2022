// convert snafu to decimal
fn to_decimal(snafu: &str) -> i64 {
    snafu.chars().fold(0, |decimal, snafu_digit| {
        let decimal_digit = ['=', '-', '0', '1', '2']
            .into_iter()
            // find the index of the snafu digit in the decimal digit array
            .position(|c| c == snafu_digit)
            .unwrap() as i64
            // subtract 2 to get the decimal digit (e.g. '=' index is 0, and '=' is -2, so subtract 2 from the index)
            - 2;
        // multiply decimal (the running total starting at 0) by 5, then add decimal digit
        decimal * 5 + decimal_digit

        /*
        Example:
        snafu: 122
        '1' is at index 3 in the decimal digit array, so 3 - 2 = 1
        0*5 + 1 = 1
        '2' is at index 4 in the decimal digit array, so 4 - 2 = 2
        1*5 + 2 = 7
        decimal is 7
        '2' is at index 4 in the decimal digit array, so 4 - 2 = 2
        7*5 + 2 = 37
        */
    })
}

// convert decimal to snafu
fn to_snafu(decimal: i64) -> String {
    // base case
    if decimal == 0 {
        return String::new();
    }

    // divide decimal by 5, then store the remainder
    let decimal_remainder = decimal % 5;
    // get the snafu digit from the decimal remainder
    let snafu_digit = ['0', '1', '2', '=', '-'][decimal_remainder as usize];

    // add 2 to the decimal remainder, then divide by 5
    // we add 2 to the decimal because it will move us over to the next decimal place (e.g. move us from the 5's place to the 25's place)
    // if the resulting new decimal is 0, then we know that we do not have any more decimal digits to convert and it will hit the base case
    // the +2 accounts for the fact that snafu digits start at -2
    let new_decimal = (decimal + 2) / 5;
    // find the snafu equivalent for the new decimal
    let mut snafu = to_snafu(new_decimal);
    // add the snafu digit to the snafu string
    snafu.push(snafu_digit);

    // return the snafu string
    snafu
}

fn part_1(input: &str) -> String {
    // convert all lines of input from snafu to decimal, then sum them
    let sum = input.lines().map(to_decimal).sum();

    // convert the sum from decimal to snafu
    to_snafu(sum)
}

fn main() {
    let input = std::fs::read_to_string("src/input.txt").unwrap();
    let part1 = part_1(&input);
    println!("Part 1: {}", &part1);
}
