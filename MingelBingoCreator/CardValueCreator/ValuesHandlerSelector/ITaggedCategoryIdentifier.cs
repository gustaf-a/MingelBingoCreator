using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    internal interface ITaggedCategoryIdentifier
    {
        public bool IsAnyTaggedCategory(string categoryHeading);

        bool TryGetTaggedCategory(DataCategory dataCategory, CategoryTag categoryTag, out TaggedCategory taggedCategory);
    }
}
