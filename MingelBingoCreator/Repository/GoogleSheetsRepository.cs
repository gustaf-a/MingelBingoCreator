using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository.GoogleSheetsHelpers;
using Serilog;

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

        public async Task<List<DataCategory>> GetColumns(string sheetId, string sheetName)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(sheetId, sheetName);

            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;

            var response = await request.ExecuteAsync();

            var result = new List<DataCategory>();

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

                result.Add(new DataCategory(heading, values));
            }

            return result;
        }

        public async Task<TemplateSpreadSheet> CopyFile(string originalSpreadsheetId, string newName)
        {
            var sheetIdToCopy = await GetSheetIdToCopy(originalSpreadsheetId);

            var createdSpreadSheet = await CreateNewSpreadSheet(newName);

            var copiedSheetId = await CopySheetToSpreadSheet(originalSpreadsheetId, sheetIdToCopy, createdSpreadSheet.SpreadsheetId);

            Log.Information($"Successfully created {newName} in Google Drive root folder by copying {originalSpreadsheetId}.");

            var sheetIds = new List<int>
            {
                copiedSheetId
            };

            foreach (var sheet in createdSpreadSheet.Sheets)
                sheetIds.Add(sheet.Properties.SheetId
                    ?? throw LogAndReturnException(new Exception($"Null sheetId found in created spreadsheet: {createdSpreadSheet.SpreadsheetId}")));

            return new TemplateSpreadSheet
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

        internal async Task<SpreadSheet> CreateDuplicateSheetTabsFromTemplateSheetTab(TemplateSpreadSheet spreadSheet, int numberOfCardsToCreate)
        {
            var requests = new List<Request>();

            requests.AddRange(GetDuplicateTemplateSheetRequests(spreadSheet, numberOfCardsToCreate));

            //Removes the template sheet and the automatically created Sheet1 which were in the original spreadsheet object.
            requests.AddRange(GetRemoveAllSheetsRequests(spreadSheet));

            var batchUpdateRequestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests,
                IncludeSpreadsheetInResponse = true
            };

            var batchUpdateRequest = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequestBody, spreadSheet.Id);

            var batchUpdateResponse = await batchUpdateRequest.ExecuteAsync()
                ?? throw LogAndReturnException(new Exception($"Null response when trying to create cards"));

            var updatedSpreadSheet = batchUpdateResponse.UpdatedSpreadsheet
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

        private static IEnumerable<Request> GetDuplicateTemplateSheetRequests(TemplateSpreadSheet spreadSheet, int numberOfCardsToCreate)
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

        public async Task<bool> ReplacePlaceholderWithValues(SpreadSheet spreadSheet, string placeholderValue, List<CardValue> mingelBingoCards)
        {
            await UpdateBingoCardsWithRangesFromFirstSheet(spreadSheet, mingelBingoCards, placeholderValue);

            var valueRanges = new List<ValueRange>();

            for (int i = 0; i < spreadSheet.SheetIds.Count; i++)
                valueRanges.AddRange(ValueRangeCreator.CreateValueRangesForCard(mingelBingoCards[i].A1NotationRanges, mingelBingoCards[i].Values, spreadSheet.SheetNames[i]));

            var requestBody = new BatchUpdateValuesRequest
            {
                Data = valueRanges,
                ValueInputOption = "USER_ENTERED"
            };

            var batchUpdateResponse = await _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadSheet.Id).ExecuteAsync()
                ?? throw LogAndReturnException(new Exception($"Null response when trying to update placeholder values."));

            if (batchUpdateResponse.TotalUpdatedSheets != spreadSheet.SheetIds.Count)
                Log.Warning("Total count of updated sheets is different from expected. Please check tabs in final file.");

            return true;
        }

        private async Task UpdateBingoCardsWithRangesFromFirstSheet(SpreadSheet finalFile, List<CardValue> mingelBingoCards, string placeHolderValue)
        {
            var cells = await GetA1NotationsForSheet(finalFile.Id, finalFile.SheetNames.First(), placeHolderValue);

            var valueRanges = A1NotationCreator.GetA1NotationsForCells(cells);

            foreach (var card in mingelBingoCards)
                card.A1NotationRanges.AddRange(valueRanges);
        }

        private async Task<List<List<Cell>>> GetA1NotationsForSheet(string spreadsheetId, string sheetName, string placeholderValue)
        {
            var sheetValues = await _sheetsService.Spreadsheets.Values.Get(spreadsheetId, sheetName).ExecuteAsync()
                ?? throw LogAndReturnException(
                    new Exception($"Null response when trying to get sheet values using sheet name: {sheetName} for sheet with ID {spreadsheetId}."));

            var foundCells = FindPlaceHolderCellsInSheetValues(sheetValues, placeholderValue);

            if (!CellsFoundInCollection(foundCells))
                throw LogAndReturnException(
                    new Exception($"Failed to find placeholder cells (with value {placeholderValue} in sheet: {sheetName}."));

            return foundCells;
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
