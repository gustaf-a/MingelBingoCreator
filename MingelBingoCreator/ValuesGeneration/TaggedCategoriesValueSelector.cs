using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    /// <summary>
    /// Accepts tags on category headings in data document which controls how to add.
    /// </summary>
    internal class TaggedCategoriesValueSelector : IValueSelector
    {
        private readonly List<IValueSelector> _taggedValueSelectors;

        private readonly IValueSelector _fillerValueSelector;

        private readonly int _cellsToAddToEachBoard;

        private static readonly Random random = new();

        public TaggedCategoriesValueSelector(List<Category> categories)
        {
            _taggedValueSelectors = new List<IValueSelector>();

            var fillerCategories = new List<Category>();

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];

                if (IsCategory(TaggedCategoriesTags.OnEachBoard, category))
                {
                    var cellsToAddToEachBoard = GetArgument(category.Heading);

                    _taggedValueSelectors.Add(new RandomValueSelector(new List<Category> { category }, cellsToAddToEachBoard));

                    continue;
                }
                else if (IsCategory(TaggedCategoriesTags.UniquePerBoard, category))
                {
                    var cellsToAddToEachBoard = GetArgument(category.Heading);

                    _taggedValueSelectors.Add(new UniqueValueSelector(new List<Category> { category }, cellsToAddToEachBoard));

                    continue;
                }

                fillerCategories.Add(category);
            }

            _fillerValueSelector = new RandomValueSelector(fillerCategories);
        }

        public TaggedCategoriesValueSelector(List<Category> categories, int cellsToAddToEachBoard) : this(categories)
        {
            _cellsToAddToEachBoard = cellsToAddToEachBoard;
        }

        private static int GetArgument(string heading)
        {
            var splitHeading = heading.Split(TaggedCategoriesTags.BeforeTagsSymbol) ?? Array.Empty<string>();

            if (splitHeading.Length != 2)
                throw new Exception("Incorrect tag values. Don't put more than one # in category heading.");

            //TODO safety
            var tag = splitHeading[1];

            return int.Parse(tag.Split(TaggedCategoriesTags.ArgumentSplitCharacter)[1]);
        }

        private static bool IsCategory(string categoryKey, Category category)
            => category.Heading.ToLower().Contains($"{TaggedCategoriesTags.BeforeTagsSymbol}{categoryKey.ToLower()}");
       
        public List<string> GetValues()
        {
            if (_cellsToAddToEachBoard == 0)
                throw new Exception("Call to GetValues without argument requires values to return to be provided in construction of class.");

            return GetValues(_cellsToAddToEachBoard);
        }

        public List<string> GetValues(int numberOfValues)
        {
            var values = new List<string>();

            foreach (var valueSelector in _taggedValueSelectors)
                values.AddRange(valueSelector.GetValues());

            values.AddRange(_fillerValueSelector.GetValues(numberOfValues - values.Count));

            if (values.Count != numberOfValues)
                throw new Exception("Failed to collect the correct amount of values.");

            Shuffle(values);

            return values;
        }

        private static void Shuffle(List<string> list)
        {
            var n = list.Count;

            while (n > 1)
            {
                var k = random.Next(n);

                n--;

                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
