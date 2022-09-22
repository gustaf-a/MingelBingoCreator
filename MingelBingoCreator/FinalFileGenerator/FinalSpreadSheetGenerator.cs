using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository;
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
            var newSpreadSheet = await _repository.CopyFile(_appSettings.GoogleSheetsOptions.TemplateSheetId, CreateNewFileName());

            if (newSpreadSheet == null)
                throw new Exception("Failed to copy template file with sheets. Null result returned from repository.");

            var templateSpreadSheet = await _repository.CreateMingelBingoCardsFromTemplateSheet(newSpreadSheet, mingelBingoCards.Count);

            var result = await _repository.ReplacePlaceholderWithValues(templateSpreadSheet, _appSettings.GoogleSheetsOptions.PlaceHolderValue, mingelBingoCards);

            //TODO Handle result

            return templateSpreadSheet;
        }

        private string CreateNewFileName()
        {
            var dateSuffix = DateTime.Now.ToString("MMdd_HHmm");

            return $"{_appSettings.ExportOptions.FinalFileName}_{dateSuffix}";
        }
    }
}
