
using MingelBingoCreator.Data;

namespace MingelBingoCreator.Repository.GoogleSheetsHelpers
{
    internal static class A1NotationCreator
    {
        private static readonly int UnicodeLetterStart = 64;

        private static readonly int ColumnAzNumber = 27;

        public static List<A1Notation> GetA1NotationsForCells(List<List<Cell>> foundCells, string sheetName)
        {
            var foundCellsAreSquare = CheckIfFoundCellsAreInSquareFormation(foundCells);

            if (!foundCellsAreSquare)
                throw new NotSupportedException("Non square templates or templates with discontinuous ranges currently not supported.");

            return GetA1NotationForCellsInRectangle(foundCells, sheetName);
        }

        private static List<A1Notation> GetA1NotationForCellsInRectangle(List<List<Cell>> foundCells, string sheetName)
        {
            var firstCellA1Notation = GetA1NotationStringForSingleCellWithoutSheetName(foundCells[0][0]);

            var lastCellA1Notation = GetA1NotationStringForSingleCellWithoutSheetName(foundCells.Last().Last());

            return new List<A1Notation>
            {
                new A1Notation
                {
                    RangeRows = foundCells.Count,
                    RangeColumns = foundCells.First().Count,
                    A1NotationString = $"{sheetName}!{firstCellA1Notation}:{lastCellA1Notation}"
                }
            };
        }

        public static string GetA1NotationForSingleCell(string sheetName, Cell cell)
        {
            if (cell.ColumnIndex >= ColumnAzNumber)
                throw new NotSupportedException("Templates using more than Z column not supported.");

            return $"{sheetName}!{GetA1NotationStringForSingleCellWithoutSheetName(cell)}";
        }

        private static string GetA1NotationStringForSingleCellWithoutSheetName(Cell cell)
            => $"{(char)(UnicodeLetterStart + cell.ColumnIndex)}{cell.RowIndex}";

        private static bool CheckIfFoundCellsAreInSquareFormation(List<List<Cell>> foundCells)
        {
            if (!AreAllRowsSameLength(foundCells))
                return false;

            if (!DoAllRowsStartOnSameColumn(foundCells))
                return false;

            if (CheckIfEmptyCellsInRange(foundCells))
                return false;

            if (!CheckIfCellsAreContinous(foundCells))
                return false;

            return true;
        }

        private static bool AreAllRowsSameLength(List<List<Cell>> foundCells)
        {
            var firstRowLength = foundCells[0].Count;

            for (int i = 1; i < foundCells.Count; i++)
                if (firstRowLength != foundCells[i].Count)
                    return false;

            return true;
        }

        private static bool DoAllRowsStartOnSameColumn(List<List<Cell>> foundCells)
        {
            var firstRowStartingColumn = foundCells[0].First().ColumnIndex;

            for (int i = 1; i < foundCells.Count; i++)
                if (firstRowStartingColumn != foundCells[i].First().ColumnIndex)
                    return false;

            return true;
        }

        private static bool CheckIfEmptyCellsInRange(List<List<Cell>> foundCells)
        {
            for (int i = 0; i < foundCells.Count; i++)
                for (int j = 0; j < foundCells[i].Count; j++)
                    if (foundCells[i][j] == null)
                        return true;

            return false;
        }

        private static bool CheckIfCellsAreContinous(List<List<Cell>> foundCells)
        {
            for (int i = 1; i < foundCells.Count; i++)
            {
                if (foundCells[i - 1].First().RowIndex + 1 != foundCells[i].First().RowIndex)
                    return false;

                for (int j = 1; j < foundCells[i].Count; j++)
                    if (foundCells[i][j-1].ColumnIndex + 1 != foundCells[i][j].ColumnIndex)
                        return false;
            }

            return true;
        }
    }
}
