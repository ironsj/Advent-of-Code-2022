using System.Text.Json.Nodes;

namespace Sand
{
    class SandProgram
    {
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split('\n');
            int result = SolvePart1(input);
            int result2 = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
            Console.WriteLine("Part 2: " + result2);
        }

        public static int SolvePart1(string[] input)
        {
            return 1;
        }

        public static int SolvePart2(string[] input)
        {
            return 1;
        }
    }
}