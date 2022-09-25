using MingelBingoCreator.Data;
using MingelBingoCreator.Repository.GoogleSheetsHelpers;
using Xunit;

namespace MingelBingoCreatorUnitTests.RepositoryTests.GoogleSheetsRepositoryTests
{
    public static class ValueRangeCreatorTests
    {
        [Fact]
        public static void ValueRangeCreator_CreateValueRangesForCells_Returns_Correct_Values()
        {
            //Arrange
            var sheetName = "testName";

            var foundCells = new List<List<Cell>>();

            for (int i = 1; i < 3; i++)
            {
                foundCells.Add(new List<Cell>());

                for (int j = 1; j < 4; j++)
                    foundCells.Last().Add(new Cell
                    {
                        RowIndex = i,
                        ColumnIndex = j
                    });
            }

            var values = new List<string>();

            for (int i = 0; i < 6; i++)
                values.Add(i.ToString());

            //Act
            var result = ValueRangeCreator.CreateValueRangesForCells(foundCells, values, sheetName);

            //Assert
            Assert.Single(result);

            var returnedValues = result.First().Values;

            Assert.Equal("0", returnedValues[0][0]);
            Assert.Equal("2", returnedValues[0][2]);
            Assert.Equal("3", returnedValues[1][0]);
            Assert.Equal("5", returnedValues[1][2]);
        }
    }
}
