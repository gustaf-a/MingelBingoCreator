using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class UniqueValueSelector : IValueSelector
    {
        private readonly Random _random;

        private readonly List<string> _values;

        private readonly int _cellsToAddToEachBoard;

        private List<int> _indicesSelected;

        public UniqueValueSelector(List<Category> categories)
        {
            _random = new Random();

            _indicesSelected = new();

            _values = new();

            foreach (var category in categories)
                _values.AddRange(category.Values);

            if (_values.Count == 0)
                throw new Exception("No values to select from found.");
        }

        public UniqueValueSelector(List<Category> categories, int cellsToAddToEachBoard) : this(categories)
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
            {
                if (_indicesSelected.Count >= _values.Count)
                    return selectedValues;

                selectedValues.Add(_values[GetNextRandomIndex()]);
            }

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

            return nextIndex;
        }
    }
}
