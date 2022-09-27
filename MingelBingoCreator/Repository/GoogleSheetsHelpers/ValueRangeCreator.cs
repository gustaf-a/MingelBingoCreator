using Google.Apis.Sheets.v4.Data;
using MingelBingoCreator.Data;

namespace MingelBingoCreator.Repository.GoogleSheetsHelpers
{
    internal static class ValueRangeCreator
    {
        internal static IEnumerable<ValueRange> CreateValueRangesForCard(List<A1Notation> a1NotationRanges, List<string> values, string sheetName)
        {
            var valueRanges = new List<ValueRange>();

            var valueTakenIndex = 0;

            foreach (var a1Notation in a1NotationRanges)
            {
                valueRanges.Add(new ValueRange
                {
                    Range = a1Notation.GetA1NotationString(sheetName),
                    Values = GetValuesOfSize(values, a1Notation.NumberOfRows, a1Notation.NumberOfColumns)
                });

                valueTakenIndex += a1Notation.NumberOfRows * a1Notation.NumberOfColumns;
            }

            return valueRanges;
        }

        private static IList<IList<object>> GetValuesOfSize(List<string> values, int rows, int columns)
        {
            var result = new List<IList<object>>();

            var valuesTakenCounter = 0;

            for (int i = 0; i < rows; i++)
            {
                result.Add(new List<object>());

                for (int j = 0; j < columns; j++)
                {
                    result[i].Add(values[valuesTakenCounter]);

                    valuesTakenCounter++;
                }
            }

            return result;
        }
    }
}
