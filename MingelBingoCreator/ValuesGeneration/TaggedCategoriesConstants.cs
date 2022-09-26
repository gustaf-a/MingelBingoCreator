
namespace MingelBingoCreator.ValuesGeneration
{
    public static class TaggedCategoriesTags
    {
        public const string BeforeTagsSymbol = "#";

        public const char ArgumentSplitCharacter = '_';

        public enum Tags
        {
            None = 0,
            OnEachBoard = 1,
            UniquePerBoard = 2,
            Ignore = 4,

            NeedsArguments = OnEachBoard | UniquePerBoard
        }
    }
}
