using MingelBingoCreator.ValuesGeneration;

namespace MingelBingoCreator.Data
{
    internal class TaggedCategory : Category
    {
        public TaggedCategoriesTags.Tags Category;
        public int Argument;

        public TaggedCategory(string heading, List<string> values) : base(heading, values)
        {
        }

        public TaggedCategory(string heading, List<string> values, TaggedCategoriesTags.Tags category, int argument) : base(heading, values)
        {
            Category = category;

            Argument = argument;
        }
    }
}
