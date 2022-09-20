
using Newtonsoft.Json;

namespace MingelBingoCreator.Configurations
{
    public sealed class AppSettings
    {
        [JsonProperty(Required = Required.Always)]
        public ExportOptions ExportOptions { get; set; }

        [JsonProperty(Required = Required.Always)]
        public GoogleSheetsOptions GoogleSheetsOptions { get; set; }
    }
}
