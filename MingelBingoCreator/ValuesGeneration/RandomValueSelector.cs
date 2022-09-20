using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    /// <summary>
    /// Randomly selects from all possible values independent of categories while avoiding duplicates if possible.
    /// </summary>
    internal class RandomValueSelector : IValueSelector
    {
        private Random _random;

        private int _valuesPerSelection;

        private readonly List<string> _values;

        private List<int> _indicesSelected;

        public RandomValueSelector(MingelBingoData mingelBingoData)
        {
            _random = new Random();

            _valuesPerSelection = mingelBingoData.CellsInEachBoard;

            _values = new List<string>();

            _indicesSelected = new List<int>();

            foreach (var category in mingelBingoData.RawDataCategories)
                _values.AddRange(category.Values);

            if (_values.Count == 0)
                throw new Exception("No values to select from found.");
        }

        public List<string> GetValues()
        {
            var selectedValues = new List<string>();

            for (int i = 0; i < _valuesPerSelection; i++)
                selectedValues.Add(_values[GetNextRandomIndex()]);

            return selectedValues;
        }

        private int GetNextRandomIndex()
        {
            int nextIndex;

            do
            {
                nextIndex = _random.Next(_values.Count);

            } while (_indicesSelected.Contains(nextIndex));

            _indicesSelected.Add(nextIndex);

            if (_indicesSelected.Count >= _values.Count)
                _indicesSelected = new List<int>();

            return nextIndex;
        }
    }
}
