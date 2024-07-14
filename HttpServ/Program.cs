using System.Text.Json;

namespace HttpServ;

static class Program
{
    public static async Task Main()
    {
        Config config = new Config();
        if (File.Exists("../../../config.json"))
        {
            await using var stream = File.OpenRead("../../../config.json");
            config = await JsonSerializer.DeserializeAsync<Config>(stream) ?? config;
        }

        if (!config.IsReverseProxy)
        {
            WebServer server = new WebServer(config);
            Console.WriteLine($"""
                               Веб-сервер запущен
                               Адрес: {config.ServAddress}:{config.ServPort};
                               Корневая папка: {config.RootPath};
                               """);
            await server.ListenAsync();
        }
        else
        {
            ReverseProxy server = new ReverseProxy(config);
            Console.WriteLine($"""
                               Обратный прокси запущен
                               Адрес прокси: {config.ServAddress}:{config.ServPort};
                               Конечная точка: {config.EndPointAddress}:{config.EndPointPort};
                               """);
            await server.ListenAsync();
        }
    }
}