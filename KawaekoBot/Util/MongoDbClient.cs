using MongoDB.Driver;

using Serilog;

namespace KawaekoBot.Util
{
    public static class MongoDbClient
    {
        public static MongoClient client;
        public static IMongoDatabase kawaekoDatabase;
        public static void InstantiateMongoDriver()
        {
            Log.Information("Instantiating Database Connection");
            var mongoConnectionString = AppSettings.Settings["mongoConnectionString"];
            client = new MongoClient(mongoConnectionString);
            kawaekoDatabase = client.GetDatabase("KawaekoBotDB");
            Log.Information("Database Connection complete");
        }
    }
}
