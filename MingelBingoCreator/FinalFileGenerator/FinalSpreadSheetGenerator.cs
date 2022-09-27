using Google.Apis.Sheets.v4.Data;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository;
using MingelBingoCreator.Repository.GoogleSheetsHelpers;
using Serilog;

namespace MingelBingoCreator.FinalFileGenerator
{
    internal class FinalSpreadSheetGenerator
    {
        private readonly AppSettings _appSettings;
        private readonly GoogleSheetsRepository _repository;

        public FinalSpreadSheetGenerator(AppSettings appSettings, GoogleSheetsRepository repository)
        {
            _appSettings = appSettings;
            _repository = repository;
        }

        internal SpreadSheet CreateFinalFile(List<MingelBingoCard> mingelBingoCards)
        {
            try
            {
                var createFinalFileTask = Task.Run(async () => await CreateFinalFileAsync(mingelBingoCards));

                createFinalFileTask.Wait();

                return createFinalFileTask.Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception encountered when creating final file: {ex.Message}");

                throw;
            }
        }

        private async Task<SpreadSheet> CreateFinalFileAsync(List<MingelBingoCard> mingelBingoCards)
        {
            var finalFileUnprocessed = await _repository.CopyFile(_appSettings.GoogleSheetsOptions.TemplateSheetId, GetNewFileName());

            if (finalFileUnprocessed == null)
                throw new Exception("Failed to copy template file with sheets. Null result returned from repository.");


            var finalFileWithTabs = await _repository.CreateDuplicateSheetTabsFromTemplateSheetTab(finalFileUnprocessed, mingelBingoCards.Count);
            
            var result = await _repository.ReplacePlaceholderWithValues(finalFileWithTabs, _appSettings.GoogleSheetsOptions.PlaceHolderValue, mingelBingoCards);

            if (result)
                Log.Information("Updated placeholder values in final file successfully");
            else
                Log.Warning("Something went wrong when updating placeholder values. Please check final file manually.");

            return finalFileWithTabs;
        }

        private string GetNewFileName()
        {
            var dateSuffix = DateTime.Now.ToString("MMdd_HHmm");

            return $"{_appSettings.ExportOptions.FinalFileName}_{dateSuffix}";
        }
    }
}
