using System.Diagnostics;

namespace Sand
{
    class SandProgram
    {
        private static HashSet<(int x, int y)> Wall { get; set; } = new();
        private static int Floor { get; set; }
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
            // find places sand can't fall through
            Initialize(input);
            // number of units of sand dropped
            int droppedSand = 0;
            // drop sand from starting point, iterate until no more sand can fall
            while (MoveSand((500, 0)))
            {
                droppedSand++;
            }

            return droppedSand;
        }

        public static int SolvePart2(string[] input)
        {
            // clear the HashSet for the second part
            Wall.Clear();
            Initialize(input);

            // the heighest y coordinate plus 1 (1 unit above the floor where we are standing)
            Floor = Wall.Max(wall => wall.y) + 1;

            int droppedSand = 0;
            // loop while sand can still drop (the source is not covered)
            while (CanDropSand())
            {
                droppedSand++;
            }

            return droppedSand;
        }

        public static bool Stop((int x, int y) pos)
        {
            // count the number of walls with a y coordinate greater than/equal to the y position of the current position
            // if this equals 1 this means we are falling into the abyss as nothing will be able to stop it after this point
            return Wall.Count(m => m.y >= pos.y) == 1;
        }

        public static bool Stop2((int x, int y) pos)
        {
            // stop if the new y position is equal to the floor of the cave
            return pos.y == Floor;
        }

        public static bool CanDropSand()
        {
            // if the source is in the Walls (covered) return false
            if (Wall.Contains((500, 0)))
            {
                return false;
            }
            // drop from the source
            Wall.Add((500, 0));
            MoveSand2((500, 0));
            return true;
        }

        public static void Initialize(string[] input)
        {
            foreach (string line in input)
            {
                // get string array of each coordinate of wall for a given line
                string[] coordinate = line.Split(" -> ");
                for(int i = 1; i < coordinate.Length; i++)
                {
                    // get x and y of first two coordinate pairs
                    String[] coord1 = coordinate[i - 1].Split(",");
                    String[] coord2 = coordinate[i].Split(",");

                    int x1 = int.Parse(coord1[0]);
                    int x2 = int.Parse(coord2[0]);
                    int y1 = int.Parse(coord1[1]);
                    int y2 = int.Parse(coord2[1]);

                    // if the x values are the same, the wall is vertical
                    if (x1 == x2)
                    {
                        // add each x,y position between the two y values to Wall
                        for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                        {
                            Wall.Add((x1, y));
                        }
                    }
                    // if the y values are the same, the wall is horizontal
                    else
                    {
                        // add each x,y position between the two x values to Wall
                        for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                        {
                            Wall.Add((x, y1));
                        }
                    }
                }
            }
        }

        public static bool MoveSand((int x, int y) pos)
        {
            // if the sand is lower than the walls (falling into the abyss) at the current position, return false
            if(Stop((pos.x, pos.y)))
            {
                return false;
            }

            // check if the position below the current position is blocked
            if (!Wall.Contains((pos.x, pos.y + 1)))
            {
                // remove the current position from wall as it is not blocked
                Wall.Remove((pos.x, pos.y));
                // add the position below
                Wall.Add((pos.x, pos.y + 1));
                // call again for the position below
                return MoveSand((pos.x, pos.y + 1));
            }

            // check if the spot diagonally left is blocked
            if (!Wall.Contains((pos.x - 1, pos.y + 1)))
            {
                // remove current position
                Wall.Remove((pos.x, pos.y));
                // add the position diagonally to the left
                Wall.Add((pos.x - 1, pos.y + 1));
                // call for the next position
                return MoveSand((pos.x - 1, pos.y + 1));
            }

            // check if the spot diagonally right is blocked
            if (!Wall.Contains((pos.x + 1, pos.y + 1)))
            {
                // remove current position
                Wall.Remove((pos.x, pos.y));
                // add the position diagonally right
                Wall.Add((pos.x + 1, pos.y + 1));
                // call for the next position
                return MoveSand((pos.x + 1, pos.y + 1));
            }
            
            // if all of spots checked above are blocked, return true and drop a new piece of sand
            return true;
        }

        public static bool MoveSand2((int x, int y) pos)
        {
            // if the current y position is the floor return false
            if (Stop2((pos.x, pos.y)))
            {
                return false;
            }

            // check if the position below the current position is blocked
            if (!Wall.Contains((pos.x, pos.y + 1)))
            {
                // remove the current position from wall as it is not blocked
                Wall.Remove((pos.x, pos.y));
                // add the position below
                Wall.Add((pos.x, pos.y + 1));
                // call again for the position below
                return MoveSand2((pos.x, pos.y + 1));
            }

            // check if the spot diagonally left is blocked
            if (!Wall.Contains((pos.x - 1, pos.y + 1)))
            {
                // remove current position
                Wall.Remove((pos.x, pos.y));
                // add the position diagonally to the left
                Wall.Add((pos.x - 1, pos.y + 1));
                // call for the next position
                return MoveSand2((pos.x - 1, pos.y + 1));
            }

            // check if the spot diagonally right is blocked
            if (!Wall.Contains((pos.x + 1, pos.y + 1)))
            {
                // remove current position
                Wall.Remove((pos.x, pos.y));
                // add the position diagonally right
                Wall.Add((pos.x + 1, pos.y + 1));
                // call for the next position
                return MoveSand2((pos.x + 1, pos.y + 1));
            }

            // if all of spots checked above are blocked, return true and drop a new piece of sand
            return true;
        }
    }


}