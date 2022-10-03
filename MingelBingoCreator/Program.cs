using MingelBingoCreator.CardValueCreator;
using MingelBingoCreator.CardValueCreator.ValuesHandlerSelector;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.DataGathering;
using MingelBingoCreator.FinalFileGenerator;
using MingelBingoCreator.Repository;
using Newtonsoft.Json;
using Serilog;

namespace MingelBingoCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //----- Setup -----
            SetupLogger();

            var appSettings = GetAppSettings();

            var repository = new GoogleSheetsRepository(appSettings);

            var dataGatherer = new GoogleSheetsDataGatherer(appSettings, repository);

            var cardValueCreator = new CategoryCardValueCreator(appSettings, new TaggedCategoriesValuesHandlerSelector());

            var finalFileGenerator = new FinalSpreadSheetGenerator(appSettings, repository);

            //----- Execution -----

            var mingelBingoData = dataGatherer.GatherData();
            Log.Information("Gathered data.");

            var cardValues = cardValueCreator.CreateCardValues(mingelBingoData);
            Log.Information("Values for cards created.");

            var finalFile = finalFileGenerator.CreateFinalFile(cardValues);
            Log.Information($"Successfully created final file: {finalFile.Name}");
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/mingelBingoCreator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static AppSettings GetAppSettings()
        {
            try
            {
                var rawFile = File.ReadAllText("appsettings.json");
                if (string.IsNullOrEmpty(rawFile))
                    throw new Exception("Failed to find or load appsettings.json file");

                var appSettings = JsonConvert.DeserializeObject<AppSettings>(rawFile);
                if (appSettings == null)
                    throw new Exception("Failed to deserialize file to AppSettings-object.");

                return appSettings;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to read appsettings. Please ensure appsettings.json is correct: {0}", e.Message);
                throw;
            }
        }
    }
}