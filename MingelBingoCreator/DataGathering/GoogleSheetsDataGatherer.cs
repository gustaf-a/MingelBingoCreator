using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository;
using Serilog;

namespace MingelBingoCreator.DataGathering
{
    public class GoogleSheetsDataGatherer : IDataGatherer
    {
        private AppSettings _appSettings;
        private IRepository _repository;

        public GoogleSheetsDataGatherer(IConfigurationsReader configReader, IRepository repository)
        {
            _appSettings = configReader.GetAppSettings();
            _repository = repository;
        }

        public MingelBingoData GatherData()
        {
            var sheetsOptions = _appSettings.GoogleSheetsOptions;
            
            try
            {
                var taskGetData = _repository.GetColumns(sheetsOptions.DataSheetId, sheetsOptions.DataSheetTabName);
                
                var taskCountPlaceHolderCells = _repository.CountCellsWithValue(sheetsOptions.TemplateSheetId, sheetsOptions.TemplateSheetTabName, sheetsOptions.PlaceHolderValue);
                
                Task.WaitAll(taskGetData, taskCountPlaceHolderCells);

                var data = taskGetData.Result;
                var placeHolderCellsCount = taskCountPlaceHolderCells.Result;

                return new MingelBingoData(data, placeHolderCellsCount);
            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.InnerExceptions)
                    Log.Error(exception, $"Failure when working with external API: {0}", exception.Message);

                throw;
            }
        }
    }
}
