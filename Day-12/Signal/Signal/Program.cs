namespace Signal
{
    class SignalProgram
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
            Grid grid = new(input);
            List<Cell> useBfs = BFS(grid, grid.Start, grid.End);
            // Path length - 1 because we don't count the start cell
            return useBfs.Count - 1;
        }

        public static int SolvePart2(string[] input)
        {
            Grid grid = new(input);
            // List of all possible starting spots (lowest heights + starting position)
            List<Cell> possibleStarts = new()
            {
                grid.Start
            };

            // Add all spots with lowest height to possibleStarts
            for (int x = 0; x < grid.Rows; x++)
            {
                for (int y = 0; y < grid.Columns; y++)
                {
                    Cell cell = grid.MyGrid[x, y];
                    if (cell.Value == 'a')
                    {
                        possibleStarts.Add(cell);
                    }
                }
            }

            // Do BFS on all starting positions
            // If the end is found and the path length is not 0 (for starting positions that did not find the end)
            // Add the path length to sequence and return the shortest path
            var shortest = possibleStarts.Select(start => BFS(grid, start, grid.End))
                .Where(path => path.Count > 0 && path.Last().Value == grid.End.Value)
                .Select(path => path.Count - 1)
                .Min();
            return shortest;
        }


        public static List<Cell> BFS(Grid grid, Cell start, Cell end)
        {
            // Dictionary containing a cell and the cell that discovered it (where we previously were)
            Dictionary<Cell, Cell?> bfsDiscovered = new()
            {
                { start, null }
            };

            // Queue of cells to visit
            Queue<Cell> bfsQueue = new();
            // Add starting cell
            bfsQueue.Enqueue(start);

            // While there are cells to visit
            while (bfsQueue.Count > 0)
            {
                // Get the next cell to visit
                Cell currentCell = bfsQueue.Dequeue();

                // If the current cell is the end cell, break from the loop
                if (currentCell == end)
                {
                    break;
                }

                // Get the neighbors of the current cell (cells that are 1 step away and we can possibly visit)
                foreach (Cell neighbor in grid.GetNeighbors(currentCell))
                {
                    // Only visit the neigbor if it has not been visited yet
                    if (!bfsDiscovered.ContainsKey(neighbor))
                    {
                        bfsDiscovered.Add(neighbor, currentCell);
                        bfsQueue.Enqueue(neighbor);
                    }
                }

            }

            // Take our dictionary of cells and who we visited from and follow the path backwards
            List<Cell> path = GetPath(start, end, bfsDiscovered);

            return path;
        }

        public static List<Cell> GetPath(Cell start, Cell end, Dictionary<Cell, Cell?> discovered)
        {
            // FOR PART 2: the starting cell does not contain a path to the end
            if (!discovered.ContainsKey(end))
            {
                return new List<Cell>();
            }

            // The path from start to end
            List<Cell> path = new List<Cell>();
            // Start at the end and work backwards
            Cell currentCell = end;
            // End when we reach the start
            while (currentCell != start)
            {
                // Add the current cell to the start
                path.Add(currentCell);
                // Find who we visited the cell from
                currentCell = discovered[currentCell]!;
            }
            // Add the start
            path.Add(start);
            // Reverse it
            path.Reverse();
            return path;
        }


    }
}