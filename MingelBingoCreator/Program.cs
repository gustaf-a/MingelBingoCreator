using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.DataGathering;
using MingelBingoCreator.FinalFileGenerator;
using MingelBingoCreator.Repository;
using MingelBingoCreator.ValuesGeneration;
using Newtonsoft.Json;
using Serilog;
using System;

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

            var cardsCreator = new MingelBingoValuesGenerator(ValueSelectorFactory.SelectorMethod.Random);

            var finalFileGenerator = new FinalSpreadSheetGenerator(appSettings, repository);

            //----- Execution -----

            //Gathering Data from data files and template files
            var mingelBingoData = dataGatherer.GatherData();

            //Creating list of values from the data based on template files and AppSettings
            var mingelBingoCards = cardsCreator.GetValues(appSettings, mingelBingoData);

            //Creating a new file from template file and populating it with previously created values
            var finalFile = finalFileGenerator.CreateFinalFile(mingelBingoCards);

            //----- Closure -----
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

                return JsonConvert.DeserializeObject<AppSettings>(rawFile);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to read appsettings. Please ensure appsettings.json is correct: {0}", e.Message);
                throw;
            }
        }
    }
}