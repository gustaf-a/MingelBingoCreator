using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal abstract class SingleCategoryValueSelector : IValueSelector
    {
        protected readonly Random _random;

        protected readonly List<string> _values;

        protected readonly int _cellsToAddToEachBoard;

        protected List<int> _indicesSelected;

        protected SingleCategoryValueSelector(List<Category> categories)
        {
            _random = new Random();

            _indicesSelected = new();

            _values = new();

            foreach (var category in categories)
                _values.AddRange(category.Values);

            if (_values.Count == 0)
                throw new Exception("No values to select from found.");
        }

        protected SingleCategoryValueSelector(List<Category> categories, int cellsToAddToEachBoard) : this(categories)
        {
            _cellsToAddToEachBoard = cellsToAddToEachBoard;
        }

        public List<string> GetValues()
        {
            if (_cellsToAddToEachBoard == 0)
                throw new Exception("Call to GetValues without argument requires values to return to be provided in construction of class.");

            return GetValues(_cellsToAddToEachBoard);
        }

        public abstract List<string> GetValues(int numberOfValues);

        protected int GetNextRandomIndex()
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
