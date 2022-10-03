using MingelBingoCreator.Data;
using Serilog;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    internal class TaggedCategoryIdentifier : ITaggedCategoryIdentifier
    {
        private const string ArgumentSplitCharacter = "_";
        private const string BeforeTagsSymbol = "#";

        public bool IsAnyTaggedCategory(string categoryHeading)
            => categoryHeading.Contains(BeforeTagsSymbol);

        public bool TryGetTaggedCategory(DataCategory dataCategory, CategoryTag categoryTag, out TaggedCategory taggedCategory)
        {
            taggedCategory = null;

            if (!dataCategory.Heading.Contains(categoryTag.Tag))
                return false;

            taggedCategory = new TaggedCategory(dataCategory.Heading, dataCategory.Values, categoryTag);

            if (categoryTag.HasArguments)
            {
                if (!TryGetArgument(dataCategory.Heading, out int foundArgument))
                {
                    Log.Error($"Failed to find argument for tagged category: {dataCategory.Heading}. Ignoring category tag.");
                    return false;
                }

                taggedCategory.HasArgument = true;
                taggedCategory.Argument = foundArgument;
            }

            return true;
        }

        private static bool TryGetArgument(string heading, out int argument)
        {
            argument = 0;

            var splitHeading = heading.Split(BeforeTagsSymbol);

            var tagPartOfHeading = splitHeading[1];

            var splitTagPartOfHeading = tagPartOfHeading.Split(ArgumentSplitCharacter);

            if (splitTagPartOfHeading.Length != 2)
            {
                Log.Warning($"Incorrect tag values. Too many {ArgumentSplitCharacter} in category heading: '{tagPartOfHeading}'");
                return false;
            }

            if (!int.TryParse(splitTagPartOfHeading[1].Trim(), out argument))
            {
                Log.Warning($"Failed to parse argument part of category tag to integer: {splitTagPartOfHeading[1]} ");
                return false;
            }

            return true;
        }
    }
}
