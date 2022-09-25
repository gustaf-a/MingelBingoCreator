using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    /// <summary>
    /// Randomly selects from all possible values independent of categories while avoiding duplicates if possible.
    /// </summary>
    internal class RandomValueSelector : IValueSelector
    {
        private Random _random;

        private readonly List<string> _values;

        private List<int> _indicesSelected;

        private readonly int _cellsToAddToEachBoard;

        public RandomValueSelector(List<Category> categories)
        {
            _random = new Random();

            _values = new List<string>();

            _indicesSelected = new List<int>();

            foreach (var category in categories)
                _values.AddRange(category.Values);

            if (_values.Count == 0)
                throw new Exception("No values to select from found.");
        }

        public RandomValueSelector(List<Category> categories, int cellsToAddToEachBoard) : this(categories)
        {
            _cellsToAddToEachBoard = cellsToAddToEachBoard;
        }

        public List<string> GetValues()
        {
            if (_cellsToAddToEachBoard == 0)
                throw new Exception("Call to GetValues without argument requires values to return to be provided in construction of class.");
            
            return GetValues(_cellsToAddToEachBoard);
        }

        public List<string> GetValues(int numberOfValues)
        {
            var selectedValues = new List<string>();

            for (int i = 0; i < numberOfValues; i++)
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
