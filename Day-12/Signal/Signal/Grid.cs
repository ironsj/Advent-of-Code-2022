using System.Data;

namespace Signal
{
    public class Grid
    {
        public readonly int Columns;
        public readonly int Rows;

        public readonly Cell[,] MyGrid;
        public readonly Cell? Start;
        public readonly Cell? End;

        private const char StartValue = 'S';
        private const char EndValue = 'E';
        private const char Lowest = 'a';
        private const char Heighest = 'z';

        private readonly Dictionary<Cell, List<Cell>> Neighbors = new();

        public Grid(string[] input)
        {
            string[] lines = input.Select(line => line.Trim()).ToArray();
            Columns = lines[0].Length;
            Rows = lines.Length;

            MyGrid = new Cell[Rows, Columns];

            for (int row = 0; row < Rows; ++row)
            {
                for (int col = 0; col < Columns; ++col)
                {
                    // Create a new cell and fill with value from input
                    Cell cell = new(col, row, lines[row][col]);

                    // Add to grid
                    MyGrid[row, col] = cell;

                    // Check if the cell is the start or end
                    if (cell.Value == StartValue)
                    {
                        Start = cell;
                    }
                    else if (cell.Value == EndValue)
                    {
                        End = cell;
                    }
                }
            }

            for (var row = 0; row < Rows; ++row)
            {
                for (var col = 0; col < Columns; ++col)
                {
                    var cell = MyGrid[row, col];

                    // find all neighbors
                    var leftNeighbor = FindCell(row, col - 1);
                    var rightNeighbor = FindCell(row, col + 1);
                    var upNeighbor = FindCell(row - 1, col);
                    var downNeighbor = FindCell(row + 1, col);

                    // Add all neighbors to a list
                    var neighbors = new List<Cell?> { leftNeighbor, rightNeighbor, upNeighbor, downNeighbor, };

                    // Add all neighbors to the dictionary for a cell
                    // If the neighbor is null (we are on an edge), it will not be added
                    // If the neighbor is not possible to visit (e.g. 'a' can not visit anything but 'a' and 'b'), it will not be added
                    Neighbors[cell] = neighbors
                        .Where(neighbor => neighbor is not null && IsValid(cell, neighbor))
                        .Select(neighbor => neighbor!)
                        .ToList();
                }
            }
        }

        // Retreive the neighbors of a cell from the dictionary
        public List<Cell> GetNeighbors(Cell cell) => Neighbors[cell];

        // Find the cell at a specific row and column
        // Return null if the row or column is out of bounds
        private Cell? FindCell(int row, int col)
        {
            return (row >= 0 && row < Rows && col >= 0 && col < Columns) ? MyGrid[row, col] : null;
        }

        // Check if a cell is valid to visit
        private static bool IsValid(Cell currentCell, Cell neighborCell)
        {
            // If the current cell is the start, it can only visit a cell if it's value is 'a'
            if (currentCell.Value == StartValue)
            {
                return neighborCell.Value == Lowest;
            }
            // If the neighboring cell is the end, it can only visit the cell if it's value is 'z'
            else if(neighborCell.Value == EndValue)
            {
                return currentCell.Value == Heighest;
            }

            // Otherwise, the neighboring cell must be one higher than/equal to the current cell
            return neighborCell.Value - currentCell.Value <= 1;
        }
    }
}
