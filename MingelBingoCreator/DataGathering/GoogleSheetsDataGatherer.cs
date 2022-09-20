using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository;
using Serilog;

namespace MingelBingoCreator.DataGathering
{
    internal class GoogleSheetsDataGatherer
    {
        private AppSettings _appSettings;
        private GoogleSheetsRepository _repository;

        public GoogleSheetsDataGatherer(AppSettings appSettings, GoogleSheetsRepository repository)
        {
            _appSettings = appSettings;
            _repository = repository;
        }

        internal MingelBingoData GatherData()
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
