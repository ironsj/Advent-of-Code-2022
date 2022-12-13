

namespace Signal
{
    class SignalProgram
    {
        static void Main()
        {
            string[] input = File.ReadAllText(@"../../../../../input.txt").Split('\n');
            long result = SolvePart1(input);
            // long result = SolvePart2(input);
            Console.WriteLine("Part 1: " + result);
        }

        public static int SolvePart1(string[] input)
        {
            Grid grid = new(input);
            List<Cell> useBfs = BFS(grid, grid.Start, grid.End);
            return useBfs.Count - 1;
        }

        public static int SolvePart2(string[] input)
        {
        }


        public static List<Cell> BFS(Grid grid, Cell start, Cell end)
        {
            Dictionary<Cell, Cell?> bfsDiscovered = new()
            {
                { start, null }
            };

            Queue<Cell> bfsQueue = new Queue<Cell>();
            bfsQueue.Enqueue(start);

            while(bfsQueue.Count > 0)
            {
                Cell currentCell = bfsQueue.Dequeue();
                
                if(currentCell == end)
                {
                    break;
                }

                foreach (Cell neighbor in grid.GetNeighbors(currentCell))
                {
                    if (!bfsDiscovered.ContainsKey(neighbor))
                    {
                        bfsDiscovered.Add(neighbor, currentCell);
                        bfsQueue.Enqueue(neighbor);
                    }
                }

            }

            List<Cell> path = GetPath(grid.Start, grid.End, bfsDiscovered);

            
            return path;
        }

        public static List<Cell> GetPath(Cell start, Cell end, Dictionary<Cell, Cell?> discovered)
        {
            List<Cell> path = new List<Cell>();
            Cell currentCell = end;
            while (currentCell != start)
            {
                path.Add(currentCell);
                currentCell = discovered[currentCell]!;
            }
            path.Add(start);
            path.Reverse();
            return path;
        }


    }
}