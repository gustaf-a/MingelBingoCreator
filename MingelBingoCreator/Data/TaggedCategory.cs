namespace MingelBingoCreator.Data
{
    internal class TaggedCategory : DataCategory
    {
        public CategoryTag Category;

        public bool HasArgument;
        public int Argument;

        public TaggedCategory(string heading, List<string> values) : base(heading, values)
        {

        }

        public TaggedCategory(string heading, List<string> values, CategoryTag category) : base(heading, values)
        {
            Category = category;
        }
    }
}
