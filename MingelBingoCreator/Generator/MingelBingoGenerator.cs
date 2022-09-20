using MingelBingoCreator.Data;
using MingelBingoCreator.ValueSelector;

namespace MingelBingoCreator.MingelBingoCreator
{
    internal class MingelBingoGenerator
    {
        private IValueSelector _valueSelector;

        public MingelBingoGenerator(IValueSelector valueSelector)
        {
            _valueSelector = valueSelector;
        }

        internal List<MingelBingoCard> GetBingoCards(int numberOfCardsToCreate)
        {
            var cards = new List<MingelBingoCard>();

            for (int i = 0; i < numberOfCardsToCreate; i++)
                cards.Add(new MingelBingoCard(_valueSelector.GetValues()));

            return cards;
        }
    }
}
