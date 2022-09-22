
using Newtonsoft.Json;

namespace MingelBingoCreator.Configurations
{
    public sealed class ExportOptions
    {
        [JsonProperty(Required = Required.Always)]
        public int NumberOfCards { get; private set; }

        [JsonProperty(Required = Required.Always)]
        public object FinalFileName { get; private set; }
    }
}
