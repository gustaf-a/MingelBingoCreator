using MingelBingoCreator.Data;
namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    internal static class CategoryTags
    {
        public static List<CategoryTag> AllTags
            => new()
            {
                OnEachBoard,
                UniquePerBoard,
                Ignore
            };

        public static readonly CategoryTag OnEachBoard = new()
        {
            Tag = "OnEachBoard",
            HasArguments = true
        };

        public static readonly CategoryTag UniquePerBoard = new()
        {
            Tag = "UniquePerBoard",
            HasArguments = true
        };

        public static readonly CategoryTag Ignore = new()
        {
            Tag = "Ignore",
            HasArguments = false
        };
    }
}
