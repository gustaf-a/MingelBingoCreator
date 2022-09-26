using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    /// <summary>
    /// Randomly selects from all possible values independent of categories while avoiding duplicates if possible.
    /// </summary>
    internal class RandomValueSelector : SingleCategoryValueSelector
    {
        public RandomValueSelector(List<Category> categories)
            : base(categories) { }

        public RandomValueSelector(List<Category> categories, int cellsToAddToEachBoard)
            : base(categories, cellsToAddToEachBoard) { }

        public override List<string> GetValues(int numberOfValues)
        {
            var selectedValues = new List<string>();

            for (int i = 0; i < numberOfValues; i++)
            {
                selectedValues.Add(_values[GetNextRandomIndex()]);

                if (_indicesSelected.Count >= _values.Count)
                    _indicesSelected = new List<int>();
            }

            return selectedValues;
        }
    }
}
