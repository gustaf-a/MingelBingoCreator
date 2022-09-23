using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    /// <summary>
    /// Accepts tags on headings in data document.
    /// For example:
    /// heading text #OnEachBoard_1
    /// This ensure exactly one from this category is put on each card.
    /// </summary>
    internal class TaggedCategoriesValueSelector : IValueSelector
    {
        private List<IValueSelector> _valueSelectors;

        private int _valuesPerSelection;

        private int _valuesFromTaggedCategories;

        public TaggedCategoriesValueSelector(int cellsToSelect, List<Category> categories)
        {
            _valueSelectors = new List<IValueSelector>();

            _valuesPerSelection = cellsToSelect;

            _valuesFromTaggedCategories = 0;

            var categoriesToSelectRandomly = new List<Category>();

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];

                if (IsCategory("OnEachBoard", category))
                {
                    var cellsToAddToEachBoard = GetArgument(category.Heading);

                    _valueSelectors.Add(new RandomValueSelector(cellsToAddToEachBoard, new List<Category> { category }));

                    _valuesFromTaggedCategories += cellsToAddToEachBoard;

                    continue;
                }

                //TODO Implement unique tag if (HasUniqueTag())

                categoriesToSelectRandomly.Add(category);
            }

            _valueSelectors.Add(new RandomValueSelector(_valuesPerSelection - _valuesFromTaggedCategories, categoriesToSelectRandomly));
        }

        private int GetArgument(string heading)
        {
            var splitHeading = heading.Split("#") ?? new string[0];

            if (splitHeading.Length != 2)
                throw new Exception("Incorrect tag values. Don't put more than one # in category heading.");

            //TODO safety
            var tag = splitHeading[1];

            return int.Parse(tag.Split("_")[1]);
        }

        private static bool IsCategory(string categoryKey, Category category)
            => category.Heading.ToLower().Contains(categoryKey.ToLower());

        public List<string> GetValues()
        {
            var values = new List<string>();

            foreach (var valueSelector in _valueSelectors)
                values.AddRange(valueSelector.GetValues());

            if (values.Count != _valuesPerSelection)
                throw new Exception("Failed to collect the correct amount of values.");

            Shuffle(values);

            return values;
        }

        private static Random random = new Random();

        private static void Shuffle(List<string> list)
        {
            var n = list.Count;

            while (n > 1)
            {
                var k = random.Next(n);

                n--;

                var value = list[k];

                list[k] = list[n];

                list[n] = value;
            }
        }
    }
}
