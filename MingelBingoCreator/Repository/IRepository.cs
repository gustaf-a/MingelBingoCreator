
using MingelBingoCreator.Data;

namespace MingelBingoCreator.Repository
{
    internal interface IRepository
    {
        /// <summary>
        /// Collects the columns of data from a sheet in the workbook.
        /// </summary>
        /// <param name="sheetId">Id of the spreadsheet.</param>
        /// <param name="sheetName">Name of the tab.</param>
        public Task<List<Category>> GetColumns(string sheetId, string sheetName);

        /// <summary>
        /// Counts the occurences of a value in a sheet
        /// </summary>
        /// <param name="sheetId">Id of the spreadsheet.</param>
        /// <param name="sheetName">Name of the tab.</param>
        /// <param name="value">String value to look for in cells</param>
        public Task<int> CountCellsWithValue(string sheetId, string sheetName, string value);

        /// <summary>
        /// Goes through all placeholder values in a sheet and replaces them with the provided values.
        /// </summary>
        /// <param name="spreadSheet">The spreadsheet to be worked on.</param>
        /// <param name="placeholderValue">Placeholder value to look for in cells</param>
        /// <param name="mingelBingoCards">Values to insert into the sheet instead of the placeholders.</param>
        /// <exception cref="PlaceholderCountMismatchException">Thrown when values count doesn't match number of placeholders in sheet.</exception>
        public Task<bool> ReplacePlaceholderWithValues(SpreadSheet spreadSheet, string placeholderValue, List<MingelBingoCard> mingelBingoCards);

        /// <summary>
        /// Creates a copy of a spreadsheet.
        /// </summary>
        /// <param name="originalSheetId"></param>
        /// <param name="newName"></param>
        public Task<TemplateSpreadSheet> CopyFile(string originalSheetId, string newName);
    }
}
