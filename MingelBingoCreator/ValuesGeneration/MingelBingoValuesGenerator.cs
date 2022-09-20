using MingelBingoCreator.Data;
using MingelBingoCreator.ValuesGeneration;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class MingelBingoValuesGenerator
    {
        private IValueSelector _valueSelector;

        public MingelBingoValuesGenerator(IValueSelector valueSelector)
        {
            _valueSelector = valueSelector;
        }

        internal List<MingelBingoCard> GetValues(int numberOfCardsToCreate)
        {
            var cards = new List<MingelBingoCard>();

            for (int i = 0; i < numberOfCardsToCreate; i++)
                cards.Add(new MingelBingoCard(_valueSelector.GetValues()));

            return cards;
        }
    }
}
