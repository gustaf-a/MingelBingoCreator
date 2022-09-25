using MingelBingoCreator.Configurations;
using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class MingelBingoValuesGenerator
    {
        private ValueSelectorFactory _valueSelectorFactory;

        private IValueSelector _valueSelector;

        public MingelBingoValuesGenerator(ValueSelectorFactory.SelectorMethod selectorMethod)
        {
            _valueSelectorFactory = new ValueSelectorFactory(selectorMethod);
        }

        internal List<MingelBingoCard> GetValues(AppSettings appSettings, MingelBingoData mingelBingoData)
        {
            if (_valueSelector == null)
                _valueSelector = _valueSelectorFactory.Build(mingelBingoData.RawDataCategories);

            var cards = new List<MingelBingoCard>();

            for (int i = 0; i < appSettings.ExportOptions.NumberOfCards; i++)
                cards.Add(new MingelBingoCard(_valueSelector.GetValues(mingelBingoData.CellsInEachBoard)));

            return cards;
        }
    }
}
