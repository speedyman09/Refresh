using Bunkum.CustomHttpListener.Listeners.Direct;
using Bunkum.HttpServer;
using NotEnoughLogs;
using RefreshTests.GameServer.Time;

namespace RefreshTests.GameServer.GameServer;

[Parallelizable]
[Timeout(2000)]
public class GameServerTest
{
    protected static readonly Logger Logger = new();
    
    // ReSharper disable once MemberCanBeMadeStatic.Global
    protected TestContext GetServer(bool startServer = true)
    {
        DirectHttpListener listener = new(Logger);
        HttpClient client = listener.GetClient();
        MockDateTimeProvider time = new();

        TestGameDatabaseProvider provider = new(time);

        Lazy<TestRefreshGameServer> gameServer = new(() =>
        {
            TestRefreshGameServer gameServer = new(listener, () => provider);
            gameServer.Start();

            return gameServer;
        });

        if (startServer) _ = gameServer.Value;
        else provider.Initialize();

        return new TestContext(gameServer, provider.GetContext(), client, listener, time);
    }
}