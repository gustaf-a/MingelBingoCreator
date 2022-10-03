using MingelBingoCreator.CardValueCreator;
using MingelBingoCreator.DataGathering;
using MingelBingoCreator.FinalFileGenerator;
using Serilog;

namespace MingelBingoCreator
{
    public class ProgramStart
    {
        private ICardValueCreator _cardValueCreator;
        private IFinalFileCreator _finalFileCreator;
        private IDataGatherer _dataGatherer;

        public ProgramStart(
            ICardValueCreator cardValueCreator, 
            IFinalFileCreator finalFileCreator, 
            IDataGatherer dataGatherer)
        {
            _cardValueCreator = cardValueCreator;
            _finalFileCreator = finalFileCreator;
            _dataGatherer = dataGatherer;
        }

        public void Execute()
        {
            var mingelBingoData = _dataGatherer.GatherData();
            Log.Information("Gathered data.");

            var cardValues = _cardValueCreator.CreateCardValues(mingelBingoData);
            Log.Information("Values for cards created.");

            var finalFile = _finalFileCreator.CreateFinalFile(cardValues);
            Log.Information($"Successfully created final file: {finalFile.Name}");
        }
    }
}
