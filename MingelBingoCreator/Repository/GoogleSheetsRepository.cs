using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository.GoogleSheetsHelpers;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Xml.Serialization;

namespace MingelBingoCreator.Repository
{
    internal class GoogleSheetsRepository : IRepository
    {
        private static readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };

        private SheetsService _sheetsService;

        public GoogleSheetsRepository(AppSettings appSettings)
        {
            _sheetsService = CreateService(appSettings);
        }

        private static SheetsService CreateService(AppSettings appSettings)
        {
            try
            {
                UserCredential credential;

                using (var stream =
                       new FileStream(appSettings.GoogleSheetsOptions.CredentialsFileName, FileMode.Open, FileAccess.Read))
                {
                    /* The file token.json stores the user's access and refresh tokens, and is created
                     automatically when the authorization flow completes for the first time. */
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        _scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                return new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = appSettings.GoogleSheetsOptions.ApplicationName
                });
            }
            catch (FileNotFoundException e)
            {
                Log.Error(e, "Exception thrown when creating service: {0}", e.Message);

                throw;
            }
        }

        public async Task<int> CountCellsWithValue(string sheetId, string sheetName, string value)
        {
            var response = await _sheetsService.Spreadsheets.Values.Get(sheetId, sheetName).ExecuteAsync();

            var counter = 0;

            foreach (var row in response.Values)
                foreach (var cellValue in row)
                {
                    if (cellValue == null)
                        continue;

                    if (value.Equals(cellValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        counter++;
                }

            return counter;
        }

        public async Task<List<Category>> GetColumns(string sheetId, string sheetName)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(sheetId, sheetName);

            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;

            var response = await request.ExecuteAsync();

            var result = new List<Category>();

            foreach (var columnData in response.Values)
            {
                var heading = columnData[0].ToString();
                if (string.IsNullOrWhiteSpace(heading))
                {
                    Log.Warning("Empty heading found in data. Skipping column.");
                    continue;
                }

                var values = new List<string>();

                for (int i = 1; i < columnData.Count; i++)
                {
                    var value = columnData[i];

                    if (value != null)
                        values.Add(columnData[i].ToString());
                }

                if (values.Count == 0)
                {
                    Log.Warning("No values found for heading '{0}'. Skipping column.", heading);
                    continue;
                }

                result.Add(new Category(heading, values));
            }

            return result;
        }

        public async Task<SpreadSheet> CopyFile(string originalSpreadsheetId, string newName)
        {
            var sheetIdToCopy = await GetSheetIdToCopy(originalSpreadsheetId);

            var createdSpreadSheet = await CreateNewSpreadSheet(newName);

            var copiedSheetId = await CopySheetToSpreadSheet(originalSpreadsheetId, sheetIdToCopy, createdSpreadSheet.SpreadsheetId);

            //TODO Add Google Drive Permissions and service to move spreadsheet from root folder location of template 

            Log.Information($"Successfully created {newName} in Google Drive root folder by copying {originalSpreadsheetId}.");

            var sheetIds = new List<int>();

            sheetIds.Add(copiedSheetId);

            foreach (var sheet in createdSpreadSheet.Sheets)
                sheetIds.Add(sheet.Properties.SheetId
                    ?? throw LogAndReturnException(new Exception($"Null sheetId found in created spreadsheet: {createdSpreadSheet.SpreadsheetId}")));

            return new SpreadSheet
            {
                Id = createdSpreadSheet.SpreadsheetId,
                Name = createdSpreadSheet.Properties.Title,
                TemplateSheetId = copiedSheetId,
                SheetIds = sheetIds
            };
        }

        private async Task<int> GetSheetIdToCopy(string originalSpreadsheetId)
        {
            var getSheetRequest = _sheetsService.Spreadsheets.Get(originalSpreadsheetId);

            var originalSpreadsheet = await getSheetRequest.ExecuteAsync()
                ?? throw LogAndReturnException(new Exception($"Failed to find spreadsheet to copy: {originalSpreadsheetId}")); ;

            if (originalSpreadsheet.Sheets.Count == 0)
                throw LogAndReturnException(new Exception($"No sheets (tabs) found in spreadsheet to copy: {originalSpreadsheetId}"));

            if (originalSpreadsheet.Sheets.Count > 1)
                throw LogAndReturnException(new NotSupportedException($"Multiple sheets in template spreadsheet not supported. Please use templates with only on sheet: {originalSpreadsheetId}"));

            return originalSpreadsheet.Sheets.First().Properties.SheetId
                ?? throw LogAndReturnException(new Exception($"Failed to find SheetId in spreadsheet to copy: {originalSpreadsheetId}"));
        }

        private async Task<Spreadsheet> CreateNewSpreadSheet(string newName)
        {
            var spreadSheetBody = new Spreadsheet
            {
                Properties = new SpreadsheetProperties
                {
                    Title = newName
                }
            };

            var createRequest = _sheetsService.Spreadsheets.Create(spreadSheetBody);

            var createdSpreadSheet = await createRequest.ExecuteAsync()
                ?? throw LogAndReturnException(new Exception("Failed to create new spreadsheet."));

            return createdSpreadSheet;
        }

        private async Task<int> CopySheetToSpreadSheet(string originalSpreadsheetId, int sheetIdToCopy, string destinationSpreadSheetId)
        {
            var copyRequestBody = new CopySheetToAnotherSpreadsheetRequest
            {
                DestinationSpreadsheetId = destinationSpreadSheetId,
            };

            var copySheetRequest = _sheetsService.Spreadsheets.Sheets.CopyTo(copyRequestBody, originalSpreadsheetId, sheetIdToCopy);

            var copySheetResponse = await copySheetRequest.ExecuteAsync()
                ?? throw LogAndReturnException(new Exception("Failed to copy sheet to new spreadsheet."));

            return copySheetResponse.SheetId
                ?? throw LogAndReturnException(new Exception($"Failed to find sheetId for destination spreadsheet's new sheet. Spreadsheet Id: {destinationSpreadSheetId}"));
        }

        internal async Task<SpreadSheet> CreateMingelBingoCardsFromTemplateSheet(SpreadSheet spreadSheet, int numberOfCardsToCreate)
        {
            var requests = new List<Request>();

            requests.AddRange(GetDuplicateTemplateSheetRequests(spreadSheet, numberOfCardsToCreate));

            //Removes the template sheet and automatically created Sheet1 which were in the original spreadsheet object.
            requests.AddRange(GetRemoveAllSheetsRequests(spreadSheet));

            var batchUpdateRequestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests,
                IncludeSpreadsheetInResponse = true
            };

            var batchUpdateRequest = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequestBody, spreadSheet.Id);

            var response = await batchUpdateRequest.ExecuteAsync()
                ?? throw LogAndReturnException(new Exception($"Null response when trying to create cards"));

            //TODO CheckRepliesForExceptions(response.Replies);

            var updatedSpreadSheet = response.UpdatedSpreadsheet
                ?? throw LogAndReturnException(new Exception($"Null response when trying to create cards"));

            if (updatedSpreadSheet.Sheets.Count != numberOfCardsToCreate)
                Log.Warning("Mismatch between number of existing sheets in spreadsheet and cards to be created.");

            return CreateSpreadsheetModelFromGoogleSpreadSheet(updatedSpreadSheet);
        }

        private static IEnumerable<Request> GetRemoveAllSheetsRequests(SpreadSheet spreadSheet)
        {
            var requests = new List<Request>();

            foreach (var sheetId in spreadSheet.SheetIds)
                requests.Add(new Request
                {
                    DeleteSheet = new DeleteSheetRequest
                    {
                        SheetId = sheetId
                    }
                });

            return requests;
        }

        private static IEnumerable<Request> GetDuplicateTemplateSheetRequests(SpreadSheet spreadSheet, int numberOfCardsToCreate)
        {
            var requests = new List<Request>();

            for (int i = 0; i < numberOfCardsToCreate; i++)
                requests.Add(new Request
                {
                    DuplicateSheet = new DuplicateSheetRequest
                    {
                        NewSheetName = $"Card {i}",
                        SourceSheetId = spreadSheet.TemplateSheetId
                    }
                });

            return requests;
        }

        public async Task<bool> ReplacePlaceholderWithValues(SpreadSheet spreadSheet, string placeholderValue, List<MingelBingoCard> mingelBingoCards)
        {
            var valueRanges = new List<ValueRange>();

            for (int i = 0; i < spreadSheet.SheetIds.Count; i++)
                valueRanges.AddRange(await GetValueRangesForSheet(spreadSheet, mingelBingoCards, i, placeholderValue));

            var requestBody = new BatchUpdateValuesRequest
            {
                Data = valueRanges,
                ValueInputOption = "USER_ENTERED"
            };

            var response = await _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadSheet.Id).ExecuteAsync()
                ?? throw LogAndReturnException(new Exception($"Null response when trying to update placeholder values."));

            //TODO Check (response.Replies);

            return true;
        }

        //TODO Value range is same for all sheets. Makes most sense to find for one sheet only.
        private async Task<List<ValueRange>> GetValueRangesForSheet(SpreadSheet spreadSheet, List<MingelBingoCard> mingelBingoCards, int index, string placeholderValue)
        {
            var sheetName = spreadSheet.SheetNames[index];

            var sheetValues = await _sheetsService.Spreadsheets.Values.Get(spreadSheet.Id, sheetName).ExecuteAsync()
                ?? throw LogAndReturnException(
                    new Exception($"Null response when trying to get sheet values using sheet name: {sheetName} for sheet with ID {spreadSheet.SheetIds[index]}."));

            var foundCells = FindPlaceHolderCellsInSheetValues(sheetValues, placeholderValue);

            if (!CellsFoundInCollection(foundCells))
                throw LogAndReturnException(
                    new Exception($"Failed to find placeholder cells (with value {placeholderValue} in sheet: {sheetName}."));

            var valueRanges = ValueRangeCreator.CreateValueRangesForCells(foundCells, mingelBingoCards[index].Values, sheetName);

            return valueRanges;
        }

        private static bool CellsFoundInCollection(List<List<Cell>> foundCells)
        {
            if (foundCells == null || foundCells.Count == 0)
                return false;

            foreach (var row in foundCells)
                if (row.Any())
                    return true;

            return false;
        }

        private static List<List<Cell>> FindPlaceHolderCellsInSheetValues(ValueRange sheetValues, string placeholderValue)
        {
            var foundCells = new List<List<Cell>>();

            for (int rowIndex = 0; rowIndex < sheetValues.Values.Count; rowIndex++)
            {
                foundCells.Add(new List<Cell>());

                var rowValues = sheetValues.Values[rowIndex];

                for (int columnIndex = 0; columnIndex < rowValues.Count; columnIndex++)
                {
                    var cellValue = rowValues[columnIndex];

                    if (cellValue == null)
                        continue;

                    if (placeholderValue.Equals(cellValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundCells[rowIndex].Add(new Cell
                        {
                            RowIndex = rowIndex,
                            ColumnIndex = columnIndex
                        });
                    }
                }
            }

            foundCells.RemoveAll(r => !r.Any());

            return foundCells;
        }

        private static SpreadSheet CreateSpreadsheetModelFromGoogleSpreadSheet(Spreadsheet updatedSpreadSheet)
        {
            var exceptions = new List<Exception>();

            var sheetIds = new List<int>();
            var sheetNames = new List<string>();

            foreach (var sheet in updatedSpreadSheet.Sheets)
            {
                if (sheet.Properties.SheetId == null)
                {
                    exceptions.Add(new Exception($"SheetId null for sheet {sheet.Properties.Title}"));
                    continue;
                }

                sheetIds.Add(sheet.Properties.SheetId ?? 0);
                sheetNames.Add(sheet.Properties.Title);
            }

            if (exceptions.Count > 0)
                throw LogAndReturnException(
                    new AggregateException($"Error when gathering sheet IDs and Names from spreadsheet {updatedSpreadSheet.SpreadsheetId}", exceptions));

            return new SpreadSheet
            {
                Id = updatedSpreadSheet.SpreadsheetId,
                Name = updatedSpreadSheet.Properties.Title,
                SheetIds = sheetIds,
                SheetNames = sheetNames
            };
        }

        private static Exception LogAndReturnException(Exception exception)
        {
            Log.Error(exception, exception.Message);

            return exception;
        }
    }
}
