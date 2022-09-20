using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.MingelBingoCreator;
using MingelBingoCreator.Repository;
using MingelBingoCreator.ValueSelector;
using Newtonsoft.Json;
using Serilog;

namespace MingelBingoCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/mingelBingoCreator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var appSettings = GetAppSettings();

            var sheetsOptions = appSettings.GoogleSheetsOptions;

            var repository = new GoogleSheetsRepository(appSettings);

            //extract to separate class
            try
            {
                var taskGetData = repository.GetColumns(sheetsOptions.DataSheetId, sheetsOptions.DataSheetTabName);

                var taskCountPlaceHolderCells = repository.CountCellsWithValue(sheetsOptions.TemplateSheetId, sheetsOptions.TemplateSheetTabName, sheetsOptions.PlaceHolderValue);

                Task.WaitAll(taskGetData, taskCountPlaceHolderCells);

                var data = taskGetData.Result;

                var placeHolderCellsCount = taskCountPlaceHolderCells.Result;

                var mingelBingoData = new MingelBingoData(data, placeHolderCellsCount);

                var valueSelector = new RandomValueSelector(mingelBingoData);

                var mingelBingoGenerator = new MingelBingoGenerator(valueSelector);

                //Create cards
                var mingelBingoCards = mingelBingoGenerator.GetBingoCards(appSettings.ExportOptions.NumberOfCards);

                //copy template sheet


                //Send board objects to copy of template sheet


                //remove template tab from template sheet
                
            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.InnerExceptions)
                {
                    Log.Error("Failed to get data from Google Sheets API: {0}", exception.Message);
                }

                throw;
            }

            //läs in rader

            //läs antal 
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