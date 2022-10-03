using Autofac;
using MingelBingoCreator.CardValueCreator;
using MingelBingoCreator.CardValueCreator.ValuesHandlerSelector;
using MingelBingoCreator.Configurations;
using MingelBingoCreator.DataGathering;
using MingelBingoCreator.FinalFileGenerator;
using MingelBingoCreator.Repository;
using Newtonsoft.Json;
using Serilog;

namespace MingelBingoCreator
{
    public class Program
    {
        static void Main(string[] args)
        {
            SetupLogger();

            var containerBuilder = new ContainerBuilder();

            ConfigureServices(containerBuilder);

            containerBuilder.RegisterType<ProgramStart>().As<ProgramStart>();

            var container = containerBuilder.Build();

            try
            {
                using var scope = container.BeginLifetimeScope();

                var programStart = scope.Resolve<ProgramStart>();

                programStart.Execute();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<JsonConfigurationsReader>().As<IConfigurationsReader>().SingleInstance();
            builder.RegisterType<GoogleSheetsRepository>().As<IRepository>().SingleInstance();

            builder.RegisterType<GoogleSheetsDataGatherer>().As<IDataGatherer>();
            builder.RegisterType<CategoryCardValueCreator>().As<ICardValueCreator>();
            builder.RegisterType<FinalSpreadSheetGenerator>().As<IFinalFileCreator>();

            builder.RegisterType<TaggedCategoriesValuesHandlerSelector>().As<IValuesHandlerSelector>();
            builder.RegisterType<TaggedCategoryIdentifier>().As<ITaggedCategoryIdentifier>();
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/mingelBingoCreator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}