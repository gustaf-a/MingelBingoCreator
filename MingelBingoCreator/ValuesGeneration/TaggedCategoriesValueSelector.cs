using MingelBingoCreator.Data;
using Serilog;

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
                if (TryGetValueSelectorForCategory(categories[i], out var valueSelector))
                    _taggedValueSelectors.Add(valueSelector);
                else
                    fillerCategories.Add(categories[i]);
            }

            _fillerValueSelector = new RandomValueSelector(fillerCategories);
        }

        public TaggedCategoriesValueSelector(List<Category> categories, int cellsToAddToEachBoard) : this(categories)
        {
            _cellsToAddToEachBoard = cellsToAddToEachBoard;
        }

        private static bool TryGetValueSelectorForCategory(Category category, out IValueSelector? valueSelector)
        {
            valueSelector = null;

            if (!TryFindTaggedCategory(category, out var taggedCategory))
                return false;

            switch (taggedCategory.Category)
            {
                case TaggedCategoriesTags.Tags.OnEachBoard:
                    valueSelector = new RandomValueSelector(new List<Category> { category }, taggedCategory.Argument);
                    return true;

                case TaggedCategoriesTags.Tags.UniquePerBoard:
                    valueSelector = new UniqueValueSelector(new List<Category> { category }, taggedCategory.Argument);
                    return true;

                case TaggedCategoriesTags.Tags.None:
                    //Let fall through to default
                default:
                    Log.Warning($"Failed to parse category from heading. Ignoring heading tags for: {category.Heading}");
                    return false;
            }
        }

        private static bool TryFindTaggedCategory(Category category, out TaggedCategory taggedCategory)
        {
            taggedCategory = null;

            if (!category.Heading.Contains(TaggedCategoriesTags.BeforeTagsSymbol))
                return false;

            var splitHeading = category.Heading.Split(TaggedCategoriesTags.BeforeTagsSymbol);

            var tagPartOfHeading = splitHeading[1];
            if (string.IsNullOrWhiteSpace(tagPartOfHeading))
                return false;

            if (!TryGetArgument(tagPartOfHeading, out var foundArgument))
            {
                Log.Error($"Failed to find argument for tagged category: {tagPartOfHeading}. Ignoring category tag for values.");
                return false;
            }

            var categoryTag = GetCategoryTag(tagPartOfHeading);

            taggedCategory = new TaggedCategory(category.Heading, category.Values, categoryTag, foundArgument);

            return true;
        }

        private static TaggedCategoriesTags.Tags GetCategoryTag(string tagPartOfHeading)
        {
            if (ContainsTag(tagPartOfHeading, TaggedCategoriesTags.Tags.UniquePerBoard))
                return TaggedCategoriesTags.Tags.UniquePerBoard;

            else if (ContainsTag(tagPartOfHeading, TaggedCategoriesTags.Tags.OnEachBoard))
                return TaggedCategoriesTags.Tags.OnEachBoard;

            else
                return TaggedCategoriesTags.Tags.None;
        }

        private static bool ContainsTag(string stringValue, TaggedCategoriesTags.Tags tag)
            => stringValue.ToLower().Contains(tag.ToString(), StringComparison.InvariantCultureIgnoreCase);

        private static bool TryGetArgument(string tagPartOfHeading, out int argument)
        {
            argument = 0;

            var splitTagPartOfHeading = tagPartOfHeading.Split(TaggedCategoriesTags.ArgumentSplitCharacter);

            if (splitTagPartOfHeading.Length != 2)
            {
                Log.Warning($"Incorrect tag values. Don't put more than one # in category heading: '{tagPartOfHeading}'");
                return false;
            }

            if (!int.TryParse(splitTagPartOfHeading[1], out argument))
            {
                Log.Warning($"Failed to parse argument part of category tag: {splitTagPartOfHeading[1]} ");
                return false;
            }

            return true;
        }

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
