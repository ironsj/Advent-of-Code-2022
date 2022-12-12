namespace Monkeys
{
    class MonkeyProgram
    {
        static readonly Dictionary<int, Monkey> monkeys = new();

        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split("\n\r\n");
            // long result = SolvePart1(input);
            long result = SolvePart2(input);
            Console.WriteLine(result);
        }

        public static long SolvePart1(string[] input)
        {
            foreach (string s in input)
            {
                string[] lines = s.Split("\n");
                int id = int.Parse(lines[0].Split("Monkey ")[1][..1]);
                monkeys[id] = new Monkey(
                    items: lines[1].Split(": ")[1].Split(",").Select(number => int.Parse(number)).ToList(),
                    operation: lines[2].Split(": ")[1],
                    divisible: int.Parse(lines[3].Split("by ")[1]),
                    throwToTrue: int.Parse(lines[4].Split("monkey ")[1]),
                    throwToFalse: int.Parse(lines[5].Split("monkey ")[1])
                );
            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < monkeys.Count; j++)
                {
                    monkeys[j].InspectItem();
                }
            }

            // Order the monkeys from least to most inspections and select the top 2
            var mostActive = monkeys.Values.OrderBy(monkey => monkey.NumInspections).TakeLast(2).Select(monkey => monkey.NumInspections).ToList();
            // multiply each value in the list together
            long result = mostActive.Aggregate((x, y) => x * y);
            return result;
        }

        public static long SolvePart2(string[] input)
        {
            foreach (string s in input)
            {
                string[] lines = s.Split("\n");
                int id = int.Parse(lines[0].Split("Monkey ")[1][..1]);
                monkeys[id] = new Monkey(
                    items: lines[1].Split(": ")[1].Split(",").Select(number => int.Parse(number)).ToList(),
                    operation: lines[2].Split(": ")[1],
                    divisible: int.Parse(lines[3].Split("by ")[1]),
                    throwToTrue: int.Parse(lines[4].Split("monkey ")[1]),
                    throwToFalse: int.Parse(lines[5].Split("monkey ")[1])
                );
            }

            /* 
             * Product of all divisor tests for each monkey.
             * This is because (a mod kn) mod n ≡ a mod n
             * Since a mod n∈[0,n−1], the second mod in (a mod kn) mod n will have no effect
             * 
             * Now that we don't divide by 3 and are multiplying and adding, the worry levels for each item will grow large
             * However, since the only thing that matters is that our worry level is divisible for a monkey we just need to keep the level low enough to compute
             * 
             * A number that is divisible by x * y will also be divisble by x and y separately
             * Thus, it would make sense that (worryLevel % (x * y)) % x ≡ worryLevel % x
             * 
             * For example:
             * 783 % 9 = 0
             * 783 % 7 = 6
             * 
             * 783 % (9 * 7) = 27
             * 27 % 9 = 0
             * 27 % 7 = 6
             * 
             * Thus, we are keeping the worrying levels low without affecting the divisibility check
             * Without using this modulo, you would need a super computer!
             */
            long modulo = monkeys.Values.Select(m => m.DivisibleBy).Aggregate((x, y) => x * y);
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < monkeys.Count; j++)
                {
                    monkeys[j].InspectItem2(modulo);
                }
            }

            // Order the monkeys from least to most inspections and select the top 2
            var mostActive = monkeys.Values.OrderBy(monkey => monkey.NumInspections).TakeLast(2).Select(monkey => monkey.NumInspections).ToList();
            // multiply each value in the list together
            long result = mostActive.Aggregate((x, y) => x * y);
            return result;
        }


        internal class Monkey
        {
            public Queue<long> Items { get; set; } = new();
            public string Operation { get; set; }
            public long DivisibleBy { get; set; }
            public int ThrowToTrue { get; set; }
            public int ThrowToFalse { get; set; }
            public long NumInspections { get; set; } = 0;

            public Monkey(List<int> items, string operation, int divisible, int throwToTrue, int throwToFalse)
            {
                this.Operation = operation;
                this.DivisibleBy = divisible;
                this.ThrowToTrue = throwToTrue;
                this.ThrowToFalse = throwToFalse;
                items.ForEach(item => Items.Enqueue(item));
            }

            public void InspectItem()
            {
                // inspect each item
                while (Items.Count > 0)
                {
                    NumInspections++;
                    long worryLevel = Items.Dequeue();
                    Worry(worryLevel);
                }
            }

            public void Worry(long worry)
            {
                // get operator, * or +
                string op = Operation.Contains('*') ? "*" : "+";
                // get value on each side
                string[] values = Operation.Split("new = ")[1].Split(op);
                // parse left value
                long value1 = values[0].StartsWith('o') ? worry : long.Parse(values[0].Trim());
                // get rid of tab at end of second value
                values[1] = values[1].Replace('\n', ' ').Trim();
                // parse second value
                long value2 = values[1].StartsWith('o') ? worry : long.Parse(values[1]);
                // multiply or divide for worry level
                long increase = op.Equals("*") ? value1 * value2 : value1 + value2;
                // divide worry level by 3 because monkey didn't destroy item
                long finalLevel = increase / 3;

                // figure out who the monkey is going to throw to
                monkeys[(finalLevel % DivisibleBy == 0) ? ThrowToTrue : ThrowToFalse].Items.Enqueue(finalLevel);
            }

            public void InspectItem2(long modulo)
            {
                // inspect each item
                while (Items.Count > 0)
                {
                    NumInspections++;
                    long worryLevel = Items.Dequeue();
                    Worry2(worryLevel, modulo);
                }
            }

            public void Worry2(long worry, long modulo)
            {
                // get operator, * or +
                string op = Operation.Contains('*') ? "*" : "+";
                // get value on each side
                string[] values = Operation.Split("new = ")[1].Split(op);
                // parse left value
                long value1 = values[0].StartsWith('o') ? worry : long.Parse(values[0].Trim());
                // get rid of tab at end of second value
                values[1] = values[1].Replace('\n', ' ').Trim();
                // parse second value
                long value2 = values[1].StartsWith('o') ? worry : long.Parse(values[1]);
                // multiply or divide for worry level
                long increase = op.Equals("*") ? value1 * value2 : value1 + value2;
                // modulo the worry level by the product of the divisors for each monkey
                long finalLevel = increase % modulo;

                // figure out who the monkey is going to throw to
                monkeys[(finalLevel % DivisibleBy == 0) ? ThrowToTrue : ThrowToFalse].Items.Enqueue(finalLevel);
            }
        }
    }
}