using MingelBingoCreator.Data;
using MingelBingoCreator.Repository.GoogleSheetsHelpers;
using System.Data.Common;
using Xunit;

namespace MingeBingoCreatorUnitTests.RepositoryTests.GoogleSheetsRepositoryTests
{
    public static class A1NotationCreatorTests
    {
        [Theory]
        [InlineData(0, 0, 'A')]
        [InlineData(1, 2, 'C')]
        [InlineData(7, 5, 'F')]
        [InlineData(60, 25, 'Z')]
        public static void GetA1NotationString_GetsCorrectCharacter(int row, int column, char character)
        {
            //Arrange
            var cell = new Cell
            {
                RowIndex = row,
                ColumnIndex = column
            };

            var sheetName = "test";

            var expected = sheetName + "!" + character + (row + 1).ToString();

            //Act
            var result = A1NotationCreator.GetA1NotationStringForSingleCell(sheetName, cell);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public static void GetA1NotationString_Throws_NotSupported_If_Outside_Range()
        {
            var columnAz = 27;

            var cell = new Cell
            {
                RowIndex = 10,
                ColumnIndex = columnAz
            };

            //Act and Assert
            Assert.Throws<NotSupportedException>(() => A1NotationCreator.GetA1NotationStringForSingleCell("test", cell));
        }

        [Theory]
        [InlineData(0, 0, 0, 0, "A1:A1")]
        [InlineData(0, 0, 3, 3, "A1:D4")]
        [InlineData(1, 1, 4, 4, "B2:E5")]
        [InlineData(0, 1, 0, 1, "B1:B1")]
        [InlineData(0, 0, 0, 1, "A1:B1")]
        [InlineData(0, 0, 1, 0, "A1:A2")]
        [InlineData(1, 1, 1, 4, "B2:E2")]
        [InlineData(5, 5, 19, 5, "F6:F20")]
        public static void GetA1NotationsForCells_CreatesCorrectRanges(int startRow, int startColumn, int endRow, int endColumn, string expectedNotation)
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            // % % % % %
            // % % % % %
            // % % % % %
            // % % % % %

            for (int i = startRow; i < endRow + 1; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = startColumn; j < endColumn + 1; j++)
                    foundCells.Last().Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j
                    });
            }

            //Act
            var result = A1NotationCreator.GetA1NotationsForCells(foundCells, sheetName);

            //Assert
            Assert.Single(result);

            var resultNotation = result.First();

            Assert.Equal($"{sheetName}!{expectedNotation}", resultNotation.A1NotationString);
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(3, 4, 3, 4)]
        [InlineData(1, 1, 2, 2)]
        [InlineData(1, 1, 1, 2)]
        [InlineData(1, 1, 2, 1)]
        [InlineData(4, 1, 5, 3)]
        public static void GetA1NotationsForCells_ReturnsCorrectRowAndColumnSize(int startRow, int startColumn, int endRow, int endColumn)
        {
            //Arrange
            var foundCells = new List<List<Cell>>();

            for (int i = startRow; i < endRow + 1; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = startColumn; j < endColumn + 1; j++)
                    foundCells.Last().Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j
                    });
            }

            //Act
            var result = A1NotationCreator.GetA1NotationsForCells(foundCells, "testName");

            //Assert
            Assert.Single(result);

            var resultNotation = result.First();

            Assert.Equal(foundCells.Count, resultNotation.NumberOfRows);
            Assert.Equal(foundCells.First().Count, resultNotation.NumberOfColumns);
        }

        [Fact]
        public static void GetA1NotationsForCells_Throws_NotSupported_If_CellsNotContinuousMatrix()
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            // % % % % %
            // % % % % %
            // % % X % %
            // % % % % %

            for (int i = 0; i < 4; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = 0; j < 5; j++)
                    foundCells[i].Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j
                    });
            }

            foundCells[2][2] = null;

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => A1NotationCreator.GetA1NotationsForCells(foundCells, sheetName));
        }

        [Fact]
        public static void GetA1NotationsForCells_Throws_NotSupported_If_CellsNotSquare()
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            // % % 
            //   % % 

            for (int i = 0; i < 2; i++)
            {
                foundCells.Add(new List<Cell>());

                var columnAdjustment = i;

                for (int j = 0; j < 2; j++)
                    foundCells[i].Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j + columnAdjustment
                    });
            }

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => A1NotationCreator.GetA1NotationsForCells(foundCells, sheetName));
        }

        [Fact]
        public static void GetA1NotationsForCells_Throws_NotSupported_If_CellsSkipRow()
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            // % %
            //
            // % %

            for (int i = 0; i < 2; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = 0; j < 2; j++)
                    foundCells[i].Add(new Cell
                    {
                        RowIndex = i * 2,
                        ColumnIndex = j
                    });
            }

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => A1NotationCreator.GetA1NotationsForCells(foundCells, sheetName));
        }

        [Fact]
        public static void GetA1NotationsForCells_Throws_NotSupported_If_CellsSkipColumn()
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            // %   %
            // %   %

            for (int i = 0; i < 2; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = 0; j < 2; j++)
                    foundCells[i].Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j * 2
                    });
            }

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => A1NotationCreator.GetA1NotationsForCells(foundCells, sheetName));
        }

        //TODO Sunshine tests
    }
}
