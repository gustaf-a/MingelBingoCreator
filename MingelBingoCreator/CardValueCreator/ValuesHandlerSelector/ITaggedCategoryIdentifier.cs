using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    public interface ITaggedCategoryIdentifier
    {
        public bool IsAnyTaggedCategory(string categoryHeading);

        bool TryGetTaggedCategory(DataCategory dataCategory, CategoryTag categoryTag, out TaggedCategory taggedCategory);
    }
}
