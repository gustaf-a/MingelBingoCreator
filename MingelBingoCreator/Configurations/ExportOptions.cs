
using Newtonsoft.Json;

namespace MingelBingoCreator.Configurations
{
    public sealed class ExportOptions
    {
        [JsonProperty(Required = Required.Always)]
        public int NumberOfCards { get; private set; }

        [JsonProperty(Required = Required.Always)]
        public string FinalFileName { get; private set; }
    }
}
