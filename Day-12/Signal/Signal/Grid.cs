using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal
{
    public class Grid
    {
        public readonly int Columns;
        public readonly int Rows;

        public readonly Cell[,] MyGrid;
        public readonly Cell Start;
        public readonly Cell End;

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
                    Cell cell = new(col, row, lines[row][col]);

                    MyGrid[row, col] = cell;

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

                    var leftNeighbor = FindCell(row, col - 1);
                    var rightNeighbor = FindCell(row, col + 1);
                    var upNeighbor = FindCell(row - 1, col);
                    var downNeighbor = FindCell(row + 1, col);

                    var neighbors = new List<Cell?> { leftNeighbor, rightNeighbor, upNeighbor, downNeighbor, };

                    Neighbors[cell] = neighbors
                        .Where(neighbor => neighbor is not null && IsValid(cell, neighbor))
                        .Select(neighbor => neighbor!)
                        .ToList();
                }
            }
        }

        public List<Cell> GetNeighbors(Cell cell) => Neighbors[cell];

        private Cell? FindCell(int row, int col)
        {
            return (row >= 0 && row < Rows && col >= 0 && col < Columns) ? MyGrid[row, col] : null;
        }

        private bool IsValid(Cell currentCell, Cell neighborCell)
        {
            if(currentCell.Value == StartValue)
            {
                return neighborCell.Value == Lowest;
            }
            else if(neighborCell.Value == EndValue)
            {
                return currentCell.Value == Heighest;
            }

            return neighborCell.Value - currentCell.Value <= 1;
        }
    }
}
