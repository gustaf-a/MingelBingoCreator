using MingelBingoCreator.CardValueCreator.ValuesHandler;
using MingelBingoCreator.Data;
using Serilog;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    public class TaggedCategoriesValuesHandlerSelector : IValuesHandlerSelector
    {
        private readonly ITaggedCategoryIdentifier _taggedCategoryIdentifier;

        public TaggedCategoriesValuesHandlerSelector(ITaggedCategoryIdentifier taggedCategoryIdentifier)
        {
            _taggedCategoryIdentifier = taggedCategoryIdentifier;
        }

        public List<IValuesHandler> GetValuesHandlers(MingelBingoData data)
        {
            var fillerCategories = new List<DataCategory>();

            var valuesSelectors = new List<IValuesHandler>();

            foreach (var dataCategory in data.RawDataCategories)
            {
                if (TryGetValuesSelectorForTaggedCategory(dataCategory, out var valueSelector))
                    valuesSelectors.Add(valueSelector);
                else
                    fillerCategories.Add(dataCategory);
            }

            var valuesHandlerBuilder = new ValuesHandlerBuilder();

            TaggedCategoriesValuesHandlerDirector.ConstructFillerValuesHandler(valuesHandlerBuilder, fillerCategories);

            valuesSelectors.Add(valuesHandlerBuilder.GetResult());

            return valuesSelectors;
        }

        private bool TryGetValuesSelectorForTaggedCategory(DataCategory dataCategory, out IValuesHandler valueSelector)
        {
            valueSelector = null;

            if (!_taggedCategoryIdentifier.IsAnyTaggedCategory(dataCategory.Heading))
                return false;

            foreach (var categoryTag in CategoryTags.AllTags)
            {
                if(_taggedCategoryIdentifier.TryGetTaggedCategory(dataCategory, categoryTag, out var taggedCategory))
                {
                    var valuesHandlerBuilder = new ValuesHandlerBuilder();

                    TaggedCategoriesValuesHandlerDirector.ConstructCategory(valuesHandlerBuilder, taggedCategory);

                    valueSelector = valuesHandlerBuilder.GetResult();

                    return true;
                }
            }

            Log.Error($"Invalid tag in heading: {dataCategory.Heading}");

            return false;
        }            
    }
}
