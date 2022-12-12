namespace Monkeys
{
    class MonkeyProgram
    {
        static readonly Dictionary<int, Monkey> monkeys = new();

        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split("\n\r\n");
            int result = SolvePart1(input);
            Console.WriteLine(result);
        }

        public static int SolvePart1(string[] input)
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
            int result = mostActive.Aggregate((x, y) => x * y);
            return result;
        }


        internal class Monkey
        {
            public Queue<int> Items { get; set; } = new();
            public string Operation { get; set; }
            public int DivisibleBy { get; set; }
            public int ThrowToTrue { get; set; }
            public int ThrowToFalse { get; set; }
            public int NumInspections { get; set; } = 0;

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
                    int worryLevel = Items.Dequeue();
                    Worry(worryLevel);
                }

            }

            public void Worry(int worry)
            {
                // get operator, * or +
                string op = Operation.Contains('*') ? "*" : "+";
                // get value on each side
                string[] values = Operation.Split("new = ")[1].Split(op);
                // parse left value
                int value1 = values[0].StartsWith('o') ? worry : int.Parse(values[0].Trim());
                // get rid of tab at end of second value
                values[1] = values[1].Replace('\n', ' ').Trim();
                // parse second value
                int value2 = values[1].StartsWith('o') ? worry : int.Parse(values[1]);
                // multiply or divide for worry level
                int increase = op.Equals("*") ? value1 * value2 : value1 + value2;
                // divide worry level by 3 because monkey didn't destroy item
                int finalLevel = increase / 3;

                // figure out who the monkey is going to throw to
                monkeys[(finalLevel % DivisibleBy == 0) ? ThrowToTrue : ThrowToFalse].Items.Enqueue(finalLevel);
            }
        }
    }


}


