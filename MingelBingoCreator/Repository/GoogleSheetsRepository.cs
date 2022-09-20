using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
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

            var sheetValues = response.Values;

            var counter = 0;

            foreach (var row in sheetValues)
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

        public Task ReplacePlaceholderWithValues(string sheetId, string sheetName, string placeholderValue, List<string> values)
        {
            throw new NotImplementedException();

            //var query = new FindReplaceRequest
        }
    }
}
