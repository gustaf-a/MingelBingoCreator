using MingelBingoCreator.CardValueCreator.SelectorBehaviors;
using MingelBingoCreator.CardValueCreator.ValuesHandler;
using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    internal static class TaggedCategoriesValuesHandlerDirector
    {
        private static readonly Dictionary<string, ISelectorBehavior> SelectorBehaviors
            = new()
            {
                { CategoryTags.OnEachBoard.Tag, new RepeatingValuesSelectorBehavior()},
                { CategoryTags.UniquePerBoard.Tag, new UniqueValuesSelectorBehavior()},
                { CategoryTags.Ignore.Tag, new NoValuesSelectorBehavior()}
            };

        public static void ConstructFillerValuesHandler(ValuesHandlerBuilder valuesHandlerBuilder, List<DataCategory> fillerCategories)
        {
            var values = new List<string>();

            foreach (var fillerCategory in fillerCategories)
                values.AddRange(fillerCategory.Values);

            SetValues(valuesHandlerBuilder, values);

            SetSelectorBehavior(valuesHandlerBuilder, new RepeatingValuesSelectorBehavior());
        }

        public static void ConstructCategory(ValuesHandlerBuilder valuesHandlerBuilder, TaggedCategory taggedCategory)
        {
            SetArgument(valuesHandlerBuilder, taggedCategory);

            SetValues(valuesHandlerBuilder, taggedCategory.Values);

            SetSelectorBehavior(valuesHandlerBuilder, taggedCategory.Category.Tag);

            return;
        }

        private static void SetArgument(ValuesHandlerBuilder valuesHandlerBuilder, TaggedCategory taggedCategory)
        {
            if (taggedCategory.Category.HasArguments == false)
                return;

            valuesHandlerBuilder.SetArgument(taggedCategory.Argument);
        }

        private static void SetValues(ValuesHandlerBuilder valuesHandlerBuilder, List<string> values)
            => valuesHandlerBuilder.SetValues(values);

        private static void SetSelectorBehavior(ValuesHandlerBuilder valuesHandlerBuilder, string tag)
        {
            if (!SelectorBehaviors.ContainsKey(tag))
                throw new NotImplementedException($"CategoryTag not implemented in director: {tag}");

            SetSelectorBehavior(valuesHandlerBuilder, SelectorBehaviors[tag]);
        }

        private static void SetSelectorBehavior(ValuesHandlerBuilder valuesHandlerBuilder, ISelectorBehavior selectorBehavior)
            => valuesHandlerBuilder.SetSelectorBehavior(selectorBehavior);
    }
}
