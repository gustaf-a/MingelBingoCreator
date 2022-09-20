using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;

namespace MingelBingoCreator.Configurations
{
    public sealed class Credentials
    {
        public GoogleCredential GoogleCredential { get; set; }

        private static readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };

        public Credentials(AppSettings appSettings)
        {
            using (var stream = new FileStream(appSettings.GoogleSheetsOptions.CredentialsFileName, FileMode.Open, FileAccess.Read))
            {
                GoogleCredential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }
        }
    }
}
