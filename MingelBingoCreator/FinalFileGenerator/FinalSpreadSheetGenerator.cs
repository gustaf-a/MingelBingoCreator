using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;
using MingelBingoCreator.Repository;

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
            var result = _repository.CopyFile(_appSettings.GoogleSheetsOptions.TemplateSheetId, CreateNewFileName());

            throw new NotImplementedException();
            //Send board objects to copy of template sheet


            //remove template tab from template sheet

        }

        private string CreateNewFileName()
        {
            var dateSuffix = DateTime.Now.ToString("MM-dd_HH-mm");

            return $"{_appSettings.ExportOptions.FinalFileName}_{dateSuffix}";
        }
    }
}
