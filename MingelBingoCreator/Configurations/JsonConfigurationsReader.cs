using Newtonsoft.Json;
using Serilog;

namespace MingelBingoCreator.Configurations
{
    public class JsonConfigurationsReader : IConfigurationsReader
    {
        private AppSettings _appSettings;

        public JsonConfigurationsReader()
        {
            try
            {
                var rawFile = File.ReadAllText("appsettings.json");
                if (string.IsNullOrEmpty(rawFile))
                    throw new Exception("Failed to find or load appsettings.json file");

                var appSettings = JsonConvert.DeserializeObject<AppSettings>(rawFile);
                if (appSettings == null)
                    throw new Exception("Failed to deserialize file to AppSettings-object.");

                _appSettings = appSettings;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to read appsettings. Please ensure appsettings.json is correct: {0}", e.Message);
                throw;
            }
        }

        public AppSettings GetAppSettings()
            => _appSettings;
    }
}
