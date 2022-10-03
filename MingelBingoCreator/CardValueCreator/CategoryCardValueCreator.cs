using MingelBingoCreator.CardValueCreator.ValuesHandlerSelector;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator
{
    public class CategoryCardValueCreator : ICardValueCreator
    {
        private readonly AppSettings _appSettings;

        private readonly IValuesHandlerSelector _valuesHandlerSelector;

        private static readonly Random _random = new();

        public CategoryCardValueCreator(IConfigurationsReader configReader, IValuesHandlerSelector valuesHandlerSelector)
        {
            _appSettings = configReader.GetAppSettings();

            _valuesHandlerSelector = valuesHandlerSelector;
        }

        public List<CardValue> CreateCardValues(MingelBingoData data)
        {
            var valuesHandlers = _valuesHandlerSelector.GetValuesHandlers(data);

            var createdCards = new List<CardValue>();

            for (int i = 0; i < _appSettings.ExportOptions.NumberOfCards; i++)
            {
                var values = new List<string>();

                foreach (var valueHandler in valuesHandlers)
                    values.AddRange(valueHandler.GetValues(MaxValues: data.CellsInEachBoard - values.Count));

                if (values.Count != data.CellsInEachBoard)
                    throw new Exception($"Failed to collect the correct amount of values for card {i}. Only found {values.Count} of {data.CellsInEachBoard}");

                Shuffle(values);

                createdCards.Add(new CardValue(values));
            }

            return createdCards;
        }

        private static void Shuffle(List<string> list)
        {
            var n = list.Count;

            while (n > 1)
            {
                var k = _random.Next(n);

                n--;

                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
