using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class UniqueValueSelector : SingleCategoryValueSelector
    {
        public UniqueValueSelector(List<Category> categories)
            : base(categories) { }

        public UniqueValueSelector(List<Category> categories, int cellsToAddToEachBoard)
            : base(categories, cellsToAddToEachBoard) { }

        public override List<string> GetValues(int numberOfValues)
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
    }
}
