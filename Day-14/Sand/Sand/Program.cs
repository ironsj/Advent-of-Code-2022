namespace Sand
{
    class SandProgram
    {
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input_example.txt").Split('\n');
            int result = SolvePart1(input);
            int result2 = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
            Console.WriteLine("Part 2: " + result2);
        }

        public static int SolvePart1(string[] input)
        {
            HashSet<(int x, int y)> wall = Initialize(input);
            int restingSand = 0;
            
            return 1;
        }

        public static int SolvePart2(string[] input)
        {
            return 1;
        }

        public bool Stop((int x, int y) pos, HashSet<(int x, int y)> wall)
        {
            return wall.Count(m => m.y >= pos.y) == 1;
        }

        public static HashSet<(int, int)> Initialize(string[] input)
        {
            HashSet<(int, int)> wall = new HashSet<(int, int)>();
            foreach (string line in input)
            {
                string[] coordinate = line.Split(" -> ");
                for(int i = 1; i < coordinate.Length; i++)
                {
                    String[] coord1 = coordinate[i - 1].Split(",");
                    String[] coord2 = coordinate[i].Split(",");

                    int x1 = int.Parse(coord1[0]);
                    int x2 = int.Parse(coord2[0]);
                    int y1 = int.Parse(coord1[1]);
                    int y2 = int.Parse(coord2[1]);

                    if (x1 == x2)
                    {
                        for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                        {
                            wall.Add((x1, y));
                        }
                    }
                    else
                    {
                        for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                        {
                            wall.Add((x, y1));
                        }
                    }
                }
            }

            return wall;
        } 
    }
}